using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Users: UserExts
    {
        public int id { get; set; }
        public string name { get; set; }
        public string account { get; set; }
        public string password { get; set; }
        public string weixin_openid { get; set; }
        public string email { get; set; }
        public int sex { get; set; }
        public DateTime? create_time { get; set; }
        /// <summary>
        /// 0正常 1删除
        /// </summary>
        public int? is_delete { get; set; }
        public string system_authority { get; set; }
    }

    public class UserExts
    {
        public int[] authoritys { get; set; }
    }
}