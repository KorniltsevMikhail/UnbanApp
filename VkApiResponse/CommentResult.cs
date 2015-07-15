using System;
using System.Collections.Generic;

namespace UnbanApp
{


	public class Items
	{
		public long id { get; set; }

		public long from_id { get; set; }

		public int date { get; set; }

		public string text { get; set; }

		public int? reply_to_user { get; set; }

		public int? reply_to_comment { get; set; }
	}

	public class Responses
	{
		public int count { get; set; }
		public List<Items> items { get; set; }
	}

	public class CommentResponse
	{
		public Responses response { get; set; }
	}

}

