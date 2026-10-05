using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ProveedorDAL
    {
        private Proveedor MapearProveedor(SqlDataReader reader)
        {
            return new Proveedor
            {
                ID_Proveedor = Convert.ToInt32(reader["ID_Proveedor"]),
                Nombre = reader["Nombre"].ToString(),
                CUIT = reader["CUIT"].ToString(),
                Telefono = reader["Telefono"].ToString(),
                Email = reader["Email"].ToString()
            };
        }

        public List<Proveedor> ObtenerTodos()
        {
            List<Proveedor> lista = new List<Proveedor>();

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM PROVEEDOR";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    lista.Add(MapearProveedor(reader));
            }

            return lista;
        }

        public Proveedor ObtenerPorID(int idProveedor)
        {
            Proveedor proveedor = null;

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM PROVEEDOR WHERE ID_Proveedor = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", idProveedor);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                    proveedor = MapearProveedor(reader);
            }

            return proveedor;
        }

        public bool Agregar(Proveedor proveedor)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO PROVEEDOR
                        (Nombre, CUIT, Telefono, Email)
                        VALUES
                        (@Nombre, @CUIT, @Telefono, @Email)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Nombre", proveedor.Nombre);
                    cmd.Parameters.AddWithValue("@CUIT", proveedor.CUIT);
                    cmd.Parameters.AddWithValue("@Telefono", proveedor.Telefono ?? "");
                    cmd.Parameters.AddWithValue("@Email", proveedor.Email ?? "");
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch { return false; }
        }

        public bool Modificar(Proveedor proveedor)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"UPDATE PROVEEDOR SET
                        Nombre   = @Nombre,
                        CUIT     = @CUIT,
                        Telefono = @Telefono,
                        Email    = @Email
                        WHERE ID_Proveedor = @ID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Nombre", proveedor.Nombre);
                    cmd.Parameters.AddWithValue("@CUIT", proveedor.CUIT);
                    cmd.Parameters.AddWithValue("@Telefono", proveedor.Telefono ?? "");
                    cmd.Parameters.AddWithValue("@Email", proveedor.Email ?? "");
                    cmd.Parameters.AddWithValue("@ID", proveedor.ID_Proveedor);
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch { return false; }
        }
    }
}