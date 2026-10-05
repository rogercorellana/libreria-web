using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class BitacoraDAL
    {
        private Bitacora MapearBitacora(SqlDataReader reader)
        {
            return new Bitacora
            {
                ID_Log = Convert.ToInt32(reader["ID_Log"]),
                ID_Usuario = reader["ID_Usuario"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ID_Usuario"]),
                UsernameIngresado = reader["UsernameIngresado"].ToString(),
                ID_Actividad = Convert.ToInt32(reader["ID_Actividad"]),
                TipoActividad = reader["TipoActividad"].ToString(),
                Descripcion = reader["Descripcion"].ToString(),
                FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                NivelCriticidad = Convert.ToInt32(reader["NivelCriticidad"])
            };
        }

        public bool Registrar(Bitacora bitacora)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO BITACORA
                        (ID_Usuario, UsernameIngresado, ID_Actividad, Descripcion, FechaHora, NivelCriticidad)
                        VALUES
                        (@Usuario, @Username, @Actividad, @Descripcion, @FechaHora, @Nivel)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Usuario", (object)bitacora.ID_Usuario ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Username", (object)bitacora.UsernameIngresado ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Actividad", bitacora.ID_Actividad);
                    cmd.Parameters.AddWithValue("@Descripcion", bitacora.Descripcion);
                    cmd.Parameters.AddWithValue("@FechaHora", bitacora.FechaHora);
                    cmd.Parameters.AddWithValue("@Nivel", bitacora.NivelCriticidad);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public List<Bitacora> ObtenerTodos()
        {
            List<Bitacora> lista = new List<Bitacora>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT B.*, A.TipoActividad 
                                 FROM BITACORA B
                                 INNER JOIN ACTIVIDAD A ON B.ID_Actividad = A.ID_Actividad
                                 ORDER BY B.FechaHora DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    lista.Add(MapearBitacora(reader));
            }
            return lista;
        }

        public List<Bitacora> ObtenerFiltrado(int? idUsuario, DateTime? desde, DateTime? hasta, int? idActividad)
        {
            List<Bitacora> lista = new List<Bitacora>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT B.*, A.TipoActividad 
                                 FROM BITACORA B
                                 INNER JOIN ACTIVIDAD A ON B.ID_Actividad = A.ID_Actividad
                                 WHERE 1=1";
                if (idUsuario.HasValue) query += " AND B.ID_Usuario = @Usuario";
                if (desde.HasValue) query += " AND B.FechaHora >= @Desde";
                if (hasta.HasValue) query += " AND B.FechaHora <= @Hasta";
                if (idActividad.HasValue) query += " AND B.ID_Actividad = @Actividad";
                query += " ORDER BY B.FechaHora DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                if (idUsuario.HasValue) cmd.Parameters.AddWithValue("@Usuario", idUsuario.Value);
                if (desde.HasValue) cmd.Parameters.AddWithValue("@Desde", desde.Value);
                if (hasta.HasValue) cmd.Parameters.AddWithValue("@Hasta", hasta.Value);
                if (idActividad.HasValue) cmd.Parameters.AddWithValue("@Actividad", idActividad.Value);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    lista.Add(MapearBitacora(reader));
            }
            return lista;
        }
    }
}