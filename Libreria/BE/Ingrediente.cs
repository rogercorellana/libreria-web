using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Ingrediente
    {
        public int ID_Ingrediente { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public string PrecioCosto { get; set; } // Cifrado AES
    }
}