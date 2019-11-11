using System.Web.Mvc;

namespace DeveloperTask.Controllers
{
    public class ValidateLoginAttribute : ActionFilterAttribute
    {
        public bool IsValidate { get; set; }
        public ValidateLoginAttribute(bool validate = true)
        {
            this.IsValidate = validate;
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.IsValidate)
            {
                //这个方法是在Action执行之前调用
                var user = filterContext.HttpContext.Session["user"];
                if (user == null)
                {
                    filterContext.Result = new RedirectResult("/user/login");
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}