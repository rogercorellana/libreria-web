using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BE
{
    public class MovimientoStock
    {
        public int ID_Movimiento { get; set; }
        public int ID_Ingrediente { get; set; }
        public int ID_Proveedor { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime FechaHora { get; set; }
        public int ID_Usuario { get; set; }
        public string DVH { get; set; }
    }
}
