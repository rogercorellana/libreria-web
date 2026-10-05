using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class MovimientoStockBLL
    {
        private MovimientoStockDAL movimientoDAL = new MovimientoStockDAL();
        private IngredienteDAL ingredienteDAL = new IngredienteDAL();
        private DigitoVerificadorBLL dvBLL = new DigitoVerificadorBLL();

        public bool RegistrarIngreso(int idIngrediente, int idProveedor,
                                     decimal cantidad, int idUsuario)
        {
            // 1. Validar cantidad
            if (cantidad <= 0)
                throw new Exception("La cantidad debe ser mayor a cero.");

            // 2. Verificar que el ingrediente exista
            Ingrediente ingrediente = ingredienteDAL.ObtenerPorID(idIngrediente);

            if (ingrediente == null)
                throw new Exception("El ingrediente no existe.");

            // 3. Crear el movimiento
            MovimientoStock movimiento = new MovimientoStock
            {
                ID_Ingrediente = idIngrediente,
                ID_Proveedor = idProveedor,
                Cantidad = cantidad,
                FechaHora = DateTime.Now,
                ID_Usuario = idUsuario
            };

            // 4. Registrar el movimiento
            bool resultado = movimientoDAL.Agregar(movimiento);

            if (resultado)
            {
                // 5. Actualizar el stock del ingrediente
                decimal nuevoStock = ingrediente.StockActual + cantidad;
                ingredienteDAL.ActualizarStock(idIngrediente, nuevoStock);

                // 6. Recalcular DVV
                dvBLL.RecalcularDVV("MOVIMIENTO_STOCK", "Cantidad");
            }

            return resultado;
        }
    }
}