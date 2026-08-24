using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using CapaNegocio;
using CapaPresentacion.Filters;
using CapaPresentacion.Seguridad;
using Servicios;

namespace CapaPresentacion.Controllers
{
    [Authorize]
    public class WebmasterController : Controller
    {
        // =========================
        // BACKUP
        // =========================

        [PermisoAuthorize("BACKUP")]
        public ActionResult Backup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermisoAuthorize("BACKUP")]
        public ActionResult EjecutarBackup()
        {
            string rutaBackup;
            string mensaje;

            bool resultado =
                new CN_Backup().RealizarBackup(out rutaBackup,out mensaje);

            if (resultado)
            {
                string nombreArchivo = Path.GetFileName(rutaBackup);

                BitacoraServicio.Registrar(
                    "BACKUP",
                    "BACKUP",
                    "EXITOSO",
                    "Backup realizado correctamente. Archivo: " + nombreArchivo
                );

                TempData["BackupExitoso"] = "Backup realizado correctamente. Archivo: " + nombreArchivo;
            }
            else
            {
                BitacoraServicio.Registrar(
                    "BACKUP",
                    "BACKUP",
                    "ERROR",
                    "No se pudo realizar el backup. Detalle: " + mensaje
                );

                TempData["BackupError"] = mensaje;
            }

            return RedirectToAction("Backup");
        }


        // =========================
        // RESTORE
        // =========================

        [PermisoAuthorize("RESTORE")]
        public ActionResult Restore()
        {
            string mensaje;

            List<string> backups = new CN_Restore().ListarBackups(out mensaje);

            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                ViewBag.Error = mensaje;
            }

            return View(backups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermisoAuthorize("RESTORE")]
        public ActionResult EjecutarRestore(string nombreArchivo)
        {
            string mensaje;

            bool resultado = new CN_Restore().RestaurarBackup(nombreArchivo, out mensaje);

            if (resultado)
            {
                BitacoraServicio.Registrar(
                    "RESTORE",
                    "RESTORE",
                    "EXITOSO",
                    "Base de datos restaurada correctamente desde el archivo: "
                    + nombreArchivo
                );

                TempData["RestoreExitoso"] = "La base de datos fue restaurada correctamente.";
            }
            else
            {
                BitacoraServicio.Registrar(
                    "RESTORE",
                    "RESTORE",
                    "ERROR",
                    "No se pudo restaurar la base de datos desde el archivo: "
                    + nombreArchivo +
                    ". Detalle: " + mensaje
                );

                TempData["RestoreError"] = mensaje;
            }

            return RedirectToAction("Restore");
        }


        // =========================
        // PRUEBA TEMPORAL INTEGRIDAD
        // =========================

        [PermisoAuthorize("VERIFICAR_INTEGRIDAD")]
        public ActionResult ProbarIntegridad()
        {
            string mensaje;

            bool resultado =
                new IntegridadServicio()
                    .InicializarTodasLasTablas(out mensaje);

            if (resultado)
            {
                return Content(
                    "Integridad inicializada correctamente. " +
                    mensaje
                );
            }

            return Content(
                "Error al inicializar la integridad: " +
                mensaje
            );
        }
    }

}