using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplicacaoMedicina.Controllers
{
    public class CustomAuthorization : AuthorizeAttribute
    {
        public string LoginPage { get; set; }
        public string Role { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            LoginPage += "?ReturnUrl=" + filterContext.HttpContext.Request.RawUrl;

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.HttpContext.Response.Redirect(LoginPage);
            } else {
                if (Role != null)  {
                    if (!filterContext.HttpContext.User.IsInRole(Role))
                        filterContext.HttpContext.Response.Redirect(LoginPage);
                }
            }

            base.OnAuthorization(filterContext); 
        }
    }
}