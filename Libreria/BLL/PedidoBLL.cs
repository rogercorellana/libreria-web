using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;


namespace BLL
{
    public class PedidoBLL
    {
        private PedidoDAL pedidoDAL = new PedidoDAL();
        private DetallePedidoDAL detalleDAL = new DetallePedidoDAL();
        private PlatoDAL platoDAL = new PlatoDAL();
        private DigitoVerificadorBLL dvBLL = new DigitoVerificadorBLL();
        private DigitoVerificadorDAL dvDAL = new DigitoVerificadorDAL();

        public Pedido ObtenerPorMesa(int idMesa)
        {
            return pedidoDAL.ObtenerPorMesa(idMesa);
        }

        public List<DetallePedido> ObtenerDetallePorMesa(int idMesa)
        {
            Pedido pedido = pedidoDAL.ObtenerPorMesa(idMesa);

            if (pedido == null)
                return new List<DetallePedido>();

            return detalleDAL.ObtenerPorPedido(pedido.ID_Pedido);
        }

        public decimal ObtenerTotalPorMesa(int idMesa)
        {
            Pedido pedido = pedidoDAL.ObtenerPorMesa(idMesa);

            if (pedido == null)
                return 0;

            return pedido.Total;
        }

        public bool EliminarDetalle(int idDetalle, int idMesa)
        {
            // Obtener el detalle para restar el subtotal del total del pedido
            Pedido pedido = pedidoDAL.ObtenerPorMesa(idMesa);
            if (pedido == null) return false;

            List<DetallePedido> detalles = detalleDAL.ObtenerPorPedido(pedido.ID_Pedido);
            DetallePedido detalle = detalles.Find(d => d.ID_Detalle == idDetalle);

            if (detalle == null) return false;

            bool resultado = detalleDAL.Eliminar(idDetalle);

            if (resultado)
            {
                decimal nuevoTotal = pedido.Total - detalle.Subtotal;
                if (nuevoTotal < 0) nuevoTotal = 0;
                pedidoDAL.ActualizarTotal(pedido.ID_Pedido, nuevoTotal);
                dvBLL.RecalcularDVV("DETALLE_PEDIDO", "Subtotal");
                dvBLL.RecalcularDVV("DETALLE_PEDIDO", "Cantidad");
                dvBLL.RecalcularDVV("PEDIDO", "Total");
                dvDAL.ActualizarConteoFilas("DETALLE_PEDIDO", detalleDAL.ObtenerTodos().Count);
            }

            return resultado;
        }
        public bool AgregarDetalle(int idMesa, int idUsuario, int idPlato, int cantidad)
        {
            // 1. Buscar pedido activo para esa mesa
            Pedido pedido = pedidoDAL.ObtenerPorMesa(idMesa);

            // 2. Si no existe, crear uno nuevo
            if (pedido == null)
            {
                pedido = new Pedido
                {
                    ID_Mesa = idMesa,
                    ID_Usuario = idUsuario,
                    FechaHora = DateTime.Now,
                    Estado = "EnCocina",
                    Total = 0
                };

                pedido.ID_Pedido = pedidoDAL.Agregar(pedido);
                dvBLL.RecalcularDVV("PEDIDO", "Total");
            }

            // 3. Obtener el plato
            Plato plato = platoDAL.ObtenerPorID(idPlato);

            if (plato == null)
                throw new Exception("El plato no existe.");

            if (!plato.Disponible)
                throw new Exception("El plato no está disponible.");

            // 4. Calcular subtotal
            decimal subtotal = plato.PrecioVenta * cantidad;

            // 5. Crear el detalle
            DetallePedido detalle = new DetallePedido
            {
                ID_Pedido = pedido.ID_Pedido,
                ID_Plato = idPlato,
                Cantidad = cantidad,
                PrecioUnitario = plato.PrecioVenta,
                Subtotal = subtotal
            };

            bool resultado = detalleDAL.Agregar(detalle);

            if (resultado)
            {
                // 6. Actualizar total del pedido
                decimal nuevoTotal = pedido.Total + subtotal;
                pedidoDAL.ActualizarTotal(pedido.ID_Pedido, nuevoTotal);

                // 7. Recalcular DVV
                dvBLL.RecalcularDVV("DETALLE_PEDIDO", "Subtotal");
                dvBLL.RecalcularDVV("DETALLE_PEDIDO", "Cantidad");
                dvBLL.RecalcularDVV("PEDIDO", "Total");
                dvDAL.ActualizarConteoFilas("DETALLE_PEDIDO", detalleDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("PEDIDO", pedidoDAL.ObtenerTodos().Count);
            }

            return resultado;
        }

        public bool CambiarEstado(int idPedido, string nuevoEstado)
        {
            return pedidoDAL.ActualizarEstado(idPedido, nuevoEstado);
        }
    }
}