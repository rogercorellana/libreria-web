using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class IngredienteBLL
    {
        private IngredienteDAL ingredienteDAL = new IngredienteDAL();

        public List<Ingrediente> ObtenerTodos()
        {
            return ingredienteDAL.ObtenerTodos();
        }

        public Ingrediente ObtenerPorID(int idIngrediente)
        {
            return ingredienteDAL.ObtenerPorID(idIngrediente);
        }

        public List<Ingrediente> ObtenerBajoStock()
        {
            return ingredienteDAL.ObtenerBajoStock();
        }

        public bool Agregar(Ingrediente ingrediente)
        {
            if (string.IsNullOrEmpty(ingrediente.Nombre))
                throw new Exception("El nombre del ingrediente es obligatorio.");

            if (ingrediente.StockMinimo < 0)
                throw new Exception("El stock mínimo no puede ser negativo.");

            return ingredienteDAL.Agregar(ingrediente);
        }

        public bool Modificar(Ingrediente ingrediente)
        {
            if (string.IsNullOrEmpty(ingrediente.Nombre))
                throw new Exception("El nombre del ingrediente es obligatorio.");

            if (ingrediente.StockMinimo < 0)
                throw new Exception("El stock mínimo no puede ser negativo.");

            return ingredienteDAL.Modificar(ingrediente);
        }

        public bool ActualizarStock(int idIngrediente, decimal nuevoStock)
        {
            if (nuevoStock < 0)
                throw new Exception("El stock no puede ser negativo.");

            return ingredienteDAL.ActualizarStock(idIngrediente, nuevoStock);
        }
    }
}