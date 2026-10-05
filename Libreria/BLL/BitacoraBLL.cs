using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class BitacoraBLL
    {
        private BitacoraDAL bitacoraDAL = new BitacoraDAL();

        // Nivel 1 = Informativo, Nivel 2 = Advertencia, Nivel 3 = Crítico
        public bool Registrar(int? idUsuario, string usernameIngresado,
                              int idActividad, string descripcion, int nivelCriticidad = 1)
        {
            Bitacora bitacora = new Bitacora
            {
                ID_Usuario = idUsuario,
                UsernameIngresado = usernameIngresado,
                ID_Actividad = idActividad,
                Descripcion = descripcion,
                FechaHora = DateTime.Now,
                NivelCriticidad = nivelCriticidad
            };
            return bitacoraDAL.Registrar(bitacora);
        }

        public List<Bitacora> ObtenerTodos()
        {
            return bitacoraDAL.ObtenerTodos();
        }

        public List<Bitacora> ObtenerFiltrado(int? idUsuario, DateTime? desde,
                                               DateTime? hasta, int? idActividad)
        {
            return bitacoraDAL.ObtenerFiltrado(idUsuario, desde, hasta, idActividad);
        }
    }
}
