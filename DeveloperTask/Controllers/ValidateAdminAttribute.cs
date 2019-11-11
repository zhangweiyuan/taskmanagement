using System.Linq;
using System.Web.Mvc;

namespace DeveloperTask.Controllers
{
    public class ValidateAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //这个方法是在Action执行之前调用
            var user = filterContext.HttpContext.Session["user"];
            if (user == null)
            {
                filterContext.Result = new RedirectResult("/user/login");
            }
            else
            {
                if (filterContext.RouteData.Values["controller"].ToString() == "manage") {
                    Models.Users model = filterContext.HttpContext.Session["user"] as Models.Users;
                    switch (filterContext.RouteData.Values["action"].ToString())
                    {
                        case "addtask":
                            if (!model.authoritys.Contains(1)) {
                                filterContext.Result = new ContentResult() { Content = "您的权限不够，<a href='javascript:history.go(-1);'>返回</a>" };
                            }
                            break;
                        case "tasks":
                            if (!model.authoritys.Contains(2))
                            {
                                filterContext.Result = new ContentResult() { Content = "您的权限不够，<a href='javascript:history.go(-1);'>返回</a>" };
                            }
                            break;
                        case "users":
                            if (!model.authoritys.Contains(3))
                            {
                                filterContext.Result = new ContentResult() { Content = "您的权限不够，<a href='javascript:history.go(-1);'>返回</a>" };
                            }
                            break;
                        case "adduser":
                            if (!model.authoritys.Contains(4))
                            {
                                filterContext.Result = new ContentResult() { Content = "您的权限不够，<a href='javascript:history.go(-1);'>返回</a>" };
                            }
                            break;
                        case "addgroup":
                            if (!model.authoritys.Contains(5))
                            {
                                filterContext.Result = new ContentResult() { Content = "您的权限不够，<a href='javascript:history.go(-1);'>返回</a>" };
                            }
                            break;
                        case "groups":
                            if (!model.authoritys.Contains(6))
                            {
                                filterContext.Result = new ContentResult() { Content = "您的权限不够，<a href='javascript:history.go(-1);'>返回</a>" };
                            }
                            break;
                        case "resetpassword":
                            if (!model.authoritys.Contains(0))
                            {
                                filterContext.Result = new ContentResult() { Content = "您的权限不够，<a href='javascript:history.go(-1);'>返回</a>" };
                            }
                            break;
                    }
                }
                
            }
            base.OnActionExecuting(filterContext);
        }
    }
}