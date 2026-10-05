using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BE
{
    public class Pedido
    {
        public int ID_Pedido { get; set; }
        public int ID_Mesa { get; set; }
        public int ID_Usuario { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }
        public string DVH { get; set; }
    }
}
