using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using System.Data.SqlClient;

namespace DAL
{
    public class MesaDAL
    {

        private Mesa MapearMesa(SqlDataReader reader)
        {
            return new Mesa
            {
                ID_Mesa = Convert.ToInt32(reader["ID_Mesa"]),
                NumeroMesa = Convert.ToInt32(reader["NumeroMesa"]),
                Capacidad = Convert.ToInt32(reader["Capacidad"]),
                Estado = reader["Estado"].ToString()
            };
        }

        public List<Mesa> ObtenerTodas()
        {
            List<Mesa> lista = new List<Mesa>();

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM MESA";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    lista.Add(MapearMesa(reader));
            }

            return lista;
        }

        public Mesa ObtenerPorID(int idMesa)
        {
            Mesa mesa = null;

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM MESA WHERE ID_Mesa = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", idMesa);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                    mesa = MapearMesa(reader);
            }

            return mesa;
        }

        public void CambiarEstado(int idMesa, string nuevoEstado)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "UPDATE MESA SET Estado = @Estado WHERE ID_Mesa = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
                cmd.Parameters.AddWithValue("@ID", idMesa);
                cmd.ExecuteNonQuery();
            }
        }
    }
}