using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class PlatoIngrediente
    {
        public int ID_PlatoIngrediente { get; set; }
        public int ID_Plato { get; set; }
        public int ID_Ingrediente { get; set; }
        public string NombreIngrediente { get; set; }
        public string UnidadMedida { get; set; }
        public decimal CantidadUsada { get; set; }
    }
}
