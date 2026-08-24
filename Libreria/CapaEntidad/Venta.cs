using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public string Usuario { get; set; }
        public decimal Monto { get; set; }
        public string FechaRegistro { get; set; }
    }
}
