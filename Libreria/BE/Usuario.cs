using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Usuario
    {
        public int ID_Usuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public int IntentosFallidos { get; set; }
        public string DVH { get; set; }
    }
}
