using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Conexion
    {
        private static string stringConexion = ConfigurationManager.ConnectionStrings["RestauranteDB"].ConnectionString;

        public static SqlConnection ObtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(stringConexion);
            conexion.Open();
            return conexion;
        }

        // Necesario para el RESTORE — conecta a master en lugar de TPSPN
        public static string ObtenerConexionStringMaster()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(stringConexion);
            builder.InitialCatalog = "master";
            return builder.ConnectionString;
        }
    }
}
