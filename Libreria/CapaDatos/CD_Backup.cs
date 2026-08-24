using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CapaDatos
{
    public class CD_Backup
    {
        public bool RealizarBackup(out string rutaBackup, out string mensaje)
        {
            rutaBackup = string.Empty;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    conexion.Open();

                    // Obtener la carpeta de backup predeterminada de SQL Server.
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
                        return false;
                    }

                    string nombreArchivo =
                        "LIBRERIA_" +
                        DateTime.Now.ToString("yyyyMMdd_HHmmss") +
                        ".bak";

                    rutaBackup =
                        Path.Combine(carpetaBackup, nombreArchivo);

                    // Ejecutar el backup real de la base de datos.
                    string consultaBackup = @"
                        BACKUP DATABASE [LIBRERIA]
                        TO DISK = @ruta
                        WITH INIT, CHECKSUM, STATS = 10;";

                    using (SqlCommand cmdBackup =
                           new SqlCommand(consultaBackup, conexion))
                    {
                        cmdBackup.CommandType = CommandType.Text;

                        cmdBackup.Parameters.Add(
                            "@ruta",
                            SqlDbType.NVarChar,
                            4000
                        ).Value = rutaBackup;

                        cmdBackup.ExecuteNonQuery();
                    }

                    mensaje = "Backup realizado correctamente.";
                    return true;
                }
            }
            catch (Exception ex)
            {
                rutaBackup = string.Empty;
                mensaje = ex.Message;
                return false;
            }
        }
    }
}

