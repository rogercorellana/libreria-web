using BE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DAL
{
    public class UsuarioDAL
    {
        private AuditoriaDAL auditoriaDAL = new AuditoriaDAL();

        private string CalcularDVH(Usuario usuario)
        {
            return DigitoVerificador.CalcularDVH(
                usuario.Nombre,
                usuario.Apellido,
                usuario.Username,
                usuario.Mail,
                usuario.Rol,
                usuario.Activo ? 1 : 0,
                usuario.IntentosFallidos
            );
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                ID_Usuario = Convert.ToInt32(reader["ID_Usuario"]),
                Nombre = reader["Nombre"].ToString(),
                Apellido = reader["Apellido"].ToString(),
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString(),
                Mail = reader["Mail"].ToString(),
                Rol = reader["Rol"].ToString(),
                Activo = Convert.ToBoolean(reader["Activo"]),
                IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]),
                DVH = reader["DVH"].ToString()
            };
        }

        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> lista = new List<Usuario>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM USUARIO";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    lista.Add(MapearUsuario(reader));
            }
            return lista;
        }

        public Usuario ObtenerPorID(int idUsuario)
        {
            Usuario usuario = null;
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM USUARIO WHERE ID_Usuario = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", idUsuario);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    usuario = MapearUsuario(reader);
            }
            return usuario;
        }

        public Usuario ObtenerPorUsername(string username)
        {
            Usuario usuario = null;
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM USUARIO WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    usuario = MapearUsuario(reader);
            }
            return usuario;
        }

        public bool Agregar(Usuario usuario)
        {
            try
            {
                string dvh = CalcularDVH(usuario);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO USUARIO 
                        (Nombre, Apellido, Username, Password, Mail, Rol, Activo, IntentosFallidos, DVH)
                        VALUES 
                        (@Nombre, @Apellido, @Username, @Password, @Mail, @Rol, @Activo, @Intentos, @DVH)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@Username", usuario.Username);
                    cmd.Parameters.AddWithValue("@Password", usuario.Password);
                    cmd.Parameters.AddWithValue("@Mail", usuario.Mail);
                    cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
                    cmd.Parameters.AddWithValue("@Activo", usuario.Activo);
                    cmd.Parameters.AddWithValue("@Intentos", usuario.IntentosFallidos);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public bool Modificar(Usuario usuario)
        {
            try
            {
                Usuario anterior = ObtenerPorID(usuario.ID_Usuario);
                if (anterior != null)
                {
                    auditoriaDAL.GuardarValorAnterior("USUARIO", usuario.ID_Usuario, "Activo", anterior.Activo.ToString());
                    auditoriaDAL.GuardarValorAnterior("USUARIO", usuario.ID_Usuario, "Rol", anterior.Rol);
                    auditoriaDAL.GuardarValorAnterior("USUARIO", usuario.ID_Usuario, "IntentosFallidos", anterior.IntentosFallidos.ToString());
                }

                string dvh = CalcularDVH(usuario);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"UPDATE USUARIO SET
                        Nombre           = @Nombre,
                        Apellido         = @Apellido,
                        Username         = @Username,
                        Password         = @Password,
                        Mail             = @Mail,
                        Rol              = @Rol,
                        DVH              = @DVH
                        WHERE ID_Usuario = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@Username", usuario.Username);
                    cmd.Parameters.AddWithValue("@Password", usuario.Password);
                    cmd.Parameters.AddWithValue("@Mail", usuario.Mail);
                    cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", usuario.ID_Usuario);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public bool Bloquear(int idUsuario)
        {
            try
            {
                Usuario usuario = ObtenerPorID(idUsuario);
                auditoriaDAL.GuardarValorAnterior("USUARIO", idUsuario, "Activo", usuario.Activo.ToString());
                auditoriaDAL.GuardarValorAnterior("USUARIO", idUsuario, "IntentosFallidos", usuario.IntentosFallidos.ToString());
                usuario.Activo = false;
                usuario.IntentosFallidos = 3;
                string dvh = CalcularDVH(usuario);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"UPDATE USUARIO SET 
                        Activo           = 0, 
                        IntentosFallidos = 3,
                        DVH              = @DVH
                        WHERE ID_Usuario = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", idUsuario);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public bool Desbloquear(int idUsuario)
        {
            try
            {
                Usuario usuario = ObtenerPorID(idUsuario);
                auditoriaDAL.GuardarValorAnterior("USUARIO", idUsuario, "Activo", usuario.Activo.ToString());
                auditoriaDAL.GuardarValorAnterior("USUARIO", idUsuario, "IntentosFallidos", usuario.IntentosFallidos.ToString());
                usuario.Activo = true;
                usuario.IntentosFallidos = 0;
                string dvh = CalcularDVH(usuario);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"UPDATE USUARIO SET 
                        Activo           = 1, 
                        IntentosFallidos = 0,
                        DVH              = @DVH
                        WHERE ID_Usuario = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", idUsuario);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public void ActualizarIntentosFallidos(int idUsuario, int intentos)
        {
            Usuario usuario = ObtenerPorID(idUsuario);
            auditoriaDAL.GuardarValorAnterior("USUARIO", idUsuario, "IntentosFallidos", usuario.IntentosFallidos.ToString());
            usuario.IntentosFallidos = intentos;
            string dvh = CalcularDVH(usuario);
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"UPDATE USUARIO SET 
                    IntentosFallidos = @Intentos,
                    DVH              = @DVH
                    WHERE ID_Usuario = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Intentos", intentos);
                cmd.Parameters.AddWithValue("@DVH", dvh);
                cmd.Parameters.AddWithValue("@ID", idUsuario);
                cmd.ExecuteNonQuery();
            }
        }

        public void BloquearUsuario(int idUsuario) { Bloquear(idUsuario); }

        public void ResetearIntentos(int idUsuario)
        {
            Usuario usuario = ObtenerPorID(idUsuario);
            auditoriaDAL.GuardarValorAnterior("USUARIO", idUsuario, "IntentosFallidos", usuario.IntentosFallidos.ToString());
            usuario.IntentosFallidos = 0;
            string dvh = CalcularDVH(usuario);
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"UPDATE USUARIO SET 
                    IntentosFallidos = 0,
                    DVH              = @DVH
                    WHERE ID_Usuario = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DVH", dvh);
                cmd.Parameters.AddWithValue("@ID", idUsuario);
                cmd.ExecuteNonQuery();
            }
        }

        public void RecalcularTodosDVH()
        {
            List<Usuario> lista = ObtenerTodos();
            foreach (var u in lista)
            {
                string dvh = CalcularDVH(u);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = "UPDATE USUARIO SET DVH = @DVH WHERE ID_Usuario = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", u.ID_Usuario);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}