using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace BLL
{
    public class UsuarioBLL
    {
        private UsuarioDAL usuarioDAL = new UsuarioDAL();
        private DigitoVerificadorBLL dvBLL = new DigitoVerificadorBLL();
        private DigitoVerificadorDAL dvDAL = new DigitoVerificadorDAL();

        public List<Usuario> ObtenerTodos()
        {
            return usuarioDAL.ObtenerTodos();
        }

        public Usuario ObtenerPorID(int idUsuario)
        {
            return usuarioDAL.ObtenerPorID(idUsuario);
        }

        public Usuario ObtenerPorUsername(string username)
        {
            return usuarioDAL.ObtenerPorUsername(username);
        }

        public bool Agregar(Usuario usuario)
        {
            if (usuarioDAL.ObtenerPorUsername(usuario.Username) != null)
                throw new Exception("El username ya existe en el sistema.");

            if (!ValidarPassword(usuario.Password))
                throw new Exception("La contraseña debe tener mínimo 8 caracteres y un carácter especial.");

            usuario.Password = HashSHA256(usuario.Password);
            usuario.Activo = true;
            usuario.IntentosFallidos = 0;

            bool resultado = usuarioDAL.Agregar(usuario);

            if (resultado)
            {
                dvBLL.RecalcularDVV("USUARIO", "Activo");
                dvBLL.RecalcularDVV("USUARIO", "Rol");
                dvDAL.ActualizarConteoFilas("USUARIO", usuarioDAL.ObtenerTodos().Count);
            }

            return resultado;
        }

        public bool Modificar(Usuario usuario)
        {
            Usuario usuarioExistente = usuarioDAL.ObtenerPorID(usuario.ID_Usuario);

            if (usuarioExistente == null)
                throw new Exception("El usuario no existe.");

            if (!string.IsNullOrEmpty(usuario.Password))
            {
                if (!ValidarPassword(usuario.Password))
                    throw new Exception("La contraseña debe tener mínimo 8 caracteres y un carácter especial.");
                usuario.Password = HashSHA256(usuario.Password);
            }
            else
            {
                usuario.Password = usuarioExistente.Password;
            }

            usuario.Activo = usuarioExistente.Activo;
            usuario.IntentosFallidos = usuarioExistente.IntentosFallidos;

            bool resultado = usuarioDAL.Modificar(usuario);

            if (resultado)
            {
                dvBLL.RecalcularDVV("USUARIO", "Activo");
                dvBLL.RecalcularDVV("USUARIO", "Rol");
            }
            return resultado;
        }

        public bool Bloquear(int idUsuario)
        {
            Usuario usuario = usuarioDAL.ObtenerPorID(idUsuario);

            if (usuario == null)
                throw new Exception("El usuario no existe.");

            if (!usuario.Activo)
                throw new Exception("El usuario ya está bloqueado.");

            bool resultado = usuarioDAL.Bloquear(idUsuario);

            if (resultado)
            {
                dvBLL.RecalcularDVV("USUARIO", "Activo");
                dvBLL.RecalcularDVV("USUARIO", "Rol");
            }
            return resultado;
        }

        public bool Desbloquear(int idUsuario)
        {
            Usuario usuario = usuarioDAL.ObtenerPorID(idUsuario);

            if (usuario == null)
                throw new Exception("El usuario no existe.");

            if (usuario.Activo)
                throw new Exception("El usuario ya está activo.");

            bool resultado = usuarioDAL.Desbloquear(idUsuario);

            if (resultado)
            {
                dvBLL.RecalcularDVV("USUARIO", "Activo");
                dvBLL.RecalcularDVV("USUARIO", "Rol");
            }

            return resultado;
        }

        public Usuario ValidarLogin(string username, string password)
        {
            Usuario usuario = usuarioDAL.ObtenerPorUsername(username);

            if (usuario == null) return null;
            if (!usuario.Activo) return null;

            string passwordHash = HashSHA256(password);

            if (usuario.Password == passwordHash)
            {
                usuarioDAL.ResetearIntentos(usuario.ID_Usuario);
                return usuario;
            }
            else
            {
                int nuevosIntentos = usuario.IntentosFallidos + 1;

                if (nuevosIntentos >= 3 && usuario.Rol != "ROL-03")
                    usuarioDAL.BloquearUsuario(usuario.ID_Usuario);
                else
                    usuarioDAL.ActualizarIntentosFallidos(usuario.ID_Usuario, nuevosIntentos);

                return null;
            }
        }

        private bool ValidarPassword(string password)
        {
            if (password.Length < 8) return false;
            string especiales = "!@#$%^&*(),.?\":{}|<>";
            foreach (char c in password)
                if (especiales.Contains(c.ToString())) return true;
            return false;
        }

        private string HashSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}