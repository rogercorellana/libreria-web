using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Plato
    {
        public int ID_Plato { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public int ID_Categoria { get; set; }
        public string NombreCategoria { get; set; }
        public bool Disponible { get; set; }
        public string DVH { get; set; }
    }
}
