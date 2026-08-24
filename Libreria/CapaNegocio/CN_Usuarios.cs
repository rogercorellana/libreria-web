using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using CapaDatos;
using Servicios;

namespace CapaNegocio
{
    public class CN_Usuarios
    {
        private CD_Usuarios objCD_Usuarios = new CD_Usuarios();

        public List<Usuario> Listar()
        {
            return objCD_Usuarios.Listar();
        }

        public bool CambiaClave(int idusuario, string nuevaclave, out string Mensaje)
        {
            return objCD_Usuarios.CambiarClave(idusuario, nuevaclave, out Mensaje);
        }

        public bool ReestablecerClave(int idusuario, string correo, out string Mensaje)
        {
            Mensaje = string.Empty;
            string nuevaclave = Configurador.GenerarClave();
            bool resultado = objCD_Usuarios.ReestablecerClave(idusuario, Encriptador.ConvertirSHA256(nuevaclave), out Mensaje);

            if(resultado)
            {
                string asunto = "Contraseña reestablecida";
                string mensajeCorreo = "<h3>Su cuenta se reestablecio correctamente</h3></br><p>Su contraeña para acceder ahora es: !clave!</p>";
                mensajeCorreo = mensajeCorreo.Replace("!clave", nuevaclave);
                bool respuesta = Configurador.EnviarCorreo(correo, asunto, mensajeCorreo);

                if(respuesta)
                {
                    return true;
                }
                else
                {
                    Mensaje = "No se pudo enviar el correo";
                    return false;
                }
            }
            else
            {
                Mensaje = "No se pudo reestablecer la contraseña";
                return false;
            }
        }

        public int Registrar(Usuario usu, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (usu.Dni <= 0)
            {
                Mensaje = "Debe ingresar un DNI válido.";
                return 0;
            }

            if (usu.IdRol <= 0)
            {
                Mensaje = "Debe seleccionar un rol.";
                return 0;
            }

            if (string.IsNullOrWhiteSpace(usu.NombreUsuario))
            {
                Mensaje = "Debe completar el nombre del usuario.";
                return 0;
            }

            if (string.IsNullOrWhiteSpace(usu.ApellidoUsuario))
            {
                Mensaje = "Debe completar el apellido del usuario.";
                return 0;
            }

            if (string.IsNullOrWhiteSpace(usu.Correo))
            {
                Mensaje = "Debe completar el correo del usuario.";
                return 0;
            }

            string clave = Configurador.GenerarClave();
            string asunto = "Credenciales de acceso";

            string mensajeCorreo =
                "<h3>Credenciales de acceso al sistema</h3>" +
                "</br>" +
                "<p>Su contraseña inicial para acceder es: " +
                clave +
                "</p>" +
                "</br>" +
                "<p>Al ingresar al sistema se le solicitará cambiar esta contraseña.</p>";

            bool respuesta =
                Configurador.EnviarCorreo(
                    usu.Correo,
                    asunto,
                    mensajeCorreo
                );

            if (!respuesta)
            {
                Mensaje ="No se pudo enviar el correo de creación de cuenta.";
                return 0;
            }

            usu.Contraseña = Encriptador.ConvertirSHA256(clave);
            return objCD_Usuarios.Registrar(usu,out Mensaje);
        }

        public bool Editar(Usuario usu, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (usu.Dni <= 0)
            {
                Mensaje = "Debe ingresar un DNI válido.";
            }
            else if (usu.IdRol <= 0)
            {
                Mensaje = "Debe seleccionar un rol.";
            }
            else if (string.IsNullOrWhiteSpace(usu.NombreUsuario))
            {
                Mensaje = "Debe completar el nombre del usuario.";
            }
            else if (string.IsNullOrWhiteSpace(usu.ApellidoUsuario))
            {
                Mensaje = "Debe completar el apellido del usuario.";
            }
            else if (string.IsNullOrWhiteSpace(usu.Correo))
            {
                Mensaje = "Debe completar el correo del usuario.";
            }

            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCD_Usuarios.Editar(usu, out Mensaje);
            }

            return false;
        }

        public bool Eliminar(int id, out string Mensaje)
        {
            return objCD_Usuarios.Eliminar(id, out Mensaje);
        }
    }
}
