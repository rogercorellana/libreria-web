using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CapaDatos
{
    public class CD_Restore
    {
        public List<string> ListarBackups(out string mensaje)
        {
            List<string> lista = new List<string>();
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string consultaRuta = @"
                        SELECT CAST(
                            SERVERPROPERTY('InstanceDefaultBackupPath')
                            AS nvarchar(4000)
                        );";

                    string carpetaBackup;

                    using (SqlCommand cmdRuta =
                           new SqlCommand(consultaRuta, conexion))
                    {
                        object resultado = cmdRuta.ExecuteScalar();

                        carpetaBackup = resultado == null
                            ? string.Empty
                            : resultado.ToString();
                    }

                    if (string.IsNullOrWhiteSpace(carpetaBackup))
                    {
                        mensaje =
                            "No se pudo determinar la carpeta de backup de SQL Server.";
                        return lista;
                    }

                    if (!Directory.Exists(carpetaBackup))
                    {
                        mensaje =
                            "La carpeta de backups no existe.";
                        return lista;
                    }

                    string[] archivos =
                        Directory.GetFiles(
                            carpetaBackup,
                            "LIBRERIA_*.bak"
                        );

                    foreach (string archivo in archivos)
                    {
                        lista.Add(Path.GetFileName(archivo));
                    }

                    lista.Sort();
                    lista.Reverse();
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }

            return lista;
        }

        public bool RestaurarBackup(
            string nombreArchivo,
            out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                // Primero obtenemos la carpeta de backup.
                string carpetaBackup;

                using (SqlConnection conexion =
                       new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    string consultaRuta = @"
                        SELECT CAST(
                            SERVERPROPERTY('InstanceDefaultBackupPath')
                            AS nvarchar(4000)
                        );";

                    using (SqlCommand cmdRuta =
                           new SqlCommand(consultaRuta, conexion))
                    {
                        object resultado = cmdRuta.ExecuteScalar();

                        carpetaBackup = resultado == null
                            ? string.Empty
                            : resultado.ToString();
                    }
                }

                if (string.IsNullOrWhiteSpace(carpetaBackup))
                {
                    mensaje =
                        "No se pudo determinar la carpeta de backup.";
                    return false;
                }

                // Evitamos aceptar una ruta arbitraria.
                string nombreSeguro =
                    Path.GetFileName(nombreArchivo);

                if (!nombreSeguro.EndsWith(
                    ".bak",
                    StringComparison.OrdinalIgnoreCase))
                {
                    mensaje =
                        "El archivo seleccionado no es un backup válido.";
                    return false;
                }

                string rutaBackup =
                    Path.Combine(
                        carpetaBackup,
                        nombreSeguro
                    );

                if (!File.Exists(rutaBackup))
                {
                    mensaje =
                        "El archivo de backup seleccionado no existe.";
                    return false;
                }

                // Conectamos a MASTER, no a LIBRERIA.
                SqlConnectionStringBuilder builder =
                    new SqlConnectionStringBuilder(Conexion.cn);

                builder.InitialCatalog = "master";

                using (SqlConnection conexionMaster =
                       new SqlConnection(builder.ConnectionString))
                {
                    conexionMaster.Open();

                    string consultaRestore = @"
                        ALTER DATABASE [LIBRERIA]
                        SET SINGLE_USER
                        WITH ROLLBACK IMMEDIATE;

                        RESTORE DATABASE [LIBRERIA]
                        FROM DISK = @rutaBackup
                        WITH REPLACE,
                             RECOVERY,
                             STATS = 10;

                        ALTER DATABASE [LIBRERIA]
                        SET MULTI_USER;
                    ";

                    using (SqlCommand cmdRestore =
                           new SqlCommand(
                               consultaRestore,
                               conexionMaster))
                    {
                        cmdRestore.CommandType =
                            CommandType.Text;

                        cmdRestore.Parameters.Add(
                            "@rutaBackup",
                            SqlDbType.NVarChar,
                            4000
                        ).Value = rutaBackup;

                        cmdRestore.CommandTimeout = 300;

                        cmdRestore.ExecuteNonQuery();
                    }
                }

                mensaje =
                    "La base de datos fue restaurada correctamente.";

                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;

                // Intentamos devolver la base a MULTI_USER
                // en caso de que haya quedado en SINGLE_USER.
                try
                {
                    SqlConnectionStringBuilder builder =
                        new SqlConnectionStringBuilder(Conexion.cn);

                    builder.InitialCatalog = "master";

                    using (SqlConnection conexionMaster =
                           new SqlConnection(builder.ConnectionString))
                    {
                        conexionMaster.Open();

                        using (SqlCommand cmd =
                               new SqlCommand(
                                   "ALTER DATABASE [LIBRERIA] SET MULTI_USER;",
                                   conexionMaster))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    // No sobrescribimos el mensaje original.
                }

                return false;
            }
        }
    }
}
