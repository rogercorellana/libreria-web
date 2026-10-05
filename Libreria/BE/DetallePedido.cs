using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class DetallePedido
    {
        public int ID_Detalle { get; set; }
        public int ID_Pedido { get; set; }
        public int ID_Plato { get; set; }
        public string NombrePlato { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string DVH { get; set; }
    }
}