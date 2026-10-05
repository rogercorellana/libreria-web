using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class IngredienteDAL
    {
        private Ingrediente MapearIngrediente(SqlDataReader reader)
        {
            return new Ingrediente
            {
                ID_Ingrediente = Convert.ToInt32(reader["ID_Ingrediente"]),
                Nombre = reader["Nombre"].ToString(),
                UnidadMedida = reader["UnidadMedida"].ToString(),
                StockActual = Convert.ToDecimal(reader["StockActual"]),
                StockMinimo = Convert.ToDecimal(reader["StockMinimo"]),
                PrecioCosto = reader["PrecioCosto"].ToString()
            };
        }

        public List<Ingrediente> ObtenerTodos()
        {
            List<Ingrediente> lista = new List<Ingrediente>();

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM INGREDIENTE";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    lista.Add(MapearIngrediente(reader));
            }

            return lista;
        }

        public Ingrediente ObtenerPorID(int idIngrediente)
        {
            Ingrediente ingrediente = null;

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM INGREDIENTE WHERE ID_Ingrediente = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", idIngrediente);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                    ingrediente = MapearIngrediente(reader);
            }

            return ingrediente;
        }

        public bool Agregar(Ingrediente ingrediente)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"INSERT INTO INGREDIENTE
            (Nombre, UnidadMedida, StockActual, StockMinimo, PrecioCosto)
            VALUES
            (@Nombre, @UnidadMedida, @StockActual, @StockMinimo, @PrecioCosto)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Nombre", ingrediente.Nombre);
                cmd.Parameters.AddWithValue("@UnidadMedida", ingrediente.UnidadMedida);
                cmd.Parameters.AddWithValue("@StockActual", ingrediente.StockActual);
                cmd.Parameters.AddWithValue("@StockMinimo", ingrediente.StockMinimo);
                cmd.Parameters.AddWithValue("@PrecioCosto", ingrediente.PrecioCosto);
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        public bool Modificar(Ingrediente ingrediente)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"UPDATE INGREDIENTE SET
                        Nombre       = @Nombre,
                        UnidadMedida = @UnidadMedida,
                        StockActual  = @StockActual,
                        StockMinimo  = @StockMinimo,
                        PrecioCosto  = @PrecioCosto
                        WHERE ID_Ingrediente = @ID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Nombre", ingrediente.Nombre);
                    cmd.Parameters.AddWithValue("@UnidadMedida", ingrediente.UnidadMedida);
                    cmd.Parameters.AddWithValue("@StockActual", ingrediente.StockActual);
                    cmd.Parameters.AddWithValue("@StockMinimo", ingrediente.StockMinimo);
                    cmd.Parameters.AddWithValue("@PrecioCosto", ingrediente.PrecioCosto);
                    cmd.Parameters.AddWithValue("@ID", ingrediente.ID_Ingrediente);
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch { return false; }
        }

        public bool ActualizarStock(int idIngrediente, decimal nuevoStock)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"UPDATE INGREDIENTE SET
                        StockActual = @Stock
                        WHERE ID_Ingrediente = @ID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Stock", nuevoStock);
                    cmd.Parameters.AddWithValue("@ID", idIngrediente);
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch { return false; }
        }

        public List<Ingrediente> ObtenerBajoStock()
        {
            List<Ingrediente> lista = new List<Ingrediente>();

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM INGREDIENTE WHERE StockActual <= StockMinimo";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    lista.Add(MapearIngrediente(reader));
            }

            return lista;
        }
    }
}
