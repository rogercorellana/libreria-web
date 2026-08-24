using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;

namespace CapaNegocio
{
    public class CN_Restore
    {
        private CD_Restore objCD_Restore =
            new CD_Restore();

        public List<string> ListarBackups(
            out string mensaje)
        {
            return objCD_Restore.ListarBackups(
                out mensaje
            );
        }

        public bool RestaurarBackup(
            string nombreArchivo,
            out string mensaje)
        {
            return objCD_Restore.RestaurarBackup(
                nombreArchivo,
                out mensaje
            );
        }
    }
}
