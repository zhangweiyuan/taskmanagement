using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Helper;

namespace DeveloperTask.Controllers
{
    [ValidateLogin]
    public class UserController : Controller
    {
        Db.TaskDbContext db = new Db.TaskDbContext();
        // GET: User
        [HttpGet]
        [ValidateLogin(false)]
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


        [HttpGet]
        public ActionResult QueryMyTasks(bool isok, int type, int page, int size)
        {
            int uid = Convert.ToInt32(Session["uid"]);
            string where = type > -1 ? " and task_type=" + type : "";
            var tasks = db.Database.SqlQuery<Models.Tasks>($"select * from tasks where 1=1" +
                (isok ? ($" and task_status=2 ") : (" and task_status=0 ")) +
                $" and task_developer REGEXP '^{uid}|{uid},|,{uid}$' " + where +
                    $" order by task_priority desc,task_begin_time asc,task_create_time desc,task_change_time desc limit {(page - 1) * size},{size}");

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
                            publish=db.Users.Find(a.q.author_userid).name,
                            users = from u in db.Users
                                    where a.e.Contains(u.id.ToString())
                                    select u
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
                $" and task_developer REGEXP '^{uid}|{uid},|,{uid}$' " + where);
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Detail(int id)
        {
            var entity = db.Tasks.Find(id);
            //我是否在该任务中，是否有权利查看
            int[] arr = Array.ConvertAll<string, int>(entity.task_developer.ToString().Split(','), n => Convert.ToInt32(n));
            if (!arr.Contains(Convert.ToInt32(Session["uid"])))
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

    }
}