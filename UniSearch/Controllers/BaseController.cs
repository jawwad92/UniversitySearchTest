using Portal.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        UserSessionData _user;


        public UserSessionData CurrentUser
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["UserProfile"] != null)
                {
                    _user = (UserSessionData)System.Web.HttpContext.Current.Session["UserProfile"];
                }
                return _user;
            }
            set
            {
                _user = value;
            }
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var rd = System.Web.HttpContext.Current.Request.RequestContext.RouteData;
            string currentController = rd.GetRequiredString("controller");
            string currentAction = rd.GetRequiredString("action");

            switch (currentAction)
            {

                case "Login":
                    break;
                case "LoginUser":
                    break;
                case "Logout":
                    break;
                case "Dispose":
                    break;

                default:
                    if (System.Web.HttpContext.Current.Session["UserProfile"] == null)
                    {
                        filterContext.Result = new RedirectResult("~/Home/Login");
                        base.OnActionExecuting(filterContext);
                    }
                    break;
            }

          
        }

    }
}