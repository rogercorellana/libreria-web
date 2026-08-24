using System;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Bitacora
    {
        public bool Registrar(Bitacora evento, out string mensaje)
        {
            bool resultado = false;
            mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion =
                       new SqlConnection(Conexion.cn))
                {
                    string consulta = @"
                        INSERT INTO dbo.Bitacora
                        (
                            id_usuario,
                            usuario,
                            id_rol,
                            operacion,
                            modulo,
                            resultado,
                            descripcion,
                            tabla_afectada,
                            registro_afectado
                        )
                        VALUES
                        (
                            @idUsuario,
                            @usuario,
                            @idRol,
                            @operacion,
                            @modulo,
                            @resultado,
                            @descripcion,
                            @tablaAfectada,
                            @registroAfectado
                        );";

                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = (object)evento.IdUsuario ?? DBNull.Value;
                        cmd.Parameters.Add("@usuario", SqlDbType.VarChar, 100).Value = (object)evento.Usuario ?? DBNull.Value;
                        cmd.Parameters.Add("@idRol", SqlDbType.Int).Value = (object)evento.IdRol ?? DBNull.Value;
                        cmd.Parameters.Add("@operacion", SqlDbType.VarChar, 50).Value = evento.Operacion;
                        cmd.Parameters.Add("@modulo", SqlDbType.VarChar, 50).Value = evento.Modulo;
                        cmd.Parameters.Add("@resultado", SqlDbType.VarChar, 20).Value = evento.Resultado;
                        cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 500).Value = (object)evento.Descripcion ?? DBNull.Value;
                        cmd.Parameters.Add("@tablaAfectada", SqlDbType.VarChar, 128).Value = (object)evento.TablaAfectada ?? DBNull.Value;
                        cmd.Parameters.Add("@registroAfectado", SqlDbType.VarChar, 100).Value = (object)evento.RegistroAfectado ?? DBNull.Value;
                        conexion.Open();
                        resultado = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            return resultado;
        }
    }
}
