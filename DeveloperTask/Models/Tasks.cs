using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Tasks
    {
        public int id { get; set; }
        public int author_userid { get; set; }
        public string task_name { get; set; }
        public string task_content { get; set; }
        public DateTime? task_begin_time { get; set; }
        public DateTime? task_end_time { get; set; }
        public int task_status { get; set; }
        public int task_is_delete { get; set; }
        /// <summary>
        /// 0普通 1优先 2紧急 数字越小优先级别越高
        /// </summary>
        public int task_priority { get; set; }
        public DateTime? task_create_time { get; set; }
        public DateTime? task_change_time { get; set; }
        public string task_developer { get; set; }
        public string task_view { get; set; }
        /// <summary>
        /// 0Bug 1新增 2优化 3升级
        /// </summary>
        public int task_type { get; set; }
        public DateTime? task_success_time { get; set; }

    }
}