using System;
using System.Collections.Generic;

namespace UnbanApp
{
	public class Response
	{
		public int count { get; set; }

		public List<Item> items { get; set; }
	}

	public class WallResponse
	{
		public Response response { get; set; }
	}

	public class Item
	{
		public long id { get; set; }

		public long from_id { get; set; }

		public int date { get; set; }

		public string text { get; set; }

        public like likes { get; set; }
	}
    public class like
    {
        public int count { get; set; }
    }


}

