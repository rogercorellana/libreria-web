using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class PagoBLL
    {
        private PagoDAL pagoDAL = new PagoDAL();
        private PedidoDAL pedidoDAL = new PedidoDAL();
        private MesaDAL mesaDAL = new MesaDAL();
        private DigitoVerificadorBLL dvBLL = new DigitoVerificadorBLL();
        private PlatoIngredienteBLL platoIngredienteBLL = new PlatoIngredienteBLL();

        public bool RegistrarPago(int idMesa, int idUsuario, string metodoPago)
        {
            // 1. Obtener el pedido activo de la mesa
            Pedido pedido = pedidoDAL.ObtenerPorMesa(idMesa);

            if (pedido == null)
                throw new Exception("No existe un pedido activo para esta mesa.");

            // 2. Crear el pago
            Pago pago = new Pago
            {
                ID_Pedido = pedido.ID_Pedido,
                MetodoPago = metodoPago,
                Monto = pedido.Total,
                FechaHora = DateTime.Now,
                ID_Usuario = idUsuario
            };

            // 3. Registrar el pago
            bool resultado = pagoDAL.Agregar(pago);

            if (resultado)
            {
                // 4. Cambiar estado del pedido a Entregado
                pedidoDAL.ActualizarEstado(pedido.ID_Pedido, "Entregado");

                // 5. Liberar la mesa
                mesaDAL.CambiarEstado(idMesa, "Libre");

                // Descontar ingredientes del stock
                platoIngredienteBLL.DescontarStock(pedido.ID_Pedido);

                // 6. Recalcular DVV
                dvBLL.RecalcularDVV("PAGO", "Monto");
            }

            return resultado;
        }
    }
}