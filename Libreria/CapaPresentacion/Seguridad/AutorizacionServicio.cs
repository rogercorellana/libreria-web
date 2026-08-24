using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapaNegocio;

namespace CapaPresentacion.Seguridad
{
    public static class AutorizacionServicio
    {
        private const string ClavePermisos = "PERMISOS_USUARIO";

        public static bool TienePermiso(string permiso)
        {
            if (string.IsNullOrWhiteSpace(permiso))
                return false;

            int idRol = Servicios.AutenticacionServicio.ObtenerIdRol();

            if (idRol == 0)
                return false;

            List<string> permisos = ObtenerPermisos();
            return permisos.Any(p =>
                string.Equals(p,permiso,StringComparison.OrdinalIgnoreCase)
            );
        }

        public static List<string> ObtenerPermisos()
        {
            if (HttpContext.Current == null)
                return new List<string>();

            List<string> permisos = HttpContext.Current.Items[ClavePermisos] as List<string>;

            if (permisos != null)
                return permisos;

            int idRol = Servicios.AutenticacionServicio.ObtenerIdRol();
            if (idRol == 0)
                return new List<string>();

            permisos = new CN_Permisos()
                .ListarPorRol(idRol)
                .Select(p => p.Menu)
                .ToList();

            HttpContext.Current.Items[ClavePermisos] = permisos;
            return permisos;
        }
    }
}