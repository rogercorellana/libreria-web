using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Usuarios
    {
        /*METODO PARA LISTAR USUARIOS*/
        public List<Usuario> Listar()
        {
            List<Usuario> Lista = new List<Usuario>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.cn))
                {
                    string consulta = @"
                SELECT
                    id_usuario,
                    id_rol,
                    dni,
                    nombre_usuario,
                    apellido_usuario,
                    correo,
                    contrasena,
                    activo,
                    restablecer
                FROM dbo.Usuario";

                    using (SqlCommand cmd = new SqlCommand(consulta, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Lista.Add(
                                    new Usuario()
                                    {
                                        IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                        IdRol = Convert.ToInt32(dr["id_rol"]),
                                        Dni = Convert.ToInt32(dr["dni"]),
                                        NombreUsuario = dr["nombre_usuario"].ToString(),
                                        ApellidoUsuario = dr["apellido_usuario"].ToString(),
                                        Correo = dr["correo"].ToString(),
                                        Contraseña = dr["contrasena"].ToString(),
                                        Activo = Convert.ToBoolean(dr["activo"]),
                                        Reestablecer = Convert.ToBoolean(dr["restablecer"])
                                    }
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Lista = new List<Usuario>();
            }

            return Lista;
        }

        /*METODO PARA CAMBIAR CLAVE DE USUARIO*/
        public bool CambiarClave(int idUsuario, string nuevaClave, out string mensaje)
            {
                bool resultado = false;
                mensaje = string.Empty;

                try
                {
                    using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                    {
                        string consulta = @"
                        UPDATE dbo.Usuario
                        SET contrasena = @nuevaClave,
                            restablecer = 0
                        WHERE id_usuario = @idUsuario;";

                        using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                        {
                            cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;
                            cmd.Parameters.Add("@nuevaClave", SqlDbType.VarChar, 64).Value = nuevaClave;

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

           /*METODO PARA REESTABLECER CLAVE DE USUARIO*/
            public bool ReestablecerClave(int idUsuario, string clave, out string mensaje)
            {
                bool resultado = false;
                mensaje = string.Empty;

                try
                {
                    using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                    {
                        string consulta = @"
                        UPDATE dbo.Usuario
                        SET contrasena = @clave,
                            restablecer = 1
                        WHERE id_usuario = @idUsuario;";

                        using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                        {
                            cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;
                            cmd.Parameters.Add("@clave", SqlDbType.VarChar, 64).Value = clave;

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


        /* METODO PARA CREAR UN USUARIO NUEVO*/
        public int Registrar(Usuario usu, out string Mensaje)
        {
            int idGenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.cn))
                {
                    string consulta = @"
                DECLARE @nuevoId INT;

                SELECT @nuevoId = ISNULL(MAX(id_usuario), 0) + 1
                FROM dbo.Usuario;

                INSERT INTO dbo.Usuario
                (
                    id_usuario,
                    id_rol,
                    dni,
                    nombre_usuario,
                    apellido_usuario,
                    correo,
                    contrasena,
                    activo,
                    restablecer
                )
                VALUES
                (
                    @nuevoId,
                    @idRol,
                    @dni,
                    @nombreUsuario,
                    @apellidoUsuario,
                    @correo,
                    @contrasena,
                    @activo,
                    @restablecer
                );

                SELECT @nuevoId;";

                    using (SqlCommand cmd =
                           new SqlCommand(consulta, conn))
                    {
                        cmd.Parameters.Add("@idRol", SqlDbType.Int)
                            .Value = usu.IdRol;

                        cmd.Parameters.Add("@dni", SqlDbType.Int)
                            .Value = usu.Dni;

                        cmd.Parameters.Add("@nombreUsuario", SqlDbType.VarChar, 30)
                            .Value = usu.NombreUsuario;

                        cmd.Parameters.Add("@apellidoUsuario", SqlDbType.VarChar, 30)
                            .Value = usu.ApellidoUsuario;

                        cmd.Parameters.Add("@correo", SqlDbType.VarChar, 30)
                            .Value = usu.Correo;

                        cmd.Parameters.Add("@contrasena", SqlDbType.VarChar, 64)
                            .Value = usu.Contraseña;

                        cmd.Parameters.Add("@activo", SqlDbType.Bit)
                            .Value = usu.Activo;

                        cmd.Parameters.Add("@restablecer", SqlDbType.Bit)
                            .Value = true;

                        conn.Open();

                        idGenerado =
                            Convert.ToInt32(
                                cmd.ExecuteScalar()
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                idGenerado = 0;
                Mensaje = ex.Message;
            }

            return idGenerado;
        }


        /*METODO PARA EDITAR USUARIO*/
        public bool Editar(Usuario usu, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.cn))
                {
                    string consulta = @"
                UPDATE dbo.Usuario
                SET
                    id_rol = @idRol,
                    dni = @dni,
                    nombre_usuario = @nombreUsuario,
                    apellido_usuario = @apellidoUsuario,
                    correo = @correo,
                    activo = @activo
                WHERE id_usuario = @idUsuario;";

                    using (SqlCommand cmd = new SqlCommand(consulta, conn))
                    {
                        cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = usu.IdUsuario;
                        cmd.Parameters.Add("@idRol", SqlDbType.Int).Value = usu.IdRol;
                        cmd.Parameters.Add("@dni", SqlDbType.Int).Value = usu.Dni;
                        cmd.Parameters.Add("@nombreUsuario", SqlDbType.VarChar, 30).Value = usu.NombreUsuario;
                        cmd.Parameters.Add("@apellidoUsuario", SqlDbType.VarChar, 30).Value = usu.ApellidoUsuario;
                        cmd.Parameters.Add("@correo", SqlDbType.VarChar, 30).Value = usu.Correo;
                        cmd.Parameters.Add("@activo", SqlDbType.Bit).Value = usu.Activo;
                        conn.Open();
                        resultado = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }


        /*METODO PARA ELIMINAR USUARIO*/
        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(Conexion.cn))
                {
                    string consulta = @"
                UPDATE dbo.Usuario
                SET activo = 0
                WHERE id_usuario = @id;";

                    using (SqlCommand cmd = new SqlCommand(consulta, conn))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        conn.Open();
                        resultado = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }

    }
}
