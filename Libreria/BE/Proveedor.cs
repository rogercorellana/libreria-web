using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Proveedor
    {
        public int ID_Proveedor { get; set; }
        public string Nombre { get; set; }
        public string CUIT { get; set; } // Cifrado AES
        public string Telefono { get; set; } // Cifrado AES
        public string Email { get; set; } // Cifrado AES
    }
}