using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class MesaBLL
    {
        private MesaDAL mesaDAL = new MesaDAL();

        public List<Mesa> ObtenerTodas()
        {
            return mesaDAL.ObtenerTodas();
        }

        public Mesa ObtenerPorID(int idMesa)
        {
            return mesaDAL.ObtenerPorID(idMesa);
        }

        public void AbrirMesa(int idMesa, int idUsuario)
        {
            Mesa mesa = mesaDAL.ObtenerPorID(idMesa);

            if (mesa == null)
                throw new Exception("La mesa no existe.");

            if (mesa.Estado != "Libre")
                throw new Exception("La mesa no está disponible para ser abierta.");

            mesaDAL.CambiarEstado(idMesa, "Ocupada");
        }

        public void SolicitarCuenta(int idMesa)
        {
            Mesa mesa = mesaDAL.ObtenerPorID(idMesa);

            if (mesa == null)
                throw new Exception("La mesa no existe.");

            if (mesa.Estado != "Ocupada")
                throw new Exception("La mesa no está en estado Ocupada.");

            mesaDAL.CambiarEstado(idMesa, "EsperandoCuenta");
        }

        public void LiberarMesa(int idMesa)
        {
            Mesa mesa = mesaDAL.ObtenerPorID(idMesa);

            if (mesa == null)
                throw new Exception("La mesa no existe.");

            mesaDAL.CambiarEstado(idMesa, "Libre");
        }
    }
}