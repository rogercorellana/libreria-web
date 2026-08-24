using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CapaDatos;

namespace Servicios
{
    public class IntegridadServicio
    {
        private readonly CD_Integridad objCD_Integridad =
            new CD_Integridad();

        public bool InicializarTabla(
            string tabla,
            out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                if (!objCD_Integridad.EsTablaProtegida(tabla))
                {
                    mensaje =
                        "La tabla indicada no forma parte del control de integridad.";

                    return false;
                }

                List<string> clavesPrimarias =
                    objCD_Integridad.ObtenerClavePrimaria(
                        tabla
                    );

                DataTable datos =
                    objCD_Integridad.ObtenerDatos(
                        tabla,
                        clavesPrimarias
                    );

                Dictionary<string, string> dvhs =
                   new Dictionary<string, string>();

                List<string> dvhsEnOrden =
                    new List<string>();

                foreach (DataRow fila in datos.Rows)
                {
                    string registroClave =
                        GenerarClaveRegistro(
                            fila,
                            clavesPrimarias
                        );

                    string dvh =
                        CalcularDVH(
                            fila,
                            datos.Columns
                        );

                    if (dvhs.ContainsKey(registroClave))
                    {
                        throw new InvalidOperationException(
                            "Se encontró una clave de registro duplicada en la tabla " +
                            tabla +
                            ": " +
                            registroClave
                        );
                    }

                    dvhs.Add(
                        registroClave,
                        dvh
                    );

                    dvhsEnOrden.Add(dvh);
                }

                string dvv =
                    CalcularDVV(dvhsEnOrden);

                objCD_Integridad.GuardarVerificadoresTabla(
                    tabla,
                    dvhs,
                    dvv,
                    datos.Rows.Count
                );

                mensaje =
                    "Verificadores generados correctamente para la tabla " +
                    tabla +
                    ".";

                return true;
            }
            catch (Exception ex)
            {
                mensaje =
                    "No se pudieron generar los verificadores de la tabla " +
                    tabla +
                    ". Detalle: " +
                    ex.Message;

                return false;
            }
        }
        
        //-----------------------------------------------------------------------------
        public bool InicializarTodasLasTablas(
            out string mensaje)
        {
            mensaje = string.Empty;

            try
            {
                List<string> tablas =
                    objCD_Integridad.ObtenerTablasProtegidas();

                foreach (string tabla in tablas)
                {
                    string mensajeTabla;

                    bool resultado =
                        InicializarTabla(
                            tabla,
                            out mensajeTabla
                        );

                    if (!resultado)
                    {
                        mensaje =
                            mensajeTabla;

                        return false;
                    }
                }

                mensaje =
                    "Los verificadores de todas las tablas fueron generados correctamente.";

                return true;
            }
            catch (Exception ex)
            {
                mensaje =
                    "No se pudieron generar los verificadores. Detalle: " +
                    ex.Message;

                return false;
            }
        }

        private string GenerarClaveRegistro(
            DataRow fila,
            List<string> clavesPrimarias)
        {
            List<string> valores =
                new List<string>();

            foreach (string columna in clavesPrimarias)
            {
                valores.Add(
                    NormalizarValor(
                        fila[columna]
                    )
                );
            }

            return string.Join(
                "|",
                valores
            );
        }

        private string CalcularDVH(
            DataRow fila,
            DataColumnCollection columnas)
        {
            List<string> valores =
                new List<string>();

            foreach (DataColumn columna in columnas)
            {
                valores.Add(
                    NormalizarValor(
                        fila[columna.ColumnName]
                    )
                );
            }

            string cadenaCanonica =
                string.Join(
                    "|",
                    valores
                );

            return CalcularSHA256(
                cadenaCanonica
            );
        }

        private string CalcularDVV(
            List<string> dvhsEnOrden)
        {
            string cadenaDVV =
                string.Join(
                    "|",
                    dvhsEnOrden
                );

            return CalcularSHA256(
                cadenaDVV
            );
        }

        private string NormalizarValor(
            object valor)
        {
            if (valor == null ||
                valor == DBNull.Value)
            {
                return "<NULL>";
            }

            if (valor is bool)
            {
                return (bool)valor
                    ? "1"
                    : "0";
            }

            if (valor is DateTime)
            {
                return ((DateTime)valor).ToString(
                    "yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture
                );
            }

            if (valor is DateTimeOffset)
            {
                return ((DateTimeOffset)valor).ToString(
                    "yyyy-MM-dd HH:mm:ss.fff zzz",
                    CultureInfo.InvariantCulture
                );
            }

            if (valor is byte[])
            {
                return Convert.ToBase64String(
                    (byte[])valor
                );
            }

            if (valor is IFormattable)
            {
                return ((IFormattable)valor).ToString(
                    null,
                    CultureInfo.InvariantCulture
                );
            }

            return valor.ToString();
        }

        private string CalcularSHA256(
            string texto)
        {
            using (SHA256 sha256 =
                   SHA256.Create())
            {
                byte[] bytes =
                    Encoding.UTF8.GetBytes(
                        texto
                    );

                byte[] hash =
                    sha256.ComputeHash(
                        bytes
                    );

                StringBuilder resultado =
                    new StringBuilder();

                foreach (byte b in hash)
                {
                    resultado.Append(
                        b.ToString(
                            "x2",
                            CultureInfo.InvariantCulture
                        )
                    );
                }

                return resultado.ToString();
            }
        }
    }
}

