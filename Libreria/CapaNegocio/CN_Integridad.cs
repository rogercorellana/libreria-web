using System.Collections.Generic;
using System.Data;
using CapaDatos;

namespace CapaNegocio
{
    public class CN_Integridad
    {
        private CD_Integridad objCD_Integridad =
            new CD_Integridad();

        public bool EsTablaProtegida(string tabla)
        {
            return objCD_Integridad.EsTablaProtegida(tabla);
        }

        public List<string> ObtenerTablasProtegidas()
        {
            return objCD_Integridad.ObtenerTablasProtegidas();
        }

        public List<string> ObtenerClavePrimaria(
            string tabla)
        {
            return objCD_Integridad.ObtenerClavePrimaria(
                tabla
            );
        }

        public DataTable ObtenerDatos(
            string tabla,
            List<string> clavesPrimarias)
        {
            return objCD_Integridad.ObtenerDatos(
                tabla,
                clavesPrimarias
            );
        }

        public void GuardarVerificadoresTabla(
            string tabla,
            Dictionary<string, string> dvhs,
            string dvv,
            int cantidadRegistros)
        {
            objCD_Integridad.GuardarVerificadoresTabla(
                tabla,
                dvhs,
                dvv,
                cantidadRegistros
            );
        }

        public DataTable ObtenerDVHsRegistrados(
            string tabla)
        {
            return objCD_Integridad.ObtenerDVHsRegistrados(
                tabla
            );
        }

        public string ObtenerDVVRegistrado(
            string tabla)
        {
            return objCD_Integridad.ObtenerDVVRegistrado(
                tabla
            );
        }
    }
}
