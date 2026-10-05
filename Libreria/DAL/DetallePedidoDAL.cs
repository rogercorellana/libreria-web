using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;


namespace DAL
{
    public class DetallePedidoDAL
    {
        private string CalcularDVH(DetallePedido detalle)
        {
            return DigitoVerificador.CalcularDVH(
                detalle.ID_Pedido,
                detalle.ID_Plato,
                detalle.Cantidad,
                detalle.PrecioUnitario,
                detalle.Subtotal
            );
        }

        public List<DetallePedido> ObtenerTodos()
        {
            List<DetallePedido> lista = new List<DetallePedido>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM DETALLE_PEDIDO";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new DetallePedido
                    {
                        ID_Detalle = Convert.ToInt32(reader["ID_Detalle"]),
                        ID_Pedido = Convert.ToInt32(reader["ID_Pedido"]),
                        ID_Plato = Convert.ToInt32(reader["ID_Plato"]),
                        Cantidad = Convert.ToInt32(reader["Cantidad"]),
                        PrecioUnitario = Convert.ToDecimal(reader["PrecioUnitario"]),
                        Subtotal = Convert.ToDecimal(reader["Subtotal"]),
                        DVH = reader["DVH"].ToString()
                    });
                }
            }
            return lista;
        }

        public List<DetallePedido> ObtenerPorPedido(int idPedido)
        {
            List<DetallePedido> lista = new List<DetallePedido>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT DP.*, P.Nombre AS NombrePlato
                                 FROM DETALLE_PEDIDO DP
                                 INNER JOIN PLATO P ON DP.ID_Plato = P.ID_Plato
                                 WHERE DP.ID_Pedido = @IDPedido";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IDPedido", idPedido);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new DetallePedido
                    {
                        ID_Detalle = Convert.ToInt32(reader["ID_Detalle"]),
                        ID_Pedido = Convert.ToInt32(reader["ID_Pedido"]),
                        ID_Plato = Convert.ToInt32(reader["ID_Plato"]),
                        NombrePlato = reader["NombrePlato"].ToString(),
                        Cantidad = Convert.ToInt32(reader["Cantidad"]),
                        PrecioUnitario = Convert.ToDecimal(reader["PrecioUnitario"]),
                        Subtotal = Convert.ToDecimal(reader["Subtotal"]),
                        DVH = reader["DVH"].ToString()
                    });
                }
            }
            return lista;
        }

        public bool Agregar(DetallePedido detalle)
        {
            try
            {
                string dvh = CalcularDVH(detalle);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO DETALLE_PEDIDO
                        (ID_Pedido, ID_Plato, Cantidad, PrecioUnitario, Subtotal, DVH)
                        VALUES
                        (@Pedido, @Plato, @Cantidad, @Precio, @Subtotal, @DVH)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Pedido", detalle.ID_Pedido);
                    cmd.Parameters.AddWithValue("@Plato", detalle.ID_Plato);
                    cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    cmd.Parameters.AddWithValue("@Precio", detalle.PrecioUnitario);
                    cmd.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public bool Eliminar(int idDetalle)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = "DELETE FROM DETALLE_PEDIDO WHERE ID_Detalle = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ID", idDetalle);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public void RecalcularTodosDVH()
        {
            List<DetallePedido> lista = ObtenerTodos();
            foreach (var d in lista)
            {
                string dvh = CalcularDVH(d);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = "UPDATE DETALLE_PEDIDO SET DVH = @DVH WHERE ID_Detalle = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", d.ID_Detalle);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
