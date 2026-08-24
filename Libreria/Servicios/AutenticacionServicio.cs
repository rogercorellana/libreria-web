using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;


namespace Servicios
{
    public static class AutenticacionServicio
    {
        private static FormsAuthenticationTicket ObtenerTicket()
        {
            if (HttpContext.Current == null)
                return null;

            HttpCookie cookie = HttpContext.Current.Request.Cookies[
                FormsAuthentication.FormsCookieName
            ];

            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                return null;

            try
            {
                return FormsAuthentication.Decrypt(cookie.Value);
            }
            catch
            {
                return null;
            }
        }

        private static string[] ObtenerDatosTicket()
        {
            FormsAuthenticationTicket ticket = ObtenerTicket();

            if (ticket == null || string.IsNullOrEmpty(ticket.UserData))
                return new string[0];

            return ticket.UserData.Split('|');
        }

        public static int ObtenerIdUsuario()
        {
            string[] datos = ObtenerDatosTicket();

            if (datos.Length < 2)
                return 0;

            int idUsuario;

            return int.TryParse(datos[0], out idUsuario)
                ? idUsuario
                : 0;
        }

        public static int ObtenerIdRol()
        {
            string[] datos = ObtenerDatosTicket();

            if (datos.Length < 2)
                return 0;

            int idRol;

            return int.TryParse(datos[1], out idRol)
                ? idRol
                : 0;
        }

        public static string ObtenerCorreo()
        {
            if (HttpContext.Current == null ||
                HttpContext.Current.User == null ||
                HttpContext.Current.User.Identity == null)
            {
                return string.Empty;
            }

            return HttpContext.Current.User.Identity.Name;
        }

        public static string ObtenerDescripcionRol()
        {
            int idRol = ObtenerIdRol();

            switch (idRol)
            {
                case 1:
                    return "Cliente";

                case 2:
                    return "Administrador";

                case 3:
                    return "Webmaster";

                default:
                    return string.Empty;
            }
        }
    }
}

