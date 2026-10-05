using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DAL;

namespace BLL
{
    public class ReporteBLL
    {
        private ReporteDAL reporteDAL = new ReporteDAL();

        public decimal ObtenerTotalVentas(DateTime desde, DateTime hasta)
        {
            return reporteDAL.ObtenerTotalVentas(desde, hasta);
        }

        public decimal ObtenerTotalPorMetodo(DateTime desde, DateTime hasta, string metodo)
        {
            return reporteDAL.ObtenerTotalPorMetodo(desde, hasta, metodo);
        }

        public DataTable ObtenerPlatosVendidos(DateTime desde, DateTime hasta)
        {
            return reporteDAL.ObtenerPlatosVendidos(desde, hasta);
        }
    }
}