using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Helper;
using System.Web.Helpers;
using System.Web.Configuration;

namespace DeveloperTask.Controllers
{
    [ValidateLogin]
    public class UserController : Controller
    {
        Db.TaskDbContext db = new Db.TaskDbContext();



        // GET: User

        [HttpGet]
        [ValidateLogin(false)]
        [Route("/")]
        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [ValidateLogin(false)]
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return Json(true);
        }

        [HttpPost]
        [ValidateLogin(false)]
        public ActionResult LoginAction(string account, string password)
        {
            if (string.IsNullOrEmpty(account))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入登录账号"));
            }

            if (string.IsNullOrEmpty(password))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "请输入登录密码"));
            }
            string pwd_md5 = password.GetMd5();

            var user = db.Users.Where(w => w.account == account && w.password == pwd_md5 && w.is_delete == 0).AsNoTracking().FirstOrDefault();
            if (user != null)
            {
                //Models.Users user = user as Models.Users;
                user.authoritys = Array.ConvertAll<string, int>(user.system_authority.Split(','), v => Convert.ToInt32(v));
                Session["user"] = user;
                Session["uid"] = user.id;
                Session["name"] = user.name;

                return Json(new Models.Result(Models.ResultEnum.Success, "success", "登录成功"));
            }
            else
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "error", "账号或密码错误"));
            }
        }

        public ActionResult Center()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Endorse(int task_id)
        {
            int uid = Convert.ToInt32(Session["uid"]);
            var endorse = db.Endorses.Where(w => w.task_id == task_id && w.user_id == uid).FirstOrDefault();
            if (endorse != null)
            {
                //存在
                db.Endorses.Remove(endorse);
            }
            else
            {
                //不存在
                db.Endorses.Add(new Models.Endorses() { create_time = DateTime.Now, task_id = task_id, user_id = uid });
            }

            int a = db.SaveChanges();
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult QueryMyTasks(bool isok, int type, int page, int size)
        {
            int uid = Convert.ToInt32(Session["uid"]);
            string where = type > -1 ? " and task_type=" + type : "";
            var tasks = db.Database.SqlQuery<Models.Tasks>($"select * from tasks where 1=1" +
                (isok ? ($" and task_status=2 ") : (" and task_status=0 ")) +
                $" and (FIND_IN_SET({uid},task_developer)>0 or (select count(*) from groupusers where FIND_IN_SET(group_id,task_view) and user_id={uid})>0) " + where +
                $" order by task_create_time desc limit {(page - 1) * size},{size}");
            //谁能看：如果当前用户ID在小组内则可以看
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
                            publish = db.Users.Find(a.q.author_userid).name,
                            users = from u in db.Users
                                    where a.e.Contains(u.id.ToString())
                                    select u,
                            endorse = from e in db.Endorses
                                      where e.task_id == a.q.id
                                      select new
                                      {
                                          e.user_id,
                                          name = (from c in db.Users where c.id == e.user_id select c.name).FirstOrDefault()
                                      }
                        };

            return Json(list3, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult QueryMyTaskCount(bool isok, int type)
        {
            int uid = Convert.ToInt32(Session["uid"]);
            string where = type > -1 ? " and task_type=" + type : "";
            var count = db.Database.SqlQuery<int>($"select COUNT(*) from tasks where 1=1 " +
                (isok ? ($" and task_status=2 ") : (" and task_status=0 ")) +
               $" and (FIND_IN_SET({uid},task_developer)>0 or (select count(*) from groupusers where FIND_IN_SET(group_id,task_view) and user_id={uid})>0) " + where);

            return Json(count, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Detail(int id)
        {
            var entity = db.Tasks.Find(id);
            int uid = Convert.ToInt32(Session["uid"]);

            bool isView = false, isExec = false;
            if (entity.task_view != null)
            {
                string sql = $"select count(*) from groupusers where group_id in ({entity.task_view}) and user_id={uid}";
                var c = db.Database.SqlQuery<int>(sql).ToList()[0];
                isView = c.ToInt32() > 0;
            }

            //我是否在该任务中，是否有权利查看
            int[] arr = Array.ConvertAll<string, int>(entity.task_developer.ToString().Split(','), n => Convert.ToInt32(n));
            isExec = arr.Contains(uid);

            //我是否有权限看
            if (isView || isExec)
            {

            }
            else
            {
                return Content("您的权限不够，<a href='javascript:history.go(-1);'>返回</a>");
            }

            ViewBag.detail = entity;
            return View();
        }

        [HttpPost]
        public ActionResult changeSuccess(int taskid, int status)
        {
            var entity = db.Tasks.Find(taskid);
            //我是否在该任务中，是否有权利查看
            int[] arr = Array.ConvertAll<string, int>(entity.task_developer.ToString().Split(','), n => Convert.ToInt32(n));
            if (!arr.Contains(Convert.ToInt32(Session["uid"])))
            {
                return Json(new Models.Result(Models.ResultEnum.Fail, "你没有权限操作"), JsonRequestBehavior.AllowGet);
            }
            var isok = db.Database.ExecuteSqlCommand($"update tasks set task_status={status},task_success_time=now() where id={taskid}");
            return Json(new Models.Result(Models.ResultEnum.Success), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Remind(int task_id)
        {
            int uid = Session["uid"].ToInt32();
            var task = db.Tasks.Find(task_id);
            int[] exes = Array.ConvertAll<string, int>(task.task_developer.Split(','), n => Convert.ToInt32(n));
            int res = 0;
            int Interval = WebConfigurationManager.AppSettings["Interval"].ToInt32();
            int m = db.Database.SqlQuery<int>($"SELECT count(*) from remind where task_id={task_id} and user_id={uid} and TIMESTAMPDIFF(MINUTE,create_time,now())<{Interval}").ToList()[0];
            if (m > 0)
            {
                res = Interval;
            }
            else
            {
                for (int i = 0; i < exes.Count(); i++)
                {
                    var user = db.Users.Find(exes[i]);
                    if (user.email != null)
                    {
                        try
                        {


                            WebMail.UserName = WebConfigurationManager.AppSettings["EmailFrom"];// "admin@xunyoukeji.com";
                            WebMail.SmtpServer = WebConfigurationManager.AppSettings["SmtpServer"];//"smtp.qq.com";
                            WebMail.SmtpPort = WebConfigurationManager.AppSettings["SmtpPort"].ToInt32();// 25;
                            WebMail.From = WebConfigurationManager.AppSettings["EmailFrom"];//"admin@xunyoukeji.com";
                            WebMail.Password = WebConfigurationManager.AppSettings["EmailPassword"];//"iowcixbzpehgbfad";
                            WebMail.Send(user.email, "【BUG跟踪】" + task.task_name, task.task_content);

                            db.Remind.Add(new Models.Remind() { task_id = task_id, create_time = DateTime.Now, user_id = uid });
                            if (db.SaveChanges() > 0)
                            {
                                res = 1;
                            }
                            else
                            {
                                res = 0;
                            }
                        }
                        catch (Exception)
                        {
                            res = -1;
                        }
                    }
                }
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}