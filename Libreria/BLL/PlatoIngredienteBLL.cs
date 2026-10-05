using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class PlatoIngredienteBLL
    {
        private PlatoIngredienteDAL platoIngredienteDAL = new PlatoIngredienteDAL();

        public List<PlatoIngrediente> ObtenerPorPlato(int idPlato)
        {
            return platoIngredienteDAL.ObtenerPorPlato(idPlato);
        }

        public bool Agregar(PlatoIngrediente platoIngrediente)
        {
            if (platoIngrediente.CantidadUsada <= 0)
                throw new Exception("La cantidad debe ser mayor a cero.");

            return platoIngredienteDAL.Agregar(platoIngrediente);
        }

        public bool Eliminar(int idPlatoIngrediente)
        {
            return platoIngredienteDAL.Eliminar(idPlatoIngrediente);
        }

        public void DescontarStock(int idPedido)
        {
            platoIngredienteDAL.DescontarStock(idPedido);
        }
    }
}