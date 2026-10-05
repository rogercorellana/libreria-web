using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;


namespace DAL
{
    public class PagoDAL
    {
        private string CalcularDVH(Pago pago)
        {
            return DigitoVerificador.CalcularDVH(
                pago.ID_Pedido,
                pago.MetodoPago,
                pago.Monto,
                pago.ID_Usuario
            );
        }

        public List<Pago> ObtenerTodos()
        {
            List<Pago> lista = new List<Pago>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM PAGO";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Pago
                    {
                        ID_Pago = Convert.ToInt32(reader["ID_Pago"]),
                        ID_Pedido = Convert.ToInt32(reader["ID_Pedido"]),
                        MetodoPago = reader["MetodoPago"].ToString(),
                        Monto = Convert.ToDecimal(reader["Monto"]),
                        FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                        ID_Usuario = Convert.ToInt32(reader["ID_Usuario"]),
                        DVH = reader["DVH"].ToString()
                    });
                }
            }
            return lista;
        }

        public bool Agregar(Pago pago)
        {
            try
            {
                string dvh = CalcularDVH(pago);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO PAGO
                        (ID_Pedido, MetodoPago, Monto, FechaHora, ID_Usuario, DVH)
                        VALUES
                        (@Pedido, @MetodoPago, @Monto, @FechaHora, @Usuario, @DVH)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Pedido", pago.ID_Pedido);
                    cmd.Parameters.AddWithValue("@MetodoPago", pago.MetodoPago);
                    cmd.Parameters.AddWithValue("@Monto", pago.Monto);
                    cmd.Parameters.AddWithValue("@FechaHora", pago.FechaHora);
                    cmd.Parameters.AddWithValue("@Usuario", pago.ID_Usuario);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public void RecalcularTodosDVH()
        {
            List<Pago> lista = ObtenerTodos();
            foreach (var p in lista)
            {
                string dvh = CalcularDVH(p);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = "UPDATE PAGO SET DVH = @DVH WHERE ID_Pago = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", p.ID_Pago);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}