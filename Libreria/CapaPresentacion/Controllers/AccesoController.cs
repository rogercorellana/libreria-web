using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapaEntidad;
using CapaNegocio;
using Servicios;
using CapaPresentacion.Seguridad;
using System.Web.Security;//LIBRERIA QUE NOS PERMITE TRABAJAR CON AUTENTICACIONES DE FORMULARIO


namespace CapaPresentacion.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CambiarClave()
        {
            return View();
        }

        public ActionResult Reestablecer()
        {
            return View();
        }

        [Authorize]
        public ActionResult Denegado()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {
            List<Usuario> listaUsuarios = new CN_Usuarios().Listar();

            Usuario objUsuario = listaUsuarios
                .FirstOrDefault(u => u.Correo == correo);

            // No existe un usuario con ese correo
            if (objUsuario == null)
            {
                BitacoraServicio.Registrar(
                    "LOGIN",
                    "ACCESO",
                    "ERROR",
                    "Intento de inicio de sesión con credenciales incorrectas.",
                    usuario: correo
                );

                ViewBag.Error = "Correo o contraseña incorrecta";
                return View();
            }

            // El usuario existe, pero está desactivado
            if (!objUsuario.Activo)
            {
                BitacoraServicio.Registrar(
                    "LOGIN",
                    "ACCESO",
                    "DESACTIVADO",
                    "Intento de inicio de sesión de un usuario desactivado.",
                    tablaAfectada: "Usuario",
                    registroAfectado: objUsuario.IdUsuario.ToString(),
                    idUsuario: objUsuario.IdUsuario,
                    idRol: objUsuario.IdRol,
                    usuario: objUsuario.Correo
                );

                ViewBag.Error = "El usuario está actualmente desactivado.";
                return View();
            }

            // Contraseña incorrecta
            if (objUsuario.Contraseña !=
                Encriptador.ConvertirSHA256(clave))
            {
                BitacoraServicio.Registrar(
                    "LOGIN",
                    "ACCESO",
                    "ERROR",
                    "Intento de inicio de sesión con contraseña incorrecta.",
                    idUsuario: objUsuario.IdUsuario,
                    idRol: objUsuario.IdRol,
                    usuario: objUsuario.Correo
                );

                ViewBag.Error = "Correo o contraseña incorrecta";
                return View();
            }

            // Contraseña temporal: debe cambiarla
            if (objUsuario.Reestablecer)
            {
                BitacoraServicio.Registrar(
                    "LOGIN",
                    "ACCESO",
                    "REQUIERE_CAMBIO_CLAVE",
                    "El usuario inició sesión con una contraseña temporal.",
                    idUsuario: objUsuario.IdUsuario,
                    idRol: objUsuario.IdRol,
                    usuario: objUsuario.Correo
                );

                TempData["IdUsuario"] = objUsuario.IdUsuario;

                return RedirectToAction("CambiarClave");
            }

            string userData =
             objUsuario.IdUsuario.ToString()
             + "|"
             + objUsuario.IdRol.ToString()
             + "|"
             + objUsuario.Correo;

            FormsAuthenticationTicket ticket =
                new FormsAuthenticationTicket(
                    1,
                    objUsuario.Correo,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    false,
                    userData,
                    FormsAuthentication.FormsCookiePath
                );

            string encryptedTicket =
                FormsAuthentication.Encrypt(ticket);

            Response.Cookies.Add(
                new HttpCookie(
                    FormsAuthentication.FormsCookieName,
                    encryptedTicket
                )
            );

            BitacoraServicio.Registrar(
                "LOGIN",
                "ACCESO",
                "EXITOSO",
                "Inicio de sesión realizado correctamente.",
                idUsuario: objUsuario.IdUsuario,
                idRol: objUsuario.IdRol,
                usuario: objUsuario.Correo
            );

            ViewBag.Error = null;

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public ActionResult CambiarClave(
            string idusuario,
            string claveactual,
            string nuevaclave,
            string confirmarclave)
        {
            int idUsuario;

            if (!int.TryParse(idusuario, out idUsuario))
            {
                ViewBag.Error = "No se pudo identificar al usuario.";
                return View();
            }

            Usuario objUsuario =
                new CN_Usuarios()
                    .Listar()
                    .Where(u => u.IdUsuario == idUsuario)
                    .FirstOrDefault();

            if (objUsuario == null)
            {
                ViewBag.Error = "No se encontró el usuario.";
                return View();
            }

            // Validar contraseña actual
            if (objUsuario.Contraseña !=
                Encriptador.ConvertirSHA256(claveactual))
            {
                BitacoraServicio.Registrar(
                    "CAMBIO_CLAVE",
                    "ACCESO",
                    "ERROR",
                    "El usuario ingresó una contraseña actual incorrecta.",
                    tablaAfectada: "Usuario",
                    registroAfectado: idUsuario.ToString(),
                    idUsuario: idUsuario,
                    idRol: objUsuario.IdRol,
                    usuario: objUsuario.Correo
                );

                TempData["IdUsuario"] = idUsuario;
                ViewData["vclave"] = "";
                ViewBag.Error =
                    "La contraseña actual no es correcta.";

                return View();
            }

            // Validar coincidencia de nuevas contraseñas
            if (nuevaclave != confirmarclave)
            {
                BitacoraServicio.Registrar(
                    "CAMBIO_CLAVE",
                    "ACCESO",
                    "ERROR",
                    "Las nuevas contraseñas no coinciden.",
                    tablaAfectada: "Usuario",
                    registroAfectado: idUsuario.ToString(),
                    idUsuario: idUsuario,
                    idRol: objUsuario.IdRol,
                    usuario: objUsuario.Correo
                );

                TempData["IdUsuario"] = idUsuario;
                ViewData["vclave"] = claveactual;
                ViewBag.Error =
                    "Las contraseñas no coinciden.";

                return View();
            }

            ViewData["vclave"] = "";

            string mensaje = string.Empty;

            string nuevaClaveHash =
                Encriptador.ConvertirSHA256(nuevaclave);

            bool respuesta =
                new CN_Usuarios().CambiaClave(
                    idUsuario,
                    nuevaClaveHash,
                    out mensaje
                );

            if (respuesta)
            {
                BitacoraServicio.Registrar(
                    "CAMBIO_CLAVE",
                    "ACCESO",
                    "EXITOSO",
                    "La contraseña del usuario fue modificada correctamente.",
                    tablaAfectada: "Usuario",
                    registroAfectado: idUsuario.ToString(),
                    idUsuario: idUsuario,
                    idRol: objUsuario.IdRol,
                    usuario: objUsuario.Correo
                );

                return RedirectToAction("Index");
            }

            BitacoraServicio.Registrar(
                "CAMBIO_CLAVE",
                "ACCESO",
                "ERROR",
                "No se pudo modificar la contraseña. Detalle: " + mensaje,
                tablaAfectada: "Usuario",
                registroAfectado: idUsuario.ToString(),
                idUsuario: idUsuario,
                idRol: objUsuario.IdRol,
                usuario: objUsuario.Correo
            );

            TempData["IdUsuario"] = idUsuario;
            ViewBag.Error = mensaje;

            return View();
        }

        [HttpPost]
        public ActionResult Reestablecer(string correo)
        {
            Usuario objUsuario =
                new CN_Usuarios()
                    .Listar()
                    .Where(item => item.Correo == correo)
                    .FirstOrDefault();

            // No existe el usuario
            if (objUsuario == null)
            {
                BitacoraServicio.Registrar(
                    "RECUPERACION_CLAVE",
                    "ACCESO",
                    "ERROR",
                    "Intento de recuperación de contraseña para un correo no registrado.",
                    usuario: correo
                );

                ViewBag.Error =
                    "No se encontró un usuario relacionado con ese correo.";

                return View();
            }

            // El usuario está desactivado
            if (!objUsuario.Activo)
            {
                BitacoraServicio.Registrar(
                    "RECUPERACION_CLAVE",
                    "ACCESO",
                    "DESACTIVADO",
                    "Intento de recuperación de contraseña de un usuario desactivado.",
                    tablaAfectada: "Usuario",
                    registroAfectado: objUsuario.IdUsuario.ToString(),
                    idUsuario: objUsuario.IdUsuario,
                    idRol: objUsuario.IdRol,
                    usuario: objUsuario.Correo
                );

                ViewBag.Error =
                    "El usuario está actualmente desactivado.";

                return View();
            }

            string mensaje = string.Empty;

            bool respuesta =
                new CN_Usuarios().ReestablecerClave(
                    objUsuario.IdUsuario,
                    correo,
                    out mensaje
                );

            if (respuesta)
            {
                BitacoraServicio.Registrar(
                    "RECUPERACION_CLAVE",
                    "ACCESO",
                    "EXITOSO",
                    "La contraseña fue restablecida y enviada correctamente por correo.",
                    tablaAfectada: "Usuario",
                    registroAfectado: objUsuario.IdUsuario.ToString(),
                    idUsuario: objUsuario.IdUsuario,
                    idRol: objUsuario.IdRol,
                    usuario: objUsuario.Correo
                );

                ViewBag.Error = null;

                return RedirectToAction(
                    "Index",
                    "Acceso"
                );
            }

            BitacoraServicio.Registrar(
                "RECUPERACION_CLAVE",
                "ACCESO",
                "ERROR",
                "No se pudo restablecer la contraseña. Detalle: " + mensaje,
                tablaAfectada: "Usuario",
                registroAfectado: objUsuario.IdUsuario.ToString(),
                idUsuario: objUsuario.IdUsuario,
                idRol: objUsuario.IdRol,
                usuario: objUsuario.Correo
            );

            ViewBag.Error = mensaje;

            return View();
        }

        public ActionResult CerrarSesion()
        {
            int idUsuario = Servicios.AutenticacionServicio.ObtenerIdUsuario();
            int idRol = Servicios.AutenticacionServicio.ObtenerIdRol();
            string correo = Servicios.AutenticacionServicio.ObtenerCorreo();

            BitacoraServicio.Registrar(
                "LOGOUT",
                "ACCESO",
                "EXITOSO",
                "Cierre de sesión realizado correctamente.",
                idUsuario: idUsuario > 0 ? idUsuario : (int?)null,
                idRol: idRol > 0 ? idRol : (int?)null,
                usuario: correo
            );

            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Acceso");
        }
    }
}