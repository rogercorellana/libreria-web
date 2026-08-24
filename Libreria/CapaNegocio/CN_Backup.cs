using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;

namespace CapaNegocio
{
    public class CN_Backup
    {
        private CD_Backup objCD_Backup = new CD_Backup();

        public bool RealizarBackup(
            out string rutaBackup,
            out string mensaje)
        {
            return objCD_Backup.RealizarBackup(
                out rutaBackup,
                out mensaje
            );
        }
    }
}
