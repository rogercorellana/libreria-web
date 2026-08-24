using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Compra
    {
        public int IdCompra { get; set; }
        public int IdEditorial { get; set; }
        public decimal Monto { get; set; }
        public string FechaRegistro { get; set; }
        public string Usuario { get; set; }
    }
}
