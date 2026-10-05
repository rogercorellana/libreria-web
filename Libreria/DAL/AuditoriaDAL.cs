using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;



namespace DAL
{
    public class AuditoriaDAL
    {
        // ============================================
        // GUARDAR VALOR ANTERIOR (cambios desde la app)
        // ============================================
        public void GuardarValorAnterior(string tabla, int idRegistro, string columna, string valorAnterior)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO AUDITORIA_DVH 
                        (NombreTabla, ID_Registro, NombreColumna, ValorAnterior, FechaRegistro, EsSnapshot)
                        VALUES (@Tabla, @ID, @Columna, @Valor, @Fecha, 0)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    cmd.Parameters.AddWithValue("@ID", idRegistro);
                    cmd.Parameters.AddWithValue("@Columna", columna);
                    cmd.Parameters.AddWithValue("@Valor", valorAnterior ?? "");
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        // ============================================
        // GUARDAR SNAPSHOT (foto del estado al logout)
        // ============================================
        public void GuardarSnapshot(string tabla, int idRegistro, string columna, string valor)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO AUDITORIA_DVH 
                        (NombreTabla, ID_Registro, NombreColumna, ValorAnterior, FechaRegistro, EsSnapshot)
                        VALUES (@Tabla, @ID, @Columna, @Valor, @Fecha, 1)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    cmd.Parameters.AddWithValue("@ID", idRegistro);
                    cmd.Parameters.AddWithValue("@Columna", columna);
                    cmd.Parameters.AddWithValue("@Valor", valor ?? "");
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        // ============================================
        // OBTENER ÚLTIMO SNAPSHOT DE UN CAMPO
        // ============================================
        public string ObtenerUltimoSnapshot(string tabla, int idRegistro, string columna)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"SELECT TOP 1 ValorAnterior 
                                     FROM AUDITORIA_DVH 
                                     WHERE NombreTabla = @Tabla 
                                     AND ID_Registro = @ID 
                                     AND NombreColumna = @Columna
                                     AND EsSnapshot = 1
                                     ORDER BY FechaRegistro DESC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    cmd.Parameters.AddWithValue("@ID", idRegistro);
                    cmd.Parameters.AddWithValue("@Columna", columna);
                    object resultado = cmd.ExecuteScalar();
                    return resultado?.ToString();
                }
            }
            catch { return null; }
        }

        // ============================================
        // OBTENER TODOS LOS SNAPSHOTS DE UN REGISTRO
        // ============================================
        public Dictionary<string, string> ObtenerUltimosSnapshots(string tabla, int idRegistro)
        {
            Dictionary<string, string> valores = new Dictionary<string, string>();
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"SELECT NombreColumna, ValorAnterior 
                                     FROM AUDITORIA_DVH A1
                                     WHERE NombreTabla = @Tabla 
                                     AND ID_Registro = @ID
                                     AND EsSnapshot = 1
                                     AND FechaRegistro = (
                                         SELECT MAX(FechaRegistro) 
                                         FROM AUDITORIA_DVH A2 
                                         WHERE A2.NombreTabla = A1.NombreTabla 
                                         AND A2.ID_Registro = A1.ID_Registro 
                                         AND A2.NombreColumna = A1.NombreColumna
                                         AND A2.EsSnapshot = 1
                                     )";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    cmd.Parameters.AddWithValue("@ID", idRegistro);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        valores[reader["NombreColumna"].ToString()] = reader["ValorAnterior"].ToString();
                }
            }
            catch { }
            return valores;
        }

        // ============================================
        // ELIMINAR SNAPSHOTS ANTERIORES DE UNA TABLA
        // Se llama antes de guardar los nuevos snapshots
        // ============================================
        public void EliminarSnapshots(string tabla)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"DELETE FROM AUDITORIA_DVH 
                                     WHERE NombreTabla = @Tabla 
                                     AND EsSnapshot = 1";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }
        // ============================================
        // OBTENER IDs GUARDADOS EN SNAPSHOT DE UNA TABLA
        // ============================================
        public List<int> ObtenerIDsEnSnapshot(string tabla)
        {
            List<int> ids = new List<int>();
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"SELECT DISTINCT ID_Registro 
                                     FROM AUDITORIA_DVH 
                                     WHERE NombreTabla = @Tabla 
                                     AND EsSnapshot = 1";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        ids.Add(Convert.ToInt32(reader["ID_Registro"]));
                }
            }
            catch { }
            return ids;
        }
        // ============================================
        // ELIMINAR SNAPSHOT DE UN REGISTRO ESPECÍFICO
        // ============================================
        public void EliminarSnapshotRegistro(string tabla, int idRegistro)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"DELETE FROM AUDITORIA_DVH 
                                     WHERE NombreTabla = @Tabla 
                                     AND ID_Registro = @ID
                                     AND EsSnapshot = 1";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    cmd.Parameters.AddWithValue("@ID", idRegistro);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

    }
}
