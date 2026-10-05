using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;


namespace DAL
{
    public class MovimientoStockDAL
    {
        private string CalcularDVH(MovimientoStock movimiento)
        {
            return DigitoVerificador.CalcularDVH(
                movimiento.ID_Ingrediente,
                movimiento.ID_Proveedor,
                movimiento.Cantidad,
                movimiento.ID_Usuario
            );
        }

        public List<MovimientoStock> ObtenerTodos()
        {
            List<MovimientoStock> lista = new List<MovimientoStock>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM MOVIMIENTO_STOCK";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new MovimientoStock
                    {
                        ID_Movimiento = Convert.ToInt32(reader["ID_Movimiento"]),
                        ID_Ingrediente = Convert.ToInt32(reader["ID_Ingrediente"]),
                        ID_Proveedor = Convert.ToInt32(reader["ID_Proveedor"]),
                        Cantidad = Convert.ToDecimal(reader["Cantidad"]),
                        FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                        ID_Usuario = Convert.ToInt32(reader["ID_Usuario"]),
                        DVH = reader["DVH"].ToString()
                    });
                }
            }
            return lista;
        }

        public bool Agregar(MovimientoStock movimiento)
        {
            try
            {
                string dvh = CalcularDVH(movimiento);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO MOVIMIENTO_STOCK
                        (ID_Ingrediente, ID_Proveedor, Cantidad, FechaHora, ID_Usuario, DVH)
                        VALUES
                        (@Ingrediente, @Proveedor, @Cantidad, @FechaHora, @Usuario, @DVH)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Ingrediente", movimiento.ID_Ingrediente);
                    cmd.Parameters.AddWithValue("@Proveedor", movimiento.ID_Proveedor);
                    cmd.Parameters.AddWithValue("@Cantidad", movimiento.Cantidad);
                    cmd.Parameters.AddWithValue("@FechaHora", movimiento.FechaHora);
                    cmd.Parameters.AddWithValue("@Usuario", movimiento.ID_Usuario);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public void RecalcularTodosDVH()
        {
            List<MovimientoStock> lista = ObtenerTodos();
            foreach (var m in lista)
            {
                string dvh = CalcularDVH(m);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = "UPDATE MOVIMIENTO_STOCK SET DVH = @DVH WHERE ID_Movimiento = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", m.ID_Movimiento);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}