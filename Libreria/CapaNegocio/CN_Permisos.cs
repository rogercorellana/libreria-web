using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Permisos
    {
        private CD_Permisos objCD_Permisos = new CD_Permisos();

        public List<Permiso> ListarPorRol(int idRol)
        {
            return objCD_Permisos.ListarPorRol(idRol);
        }
    }
}
