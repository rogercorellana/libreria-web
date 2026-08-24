using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Bitacora
    {
        public int IdBitacora { get; set; }
        public System.DateTime FechaHora { get; set; }

        public int? IdUsuario { get; set; }
        public string Usuario { get; set; }
        public int? IdRol { get; set; }

        public string Operacion { get; set; }
        public string Modulo { get; set; }
        public string Resultado { get; set; }
        public string Descripcion { get; set; }

        public string TablaAfectada { get; set; }
        public string RegistroAfectado { get; set; }
    }

    /*Los int? son intencionales: en la tabla id_usuario e id_rol pueden ser NULL.*/
}
