using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Pago
    {
        public int ID_Pago { get; set; }
        public int ID_Pedido { get; set; }
        public string MetodoPago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaHora { get; set; }
        public int ID_Usuario { get; set; }
        public string DVH { get; set; }
    }
}