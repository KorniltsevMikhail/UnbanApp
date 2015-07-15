using System.Net;
using System;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace UnbanApp
{
	public class AuthResult
	{

		public string access_token;
		public long user_id;
		public string error;
		public string captcha_sid;
		public string captcha_img;
	}
	
}
