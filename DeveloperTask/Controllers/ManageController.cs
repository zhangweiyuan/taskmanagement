using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Helper;

namespace DeveloperTask.Controllers
{

    [ValidateAdmin]
    public class ManageController : Controller
    {
        Db.TaskDbContext db = new Db.TaskDbContext();

        #region 用户管理
        [HttpGet]
        public ActionResult QueryUsers()
        {
            List<Models.Users> users = db.Users.Where(w => w.is_delete == 0).ToList();
            return Json(new Models.Result(Models.ResultEnum.Success, users, string.Empty), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult QueryUser(int id)
        {
            var user = db.Users.Where(w => w.is_delete == 0 && w.id == id).FirstOrDefault();
            return Json(new Models.Result(Models.ResultEnum.Success, user, string.Empty), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Introduction()
        {
            return View();
        }
        public ActionResult Users()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddUser(FormCollection user)
        {
            if (string.IsNullOrEmpty(user["name"]))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入姓名"));
            }
            string account = user["account"].ToString();
            if (string.IsNullOrEmpty(account))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入手机号"));
            }
            if (!Comm.ValidateMobile(account))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "手机号格式错误"));
            }
            //检查手机号是否存在
            if (db.Users.Where(w => w.account == account).Count() > 0)
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "手机号已存在"));
            }

            string email = user["email"];
            if (string.IsNullOrEmpty(email))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入Email"));
            }
            if (!Comm.ValidateEmail(email))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "Email格式错误"));
            }
            //检查email是否存在
            if (db.Users.Where(w => w.email == email).Count() > 0)
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "Email已存在"));
            }
            if (string.IsNullOrEmpty(user["password"]))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入密码"));
            }
            if (user["password"] != user["password2"])
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "两次密码不一致"));
            }

            Models.Users user2 = new Models.Users()
            {
                account = account,
                email = email,
                name = user["name"],
                password = user["password2"].GetMd5(),
                create_time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                is_delete = 0,
                system_authority = user["system_authority"]
            };

            db.Users.Add(user2);
            var res = db.SaveChanges() > 0;
            if (res)
            {
                return Json(new Models.Result(Models.ResultEnum.Success, "success", "创建成功"));
            }
            else
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "创建失败"));
            }
        }


        [HttpPost]
        public ActionResult RemoveUser(int id)
        {
            var user = new Models.Users() { id = id };
            db.Entry(user).State = System.Data.Entity.EntityState.Unchanged;
            user.is_delete = 1;
            var res = db.SaveChanges() > 0;
            Models.Result result;
            if (res)
            {
                result = new Models.Result(Models.ResultEnum.Success, string.Empty);
            }
            else
            {
                result = new Models.Result(Models.ResultEnum.Fail, string.Empty);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult ResetPassword(int id)
        {
            int uid = Convert.ToInt32(Session["uid"]);
            if (id != uid)
            {
                //我是否有权利修改其他人的密码
                Models.Users suser = Session["user"] as Models.Users;
                if (!suser.authoritys.Contains(3))
                {
                    return Content("您的权限不够，<a href='javascript:history.go(-1);'>返回</a>");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult ResetPasswordAction(int id, string password, string password2)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入密码"));
            }
            if (password != password2)
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "两次密码不一致"));
            }

            int uid = Convert.ToInt32(Session["uid"]);
            if (id != uid)
            {
                //我是否有权利修改其他人的密码
                Models.Users suser = Session["user"] as Models.Users;
                if (!suser.authoritys.Contains(3)) {
                    return Json(new Models.Result(Models.ResultEnum.Fail, "你没有权限操作"), JsonRequestBehavior.AllowGet);
                }
            }

            var user = new Models.Users() { id = id };
            db.Entry(user).State = System.Data.Entity.EntityState.Unchanged;
            user.password = password.GetMd5();
            var res = db.SaveChanges() > 0;
            Models.Result result;
            if (res)
            {
                result = new Models.Result(Models.ResultEnum.Success, "密码重置成功");
            }
            else
            {
                result = new Models.Result(Models.ResultEnum.Fail, "密码重置失败");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult EditUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditUserAction(int id, string name, string account, string email, string system_authority)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入姓名"));
            }
            if (string.IsNullOrEmpty(account))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入手机号"));
            }
            if (!Comm.ValidateMobile(account))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "手机号格式错误"));
            }
            //检查手机号是否存在
            if (db.Users.Where(w => w.account == account && w.id != id).Count() > 0)
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "手机号已存在"));
            }
            if (string.IsNullOrEmpty(email))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入Email"));
            }
            if (!Comm.ValidateEmail(email))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "Email格式错误"));
            }
            //检查手机号是否存在
            if (db.Users.Where(w => w.email == email && w.id != id).Count() > 0)
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "Email已存在"));
            }

            var euser = new Models.Users() { id = id };
            db.Entry(euser).State = System.Data.Entity.EntityState.Unchanged;
            euser.name = name;
            euser.account = account;
            euser.email = email;
            euser.system_authority = system_authority;
            var res = db.SaveChanges() > 0;
            Models.Result result;
            if (res)
            {
                result = new Models.Result(Models.ResultEnum.Success, "更新成功");
            }
            else
            {
                result = new Models.Result(Models.ResultEnum.Fail, "更新失败");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 任务管理


        public ActionResult AddTask()
        {

            return View();
        }


        public ActionResult Tasks()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveEdit(Models.Tasks task)
        {
            
            if (string.IsNullOrEmpty(task.task_content))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入任务内容"));
            }
            if (string.IsNullOrEmpty(task.task_name))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入任务标题"));
            }

            Models.Tasks newtask = new Models.Tasks()
            {
                id = task.id
            };
            db.Entry(newtask).State = System.Data.Entity.EntityState.Unchanged;

            //newtask.task_begin_time = task.task_begin_time;
            //newtask.task_end_time = task.task_end_time;

            newtask.task_change_time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            newtask.author_userid = Convert.ToInt32(Session["uid"]);
            newtask.task_content = task.task_content;
            newtask.task_developer = task.task_developer;
            newtask.task_name = task.task_name;
            newtask.task_type = task.task_type;
            newtask.task_priority = task.task_priority;
            newtask.task_view = task.task_view;

            var res = db.SaveChanges() > 0;
            if (res)
            {
                return Json(new Models.Result(Models.ResultEnum.Success, "success", "保存成功"));
            }
            else
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "保存失败"));
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveCreate(Models.Tasks task)
        {
            if (string.IsNullOrEmpty(task.task_name))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入任务标题"));
            }
            if (string.IsNullOrEmpty(task.task_content))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入任务内容"));
            }
            task.task_create_time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            task.author_userid = Convert.ToInt32(Session["uid"]);

            db.Tasks.Add(task);
            var res = db.SaveChanges() > 0;
            if (res)
            {
                return Json(new Models.Result(Models.ResultEnum.Success, "success", "保存成功"));
            }
            else
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "保存失败"));
            }
        }


        [HttpGet]
        public ActionResult QueryTasks(int type, int page, int size)
        {
            int uid = Convert.ToInt32(Session["uid"]);
            string where = type > -1 ? " and task_type=" + type : "";
            var tasks = db.Database.SqlQuery<Models.Tasks>($"select * from tasks where author_userid={uid} " + where +
                    $" order by task_create_time desc limit {(page - 1) * size},{size}");

            var list = tasks.ToList<Models.Tasks>();

            var list2 = from q in list
                        select new
                        {
                            q,
                            e = q.task_developer.Split(','),
                        };


            var list3 = from a in list2
                        select new
                        {
                            task = a.q,
                            users = from u in db.Users
                                    where a.e.Contains(u.id.ToString())
                                    select u
                        };

            return Json(list3, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult QueryTaskCount(int type)
        {
            int uid = Convert.ToInt32(Session["uid"]);
            string where = type > -1 ? " and task_type=" + type : "";
            var count = db.Database.SqlQuery<int>($"select COUNT(*) from tasks where author_userid={uid}  " + where);
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult QueryInfo(int id)
        {
            var entity = db.Tasks.Find(id);
            if (entity == null)
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "该任务已删除"), JsonRequestBehavior.AllowGet);
            }
            //当前登录用户是否有权利获取该条内容
            if (entity.author_userid != Convert.ToInt32(Session["uid"]))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "您没有权限查看"), JsonRequestBehavior.AllowGet);
            }
            return Json(new Models.Result(Models.ResultEnum.Success, entity, string.Empty), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveTask(int id)
        {
            var task = db.Tasks.Find(id);
            if (task == null)
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "该任务不存在"), JsonRequestBehavior.AllowGet);
            }

            if (task.author_userid != Convert.ToInt32(Session["uid"]))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "您没有权限操作"), JsonRequestBehavior.AllowGet);
            }

            db.Tasks.Remove(task);
            bool res = db.SaveChanges() > 0;
            Models.Result result = new Models.Result();
            if (res)
            {
                result = new Models.Result(Models.ResultEnum.Success);
            }
            else
            {
                result = new Models.Result(Models.ResultEnum.Fail, "删除失败");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 小组管理

        public ActionResult Groups()
        {
            return View();
        }


        [HttpGet]
        public ActionResult AddGroup()
        {
            return View();
        }

        public ActionResult EditGroup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddGroup(FormCollection coll)
        {
            Models.Groups group = new Models.Groups()
            {
                name = coll["name"],
                create_time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            };
            if (db.Groups.Where(w => w.name == group.name).Count() > 0)
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "小组已存在"));
            }
            db.Groups.Add(group);
            var res = db.SaveChanges() > 0;

            if (res)
            {
                if (!string.IsNullOrEmpty(coll["users"]))
                {
                    int[] users = Array.ConvertAll<string, int>(coll["users"].ToString().Split(','), num => Convert.ToInt32(num));
                    List<Models.GroupUsers> groupusers = new List<Models.GroupUsers>();
                    foreach (var uid in users)
                    {
                        db.GroupUsers.Add(new Models.GroupUsers()
                        {
                            group_id = group.id,
                            user_id = uid,
                            create_time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                        });
                    }
                    db.SaveChanges();
                }
                return Json(new Models.Result(Models.ResultEnum.Success, "success", "创建成功"));
            }
            else
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "创建失败"));
            }
        }



        [HttpPost]
        public ActionResult EditGroup(FormCollection coll)
        {
            var group = new Models.Groups()
            {
                id = Convert.ToInt32(coll["id"])
            };

            db.Entry(group).State = System.Data.Entity.EntityState.Unchanged;
            group.name = coll["name"];

            if (db.Groups.Where(w => w.name == group.name && w.id != group.id).Count() > 0)
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "小组已存在"));
            }
            var res = db.SaveChanges() > 0;

            if (res)
            {
                if (!string.IsNullOrEmpty(coll["users"]))
                {
                    int[] newusers = Array.ConvertAll<string, int>(coll["users"].ToString().Split(','), num => Convert.ToInt32(num));
                    //获取现有的
                    int[] oldusers = db.GroupUsers.Where(w => w.group_id == group.id).Select(s => s.user_id).ToArray();

                    //int[] concatusers = oldusers.Concat(newusers).ToArray();
                    //找到新增的：新的有旧的没有代表是新增
                    foreach (var n in newusers)
                    {
                        if (!oldusers.Contains(n))
                        {
                            db.GroupUsers.Add(new Models.GroupUsers()
                            {
                                group_id = group.id,
                                user_id = n,
                                create_time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                            });
                        }
                    }
                    //找到删除的：旧的有新的没有代表是删除
                    foreach (var o in oldusers)
                    {
                        if (!newusers.Contains(o))
                        {
                            db.GroupUsers.Remove(db.GroupUsers.Where(w => w.group_id == group.id && w.user_id == o).FirstOrDefault());
                        }
                    }

                    db.SaveChanges();
                }
                return Json(new Models.Result(Models.ResultEnum.Success, "success", "更新成功"));
            }
            else
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "更新失败"));
            }
        }


        [HttpPost]
        public ActionResult RemoveGroup(int id)
        {
            db.Groups.Remove(db.Groups.Find(id));
            db.GroupUsers.RemoveRange(db.GroupUsers.Where(w => w.group_id == id));
            bool res = db.SaveChanges() > 0;
            if (res)
            {
                return Json(new Models.Result(Models.ResultEnum.Success, "success", "删除成功"));
            }
            else
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "删除失败"));
            }
        }

        [HttpGet]
        public ActionResult QueryGroups()
        {

            var groups = from g in db.Groups
                         select new
                         {
                             g,
                             count = db.GroupUsers.Where(w => w.group_id == g.id).Count()
                         };
            return Json(new Models.Result(Models.ResultEnum.Success, groups, "获取成功"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult QueryGroupsAction(int id)
        {

            var groups = (from g in db.Groups
                          where g.id == id
                          select new
                          {
                              g,
                              users = db.GroupUsers.Where(w => w.group_id == g.id)
                          }).FirstOrDefault();
            return Json(new Models.Result(Models.ResultEnum.Success, groups, "获取成功"), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}