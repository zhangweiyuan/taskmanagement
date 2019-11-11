using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Groups
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime? create_time { get; set; }
    }

    public class GroupUsers
    {
        public int id { get; set; }
        public int group_id { get; set; }
        public int user_id { get; set; }
        public DateTime? create_time { get; set; }
    }
}