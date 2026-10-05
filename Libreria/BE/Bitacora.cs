using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BE
{
    public class Bitacora
    {
        public int ID_Log { get; set; }
        public int? ID_Usuario { get; set; }
        public string UsernameIngresado { get; set; }
        public int ID_Actividad { get; set; }
        public string TipoActividad { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaHora { get; set; }
        public int NivelCriticidad { get; set; }
    }
}