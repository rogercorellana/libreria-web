using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class CategoriaDAL
    {
        public List<Categoria> ObtenerTodas()
        {
            List<Categoria> lista = new List<Categoria>();

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM CATEGORIA";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Categoria
                    {
                        ID_Categoria = Convert.ToInt32(reader["ID_Categoria"]),
                        Nombre = reader["Nombre"].ToString()
                    });
                }
            }

            return lista;
        }
    }
}