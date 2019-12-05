using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Endorses
    {
        public int id  { get;set; }
        public int task_id { get; set; }
        public int user_id { get; set; }
        public DateTime create_time { get; set; }
    }
}