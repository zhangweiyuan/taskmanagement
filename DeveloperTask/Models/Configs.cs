using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Configs
    {
        public static List<string> task_type {
            get {
                return new List<string>() {
                    { "Bug" },
                    { "新增" },
                    { "优化" },
                    { "升级" }
                };
            }
        }


        public static List<string> task_priority {
            get {
                return new List<string>() {
                    { "普通" },
                    { "优先" },
                    { "紧急" }
                };
            }
        }

        public static List<string> system_authority {
            get {
                return new List<string>() {
                    { "修改密码" },//0
                    { "创建任务" },//1
                    { "管理任务" },//2
                    { "添加员工" },//3
                    { "员工管理" },//4
                    { "创建小组" },//5
                    { "小组管理" },//6
                };
            }
        }
    }
}