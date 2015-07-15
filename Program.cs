using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using System.IO;
using System.Net;

namespace UnbanApp
{
    class Program
    {

        static readonly Logger l = LogManager.GetCurrentClassLogger();
        static Dictionary<string, string> config = Utils.getDict();

        static void Main(string[] args)
        {
            try
            {
                string login = "mmm-94@bk.ru";
                string pass = "NWCneo000";
                l.Info("Программа запущена");
                VKApi api = new VKApi();
                if (!api.Authorize(login, pass, null, null))
                {
                    l.Error("failed to authorize! Wrong credentials.");
                }
                else
                {
                    string input = Console.ReadLine();
                    int sw;
                    Int32.TryParse(input, out sw);

                    switch (sw)
                    {
                        case 1:
                            Console.WriteLine("Starting count posts and likes");
                            Dictionary<long, int> users = new Dictionary<long, int>();
                            Dictionary<long, int> posts = new Dictionary<long, int>();

                            bool isFirstTime = true;
                            int offset = 0;
                            int postCount = 0;
                            l.Info("Started");
                            do
                            {
                                WallResponse wall = api.GetWallPosts("100", Convert.ToString(offset));
                                if (isFirstTime)
                                {
                                    postCount = wall.response.count;
                                    isFirstTime = false;
                                }
                                if (postCount < wall.response.count)
                                {
                                    offset += wall.response.count - postCount;
                                    postCount = wall.response.count;
                                }

                                foreach (var w in wall.response.items)
                                {
                                    if (!users.ContainsKey(w.from_id))
                                    {
                                        users.Add(w.from_id, 0);
                                    }

                                    users[w.from_id] += w.likes.count;
                                    if (!posts.ContainsKey(w.from_id))
                                    {
                                        posts.Add(w.from_id, 0);
                                    }
                                    posts[w.from_id]++;
                                }
                                offset += 100;
                            } while (offset < 200000);
                            Console.Write("Here we are");
                            var sortedUsers = from entry in users orderby entry.Value ascending select entry;
                            var sortedPosts = from entry in posts orderby entry.Value ascending select entry;
                            using (var wr = new StreamWriter("sortedUsers.txt"))
                            {
                                foreach (var entry in sortedUsers)
                                {
                                    wr.Write("[{0} {1}]", entry.Key, entry.Value);
                                }
                            }
                            using (var wr = new StreamWriter("sortedPosts.txt"))
                            {
                                foreach (var entry in sortedPosts)
                                {
                                    wr.WriteLine("[*id{0} {1}]", entry.Key, entry.Value);
                                }
                            }
                            break;
                        case 2:
                            Console.WriteLine("Starting count posts");
                            Console.Write("ss");
                            Console.Read();
                            break;
                        default: 
                            break;
                        //id = 115841733;
                        //string group_id = "-88435";
                        //string question = "Забанить?";
                        //string answer = "[\"Yes pls\", \"No pls\"]";
                        //PollResponse poll = api.createPoll(group_id, question, answer);
                        //string attachments = "poll" + poll.response.owner_id + "_" + poll.response.id;
                        //string message = "Сыграем в игру?";
                        //api.PostMessage(group_id, message, attachments);

                    }

                    }
                    
            }
            catch (WebException e)
            {
                l.Log(LogLevel.Error, "WebException", e);
            }
        }
    }
}
