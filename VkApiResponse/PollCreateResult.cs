using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnbanApp
{

    public class ResponsePoll
    {
        public int id { get; set; }
        public int owner_id { get; set; }
    }

    public class PollResponse
    {
        public ResponsePoll response { get; set; }
    }
}
