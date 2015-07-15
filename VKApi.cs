using System.Net;
using System;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System.Threading;
using System.Linq;

namespace UnbanApp
{
    public class VKApi
    {
        static readonly Logger l = LogManager.GetCurrentClassLogger();
        const string CLIENT_ID = "2983858";
        const string CLIENT_SECRET = "BcCd0UrnjsocGv7lcineuHtegno4";
        const string AUTH_URL = "https://api.vk.com/oauth/token";
        const string API_METHOD_URL = "https://api.vk.com/method/";
        const string VERSION = "5.34";
        string token;
        string accessTokenFileName = "token.txt";

        public VKApi()
        {
        }

        public bool Authorize(string mail, string pass, string captcha_key, string captcha_sid)
        {
            if (File.Exists(accessTokenFileName))
            {
                token = File.ReadAllText(accessTokenFileName);
                return true;

            }


            var values = new NameValueCollection();
            values["grant_type"] = "password";
            values["client_id"] = CLIENT_ID;
            values["client_secret"] = CLIENT_SECRET;
            values["username"] = mail;
            values["password"] = pass;
            values["scope"] = "wall,notify,friends,photos,audio,video,docs,messages,groups,stats";
            values["v"] = VERSION;
            if (captcha_key != null && captcha_sid != null)
            {
                values["captcha_sid"] = captcha_sid;
                values["captcha_key"] = captcha_key;
            }
            AuthResult ar;
            try
            {
                ar = performPostRequest<AuthResult>(AUTH_URL, values);
            }
            catch (WebException e)
            {
                var response = ((HttpWebResponse)e.Response);
                if (response == null)
                {
                    return false;//no internet/connection
                }
                using (var stream = response.GetResponseStream())
                {
                    var responseString = new StreamReader(stream).ReadToEnd();
                    ar = JsonConvert.DeserializeObject<AuthResult>(responseString);
                }
                if (ar.error == "need_captcha")
                {
                    l.Error("Need captcha: " + ar.captcha_img);
                    string captcha = Console.ReadLine();
                    return Authorize(mail, pass, captcha, ar.captcha_sid);
                }
                if (ar.error == "invalid_client")
                {
                    l.Error("Wrong credentials");
                    return false;
                }
            }




            token = ar.access_token;
            using (var wr = new StreamWriter(accessTokenFileName))
            {
                wr.Write(token);
            }
            return true;

        }

        public WallResponse GetWallPosts(string postCount, string offset)
        {
            var values = new NameValueCollection();
            values["domain"] = "ncworld";
            values["count"] = postCount;
            values["filter"] = "others";
            values["offset"] = offset;
            values["access_token"] = token;
            values["v"] = VERSION;
            return performMethod<WallResponse>("wall.get", values);
        }

        public CommentResponse getComments(long post_id, int count, int offset)
        {
            var values = new NameValueCollection();
            values["owner_id"] = "-88435";
            values["post_id"] = post_id.ToString();
            values["count"] = count.ToString();
            values["offset"] = offset.ToString();
            values["access_token"] = token;
            values["v"] = VERSION;
            l.Debug("post id - " + post_id);
            return performMethod<CommentResponse>("wall.getComments", values);
            //			if (allcomments.response == null) {
            //				allcomments.response = comments.response;
            //			}
            //			if (offset != 0) {
            //				foreach (var c in comments.response.items.ToList()) {
            //					allcomments.response.items.Add (c);
            //				}
            //			}
            //			if (comments.response.count >= Convert.ToInt32 (values ["count"]) + Convert.ToInt32 (offset)) {
            //				offset = Convert.ToInt32 (values ["count"]) + Convert.ToInt32 (offset);
            //				getComments (post_id, count, offset, allcomments);
            //			}
            //			return allcomments;
        }

        public BanResponse DeleteMessage(long post_id)
        {
            var values = new NameValueCollection();
            values["owner_id"] = "-88435";
            values["post_id"] = post_id.ToString();
            values["access_token"] = token;
            values["v"] = VERSION;
            l.Debug("post id - " + post_id);
            return performMethod<BanResponse>("wall.delete", values);
        }

        public BanResponse DeleteComment(long comment_id)
        {
            var values = new NameValueCollection();
            values["owner_id"] = "-88435";
            values["comment_id"] = comment_id.ToString();
            values["access_token"] = token;
            values["v"] = VERSION;
            l.Debug("comment id - " + comment_id);
            return performMethod<BanResponse>("wall.deleteComment", values);
        }

        public BanResponse SendMessage(long uID, string text, string chat_id, string postLink)
        {

            var values = new NameValueCollection();
            if (chat_id == null)
            {
                values["user_id"] = uID.ToString();
                values["message"] = "Вы были забанены автоботом в группе Numerus Community за \"" + text + "\"";
            }
            else
            {
                values["chat_id"] = chat_id;
                values["message"] = "Не смог заблокировать [id" + uID + "|нарушителя] за сообщение: " + text + " \nСсылка на сообщение - https://vk.com/ncworld?w=wall-88435_" + postLink;
            }

            values["access_token"] = token;
            values["v"] = VERSION;
            return performMethod<BanResponse>("messages.send", values);
        }

        public BanResponse BanUser(long uID, string text, int banTime)
        {
            string banEndTime = Convert.ToString((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + (banTime * 60));


            var values = new NameValueCollection();
            values["group_id"] = "88435";
            values["user_id"] = uID.ToString();
            values["end_date"] = banEndTime;
            values["comment"] = "Вы были забанены автоботом за \"" + text + "\"";
            values["access_token"] = token;
            values["v"] = VERSION;
            values["comment_visible"] = "1";
            return performMethod<BanResponse>("groups.banUser", values);
        }

        public BanResponse PostMessage(string uID, string message, string attachments)  // хуево сделан аттачмент, надо переделать
        {

            
            var values = new NameValueCollection();
            values["user_id"] = uID.ToString();
            values["message"] = message;
            values["access_token"] = token;
            values["owner_id"] = uID;
            values["attachments"] = attachments;  // доделать епты
            values["v"] = VERSION;
            values["acces_token"] = token;
            return performMethod<BanResponse>("wall.post", values);
        }

        public PollResponse createPoll(string owner_id, string question, string answ)
        {
            var values = new NameValueCollection();
            values["question"] = question;
            values["owner_id"] = owner_id;
            values["add_answers"] = answ;
            values["access_token"] = token;
            values["v"] = VERSION;
            return performMethod<PollResponse>("polls.create", values);
        }

        public T performMethod<T>(string method, NameValueCollection values)
        {
            string url = API_METHOD_URL + method;
            l.Debug(method + " --- " + toStringDebug(values));
            return performPostRequest<T>(url, values);
        }

        public T performPostRequest<T>(string url, NameValueCollection values)
        {
            using (var client = new WebClient())
            {
                Thread.Sleep(900);
                var response = client.UploadValues(url, values);
                var responseString = Encoding.UTF8.GetString(response);
                return JsonConvert.DeserializeObject<T>(responseString);

            }

        }

        string toStringDebug(NameValueCollection values)
        {
            StringBuilder sb = new StringBuilder("{");
            foreach (var key in values)
            {
                sb.Append(key);
                sb.Append(":");
                sb.Append(values[key.ToString()]);
                sb.Append(",");
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}



