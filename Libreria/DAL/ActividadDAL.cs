using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class ActividadDAL
    {
        public List<Actividad> ObtenerTodas()
        {
            List<Actividad> lista = new List<Actividad>();

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM ACTIVIDAD ORDER BY TipoActividad";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Actividad
                    {
                        ID_Actividad = Convert.ToInt32(reader["ID_Actividad"]),
                        TipoActividad = reader["TipoActividad"].ToString()
                    });
                }
            }

            return lista;
        }
    }
}