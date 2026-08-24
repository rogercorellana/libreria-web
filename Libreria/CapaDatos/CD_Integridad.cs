using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Integridad
    {
        // Tablas que forman parte del control de integridad.
        private static readonly HashSet<string> TablasProtegidas =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Bitacora",
                "Cliente",
                "Compra",
                "DetalleCompra",
                "Editorial",
                "Factura",
                "Libro",
                "Permiso",
                "Rol",
                "Usuario",
                "Venta"
            };

        public bool EsTablaProtegida(string tabla)
        {
            return !string.IsNullOrWhiteSpace(tabla)
                && TablasProtegidas.Contains(tabla);
        }

        public List<string> ObtenerTablasProtegidas()
        {
            return new List<string>(TablasProtegidas);
        }

        public List<string> ObtenerClavePrimaria(string tabla)
        {
            if (!EsTablaProtegida(tabla))
            {
                throw new ArgumentException(
                    "La tabla indicada no forma parte del control de integridad."
                );
            }

            List<string> claves = new List<string>();

            string objeto = "dbo." + tabla;

            string consulta = @"
                SELECT
                    c.name
                FROM sys.indexes i
                INNER JOIN sys.index_columns ic
                    ON i.object_id = ic.object_id
                    AND i.index_id = ic.index_id
                INNER JOIN sys.columns c
                    ON ic.object_id = c.object_id
                    AND ic.column_id = c.column_id
                WHERE
                    i.is_primary_key = 1
                    AND i.object_id = OBJECT_ID(@objeto)
                ORDER BY
                    ic.key_ordinal;";

            using (SqlConnection conexion =
                   new SqlConnection(Conexion.cn))
            {
                using (SqlCommand cmd =
                       new SqlCommand(consulta, conexion))
                {
                    cmd.Parameters.AddWithValue(
                        "@objeto",
                        objeto
                    );

                    conexion.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            claves.Add(dr["name"].ToString());
                        }
                    }
                }
            }

            if (claves.Count == 0)
            {
                throw new InvalidOperationException(
                    "La tabla '" + tabla +
                    "' no tiene una clave primaria definida."
                );
            }

            return claves;
        }

        public DataTable ObtenerDatos(
            string tabla,
            List<string> clavesPrimarias)
        {
            if (!EsTablaProtegida(tabla))
            {
                throw new ArgumentException(
                    "La tabla indicada no forma parte del control de integridad."
                );
            }

            if (clavesPrimarias == null ||
                clavesPrimarias.Count == 0)
            {
                throw new ArgumentException(
                    "La tabla no tiene claves primarias válidas."
                );
            }

            string columnasOrden =
                string.Join(
                    ", ",
                    clavesPrimarias.ConvertAll(
                        c => "[" + c + "]"
                    )
                );

            string consulta =
                "SELECT * " +
                "FROM dbo.[" + tabla + "] " +
                "ORDER BY " + columnasOrden + ";";

            DataTable tablaDatos =
                new DataTable();

            using (SqlConnection conexion =
                   new SqlConnection(Conexion.cn))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           consulta,
                           conexion))
                {
                    conexion.Open();

                    using (SqlDataAdapter da =
                           new SqlDataAdapter(cmd))
                    {
                        da.Fill(tablaDatos);
                    }
                }
            }

            return tablaDatos;
        }

        public void GuardarVerificadoresTabla(
            string tabla,
            Dictionary<string, string> dvhs,
            string dvv,
            int cantidadRegistros)
        {
            if (!EsTablaProtegida(tabla))
            {
                throw new ArgumentException(
                    "La tabla indicada no forma parte del control de integridad."
                );
            }

            using (SqlConnection conexion =
                   new SqlConnection(Conexion.cn))
            {
                conexion.Open();

                using (SqlTransaction transaccion =
                       conexion.BeginTransaction())
                {
                    try
                    {
                        // Eliminamos los verificadores anteriores
                        // de esta tabla.
                        string eliminarHorizontal = @"
                            DELETE FROM dbo.IntegridadHorizontal
                            WHERE tabla = @tabla;";

                        using (SqlCommand cmdDeleteH =
                               new SqlCommand(
                                   eliminarHorizontal,
                                   conexion,
                                   transaccion))
                        {
                            cmdDeleteH.Parameters.AddWithValue(
                                "@tabla",
                                tabla
                            );

                            cmdDeleteH.ExecuteNonQuery();
                        }

                        // Insertamos los nuevos DVH.
                        string insertarHorizontal = @"
                            INSERT INTO dbo.IntegridadHorizontal
                            (
                                tabla,
                                registro_clave,
                                dvh
                            )
                            VALUES
                            (
                                @tabla,
                                @registro_clave,
                                @dvh
                            );";

                        foreach (
                            KeyValuePair<string, string> item
                            in dvhs)
                        {
                            using (SqlCommand cmdInsertH =
                                   new SqlCommand(
                                       insertarHorizontal,
                                       conexion,
                                       transaccion))
                            {
                                cmdInsertH.Parameters.AddWithValue(
                                    "@tabla",
                                    tabla
                                );

                                cmdInsertH.Parameters.AddWithValue(
                                    "@registro_clave",
                                    item.Key
                                );

                                cmdInsertH.Parameters.AddWithValue(
                                    "@dvh",
                                    item.Value
                                );

                                cmdInsertH.ExecuteNonQuery();
                            }
                        }

                        // Eliminamos el DVV anterior.
                        string eliminarVertical = @"
                            DELETE FROM dbo.IntegridadVertical
                            WHERE tabla = @tabla;";

                        using (SqlCommand cmdDeleteV =
                               new SqlCommand(
                                   eliminarVertical,
                                   conexion,
                                   transaccion))
                        {
                            cmdDeleteV.Parameters.AddWithValue(
                                "@tabla",
                                tabla
                            );

                            cmdDeleteV.ExecuteNonQuery();
                        }

                        // Insertamos el nuevo DVV.
                        string insertarVertical = @"
                            INSERT INTO dbo.IntegridadVertical
                            (
                                tabla,
                                dvv,
                                cantidad_registros
                            )
                            VALUES
                            (
                                @tabla,
                                @dvv,
                                @cantidad_registros
                            );";

                        using (SqlCommand cmdInsertV =
                               new SqlCommand(
                                   insertarVertical,
                                   conexion,
                                   transaccion))
                        {
                            cmdInsertV.Parameters.AddWithValue(
                                "@tabla",
                                tabla
                            );

                            cmdInsertV.Parameters.AddWithValue(
                                "@dvv",
                                dvv
                            );

                            cmdInsertV.Parameters.AddWithValue(
                                "@cantidad_registros",
                                cantidadRegistros
                            );

                            cmdInsertV.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public DataTable ObtenerDVHsRegistrados(
            string tabla)
        {
            if (!EsTablaProtegida(tabla))
            {
                throw new ArgumentException(
                    "La tabla indicada no forma parte del control de integridad."
                );
            }

            DataTable datos =
                new DataTable();

            string consulta = @"
                SELECT
                    registro_clave,
                    dvh
                FROM dbo.IntegridadHorizontal
                WHERE tabla = @tabla
                ORDER BY registro_clave;";

            using (SqlConnection conexion =
                   new SqlConnection(Conexion.cn))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           consulta,
                           conexion))
                {
                    cmd.Parameters.AddWithValue(
                        "@tabla",
                        tabla
                    );

                    conexion.Open();

                    using (SqlDataAdapter da =
                           new SqlDataAdapter(cmd))
                    {
                        da.Fill(datos);
                    }
                }
            }

            return datos;
        }

        public string ObtenerDVVRegistrado(
            string tabla)
        {
            if (!EsTablaProtegida(tabla))
            {
                throw new ArgumentException(
                    "La tabla indicada no forma parte del control de integridad."
                );
            }

            string consulta = @"
                SELECT dvv
                FROM dbo.IntegridadVertical
                WHERE tabla = @tabla;";

            using (SqlConnection conexion =
                   new SqlConnection(Conexion.cn))
            {
                using (SqlCommand cmd =
                       new SqlCommand(
                           consulta,
                           conexion))
                {
                    cmd.Parameters.AddWithValue(
                        "@tabla",
                        tabla
                    );

                    conexion.Open();

                    object resultado =
                        cmd.ExecuteScalar();

                    if (resultado == null ||
                        resultado == DBNull.Value)
                    {
                        return null;
                    }

                    return resultado.ToString();
                }
            }
        }
    }
}

