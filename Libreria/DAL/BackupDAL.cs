using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace DAL
{
    public class BackupDAL
    {
        private string ObtenerNombreDB()
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                return con.Database;
            }
        }

        private string ObtenerRutaBackup()
        {
            string ruta = @"D:\Backups\";

            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }

            return ruta;
        }

        public bool GenerarBackup(out string rutaArchivo)
        {
            rutaArchivo = "";
            try
            {
                string db = ObtenerNombreDB();
                string carpeta = ObtenerRutaBackup();
                string archivo = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                rutaArchivo = Path.Combine(carpeta, archivo);

                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = $"BACKUP DATABASE [{db}] TO DISK = @Ruta WITH FORMAT, INIT";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Ruta", rutaArchivo);
                    cmd.CommandTimeout = 120;
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                rutaArchivo = "Error al generar backup: " + ex.Message;
                return false;
            }
        }

        public bool RestaurarUltimoBackup(out string mensaje)
        {
            mensaje = "";

            string db = "";
            string connStringMaster = "";

            try
            {
                string carpeta = ObtenerRutaBackup();

                string[] archivos = Directory.GetFiles(carpeta, "*.bak");

                if (archivos.Length == 0)
                {
                    mensaje = "No se encontró ningún backup en: " + carpeta;
                    return false;
                }

                string ultimoBackup = archivos[0];

                foreach (string archivo in archivos)
                {
                    if (File.GetLastWriteTime(archivo) >
                        File.GetLastWriteTime(ultimoBackup))
                    {
                        ultimoBackup = archivo;
                    }
                }

                db = ObtenerNombreDB();
                connStringMaster = Conexion.ObtenerConexionStringMaster();

                SqlConnection.ClearAllPools();

                using (SqlConnection con =
                    new SqlConnection(connStringMaster))
                {
                    con.Open();

                    string setSingleUser =
                        $"ALTER DATABASE [{db}] " +
                        "SET SINGLE_USER WITH ROLLBACK IMMEDIATE";

                    using (SqlCommand cmdSingle =
                        new SqlCommand(setSingleUser, con))
                    {
                        cmdSingle.CommandTimeout = 120;
                        cmdSingle.ExecuteNonQuery();
                    }

                    string restore =
                        $"RESTORE DATABASE [{db}] " +
                        "FROM DISK = @Ruta " +
                        "WITH REPLACE, RECOVERY";

                    using (SqlCommand cmdRestore =
                        new SqlCommand(restore, con))
                    {
                        cmdRestore.Parameters.AddWithValue(
                            "@Ruta",
                            ultimoBackup
                        );

                        cmdRestore.CommandTimeout = 300;
                        cmdRestore.ExecuteNonQuery();
                    }

                    string setMultiUser =
                        $"ALTER DATABASE [{db}] SET MULTI_USER";

                    using (SqlCommand cmdMulti =
                        new SqlCommand(setMultiUser, con))
                    {
                        cmdMulti.CommandTimeout = 120;
                        cmdMulti.ExecuteNonQuery();
                    }
                }

                SqlConnection.ClearAllPools();

                mensaje =
                    "Restauración completada desde: " +
                    Path.GetFileName(ultimoBackup);

                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    SqlConnection.ClearAllPools();

                    if (!string.IsNullOrEmpty(db) &&
                        !string.IsNullOrEmpty(connStringMaster))
                    {
                        using (SqlConnection con =
                            new SqlConnection(connStringMaster))
                        {
                            con.Open();

                            string setMultiUser =
                                $"ALTER DATABASE [{db}] SET MULTI_USER";

                            using (SqlCommand cmd =
                                new SqlCommand(setMultiUser, con))
                            {
                                cmd.CommandTimeout = 120;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch
                {
                    // Conservamos el error original.
                }

                mensaje = "Error al restaurar: " + ex.Message;
                return false;
            }
        }
    }
}