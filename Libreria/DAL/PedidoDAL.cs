using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;


namespace DAL
{
    public class PedidoDAL
    {
        private string CalcularDVH(Pedido pedido)
        {
            return DigitoVerificador.CalcularDVH(
                pedido.ID_Mesa,
                pedido.ID_Usuario,
                pedido.Estado,
                pedido.Total
            );
        }

        private Pedido MapearPedido(SqlDataReader reader)
        {
            return new Pedido
            {
                ID_Pedido = Convert.ToInt32(reader["ID_Pedido"]),
                ID_Mesa = Convert.ToInt32(reader["ID_Mesa"]),
                ID_Usuario = Convert.ToInt32(reader["ID_Usuario"]),
                FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                Estado = reader["Estado"].ToString(),
                Total = Convert.ToDecimal(reader["Total"]),
                DVH = reader["DVH"].ToString()
            };
        }

        public List<Pedido> ObtenerTodos()
        {
            List<Pedido> lista = new List<Pedido>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM PEDIDO";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    lista.Add(MapearPedido(reader));
            }
            return lista;
        }

        public Pedido ObtenerPorMesa(int idMesa)
        {
            Pedido pedido = null;
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT * FROM PEDIDO 
                                 WHERE ID_Mesa = @IDMesa 
                                 AND Estado IN ('EnCocina', 'Entregado')
                                 AND ID_Pedido NOT IN (SELECT ID_Pedido FROM PAGO)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IDMesa", idMesa);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    pedido = MapearPedido(reader);
            }
            return pedido;
        }

        public Pedido ObtenerPorID(int idPedido)
        {
            Pedido pedido = null;
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM PEDIDO WHERE ID_Pedido = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", idPedido);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    pedido = MapearPedido(reader);
            }
            return pedido;
        }

        public int Agregar(Pedido pedido)
        {
            string dvh = CalcularDVH(pedido);
            int idPedido = 0;
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"INSERT INTO PEDIDO
                    (ID_Mesa, ID_Usuario, FechaHora, Estado, Total, DVH)
                    VALUES
                    (@Mesa, @Usuario, @FechaHora, @Estado, @Total, @DVH);
                    SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Mesa", pedido.ID_Mesa);
                cmd.Parameters.AddWithValue("@Usuario", pedido.ID_Usuario);
                cmd.Parameters.AddWithValue("@FechaHora", pedido.FechaHora);
                cmd.Parameters.AddWithValue("@Estado", pedido.Estado);
                cmd.Parameters.AddWithValue("@Total", pedido.Total);
                cmd.Parameters.AddWithValue("@DVH", dvh);
                idPedido = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return idPedido;
        }

        public bool ActualizarEstado(int idPedido, string nuevoEstado)
        {
            try
            {
                Pedido pedido = ObtenerPorID(idPedido);
                pedido.Estado = nuevoEstado;
                string dvh = CalcularDVH(pedido);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"UPDATE PEDIDO SET 
                        Estado = @Estado,
                        DVH    = @DVH
                        WHERE ID_Pedido = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", idPedido);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public bool ActualizarTotal(int idPedido, decimal total)
        {
            try
            {
                Pedido pedido = ObtenerPorID(idPedido);
                pedido.Total = total;
                string dvh = CalcularDVH(pedido);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"UPDATE PEDIDO SET 
                        Total = @Total,
                        DVH   = @DVH
                        WHERE ID_Pedido = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", idPedido);
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
            List<Pedido> lista = ObtenerTodos();
            foreach (var p in lista)
            {
                string dvh = CalcularDVH(p);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = "UPDATE PEDIDO SET DVH = @DVH WHERE ID_Pedido = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", p.ID_Pedido);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}