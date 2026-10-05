using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class ActividadBLL
    {
        private ActividadDAL actividadDAL = new ActividadDAL();

        public List<Actividad> ObtenerTodas()
        {
            return actividadDAL.ObtenerTodas();
        }
    }
}