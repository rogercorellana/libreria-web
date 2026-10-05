using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;//VERIFICAR SI ES NECESARIO

namespace DAL
{
    public class DigitoVerificadorDAL
    {
        // ============================================
        // OBTENER DVV ALMACENADO
        // ============================================
        public string ObtenerDVV(string nombreTabla, string nombreColumna)
        {
            string dvv = null;

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT ValorAcumulado 
                                 FROM DIGITO_VERIFICADOR_VERTICAL 
                                 WHERE NombreTabla = @Tabla 
                                 AND NombreColumna = @Columna";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Tabla", nombreTabla);
                cmd.Parameters.AddWithValue("@Columna", nombreColumna);

                object resultado = cmd.ExecuteScalar();
                if (resultado != null)
                    dvv = resultado.ToString();
            }

            return dvv;
        }

        // ============================================
        // ACTUALIZAR DVV
        // ============================================
        public void ActualizarDVV(string nombreTabla, string nombreColumna, string nuevoValor)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"UPDATE DIGITO_VERIFICADOR_VERTICAL 
                                 SET ValorAcumulado = @Valor,
                                     FechaCalculo   = @Fecha
                                 WHERE NombreTabla  = @Tabla 
                                 AND NombreColumna  = @Columna";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Valor", nuevoValor);
                cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                cmd.Parameters.AddWithValue("@Tabla", nombreTabla);
                cmd.Parameters.AddWithValue("@Columna", nombreColumna);
                cmd.ExecuteNonQuery();
            }
        }

        // ============================================
        // OBTENER CONTEO DE FILAS GUARDADO
        // Devuelve -1 si no hay conteo almacenado
        // ============================================
        public int ObtenerConteoFilas(string nombreTabla)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT TOP 1 CantidadFilas 
                                 FROM DIGITO_VERIFICADOR_VERTICAL 
                                 WHERE NombreTabla = @Tabla
                                 AND CantidadFilas IS NOT NULL";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Tabla", nombreTabla);

                object resultado = cmd.ExecuteScalar();
                if (resultado != null && resultado != DBNull.Value)
                    return Convert.ToInt32(resultado);

                return -1;
            }
        }

        // ============================================
        // ACTUALIZAR CONTEO DE FILAS
        // Se llama en cada logout y recálculo
        // ============================================
        public void ActualizarConteoFilas(string nombreTabla, int cantidadFilas)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"UPDATE DIGITO_VERIFICADOR_VERTICAL 
                                 SET CantidadFilas = @Cantidad
                                 WHERE NombreTabla = @Tabla";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Cantidad", cantidadFilas);
                cmd.Parameters.AddWithValue("@Tabla", nombreTabla);
                cmd.ExecuteNonQuery();
            }
        }

        // ============================================
        // OBTENER VALORES DE COLUMNA PARA DVV
        // ============================================
        public decimal[] ObtenerValoresColumna(string nombreTabla, string nombreColumna)
        {
            List<decimal> valores = new List<decimal>();

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT " + nombreColumna + " FROM " + nombreTabla;
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        try
                        {
                            valores.Add(Convert.ToDecimal(reader[0]));
                        }
                        catch
                        {
                            // Columnas no numéricas (ej: Rol) → suma de ASCII
                            string strVal = reader[0].ToString();
                            decimal hashVal = 0;
                            foreach (char c in strVal)
                                hashVal += (decimal)c;
                            valores.Add(hashVal);
                        }
                    }
                }
            }

            return valores.ToArray();
        }
    }
}
