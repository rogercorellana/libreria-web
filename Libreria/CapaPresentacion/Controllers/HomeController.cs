using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Filters;
using CapaPresentacion.Seguridad;

namespace CapaPresentacion.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [PermisoAuthorize("GESTION_USUARIOS")]
        public ActionResult Usuarios()
        {
            return View();
        }

        [HttpGet]
        [PermisoAuthorize("GESTION_USUARIOS")]
        public JsonResult ListarUsuarios()
        {
            List<Usuario> objLista =
                new CN_Usuarios().Listar();

            return Json(
                new { data = objLista },
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpPost]
        [PermisoAuthorize("GESTION_USUARIOS")]
        public JsonResult GuardarUsuario(Usuario usu)
        {
            object resultado;
            string mensaje = string.Empty;

            if (usu.IdUsuario == 0)
            {
                int idGenerado =
                    new CN_Usuarios()
                        .Registrar(usu, out mensaje);

                resultado = idGenerado;

                if (idGenerado > 0)
                {
                    BitacoraServicio.Registrar(
                        "ALTA_USUARIO",
                        "USUARIOS",
                        "EXITOSO",
                        "Usuario registrado correctamente.",
                        tablaAfectada: "Usuario",
                        registroAfectado: idGenerado.ToString()
                    );
                }
                else
                {
                    BitacoraServicio.Registrar(
                        "ALTA_USUARIO",
                        "USUARIOS",
                        "ERROR",
                        "No se pudo registrar el usuario. Detalle: " + mensaje,
                        tablaAfectada: "Usuario"
                    );
                }
            }
            else
            {
                int idUsuario = usu.IdUsuario;

                bool respuesta =
                    new CN_Usuarios()
                        .Editar(usu, out mensaje);

                resultado = respuesta;

                if (respuesta)
                {
                    BitacoraServicio.Registrar(
                        "MODIFICACION_USUARIO",
                        "USUARIOS",
                        "EXITOSO",
                        "Usuario modificado correctamente.",
                        tablaAfectada: "Usuario",
                        registroAfectado: idUsuario.ToString()
                    );
                }
                else
                {
                    BitacoraServicio.Registrar(
                        "MODIFICACION_USUARIO",
                        "USUARIOS",
                        "ERROR",
                        "No se pudo modificar el usuario. Detalle: " + mensaje,
                        tablaAfectada: "Usuario",
                        registroAfectado: idUsuario.ToString()
                    );
                }
            }

            return Json(
                new
                {
                    resultado = resultado,
                    mensaje = mensaje
                },
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpPost]
        [PermisoAuthorize("GESTION_USUARIOS")]
        public JsonResult EliminarUsuario(int id)
        {
            string mensaje = string.Empty;

            bool respuesta =
                new CN_Usuarios()
                    .Eliminar(id, out mensaje);

            if (respuesta)
            {
                BitacoraServicio.Registrar(
                    "BAJA_USUARIO",
                    "USUARIOS",
                    "EXITOSO",
                    "Usuario eliminado correctamente.",
                    tablaAfectada: "Usuario",
                    registroAfectado: id.ToString()
                );
            }
            else
            {
                BitacoraServicio.Registrar(
                    "BAJA_USUARIO",
                    "USUARIOS",
                    "ERROR",
                    "No se pudo eliminar el usuario. Detalle: " + mensaje,
                    tablaAfectada: "Usuario",
                    registroAfectado: id.ToString()
                );
            }

            return Json(
                new
                {
                    resultado = respuesta,
                    mensaje = mensaje
                },
                JsonRequestBehavior.AllowGet
            );
        }
    }

}
