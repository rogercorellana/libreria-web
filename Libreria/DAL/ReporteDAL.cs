using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class ReporteDAL
    {
        public decimal ObtenerTotalVentas(DateTime desde, DateTime hasta)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT ISNULL(SUM(Monto), 0) 
                                 FROM PAGO 
                                 WHERE FechaHora >= @Desde 
                                 AND FechaHora <= @Hasta";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Desde", desde);
                cmd.Parameters.AddWithValue("@Hasta", hasta);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public decimal ObtenerTotalPorMetodo(DateTime desde, DateTime hasta, string metodo)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT ISNULL(SUM(Monto), 0) 
                                 FROM PAGO 
                                 WHERE FechaHora  >= @Desde 
                                 AND FechaHora    <= @Hasta
                                 AND MetodoPago    = @Metodo";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Desde", desde);
                cmd.Parameters.AddWithValue("@Hasta", hasta);
                cmd.Parameters.AddWithValue("@Metodo", metodo);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public DataTable ObtenerPlatosVendidos(DateTime desde, DateTime hasta)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT 
                                    PL.Nombre AS NombrePlato,
                                    SUM(DP.Cantidad) AS TotalUnidades,
                                    SUM(DP.Subtotal) AS TotalRecaudado
                                 FROM DETALLE_PEDIDO DP
                                 INNER JOIN PLATO PL ON DP.ID_Plato = PL.ID_Plato
                                 INNER JOIN PEDIDO P  ON DP.ID_Pedido = P.ID_Pedido
                                 INNER JOIN PAGO PA   ON P.ID_Pedido = PA.ID_Pedido
                                 WHERE PA.FechaHora >= @Desde
                                 AND PA.FechaHora   <= @Hasta
                                 GROUP BY PL.Nombre
                                 ORDER BY TotalUnidades DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Desde", desde);
                cmd.Parameters.AddWithValue("@Hasta", hasta);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}