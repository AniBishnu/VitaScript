using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vitascript.CustomAuthorization
{
	public class PatientAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var session = httpContext.Session;
            return session["UserId"] != null && session["UserType"]?.ToString() == "Patient";
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary(
                    new { controller = "Home", action = "Login" }
                )
            );
        }
    }
}