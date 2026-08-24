using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaPresentacion.Seguridad;

namespace CapaPresentacion.Filters
{
    public class PermisoAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _permiso;

        public PermisoAuthorizeAttribute(string permiso)
        {
            _permiso = permiso;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
                return false;

            return AutorizacionServicio.TienePermiso(_permiso);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new
                        {
                            controller = "Acceso",
                            action = "Denegado"
                        }
                    )
                );
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }


    }
}