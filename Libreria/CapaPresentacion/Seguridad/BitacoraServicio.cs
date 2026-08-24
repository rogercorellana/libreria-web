using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Seguridad
{
    public static class BitacoraServicio
    {
        public static bool Registrar(
            string operacion,
            string modulo,
            string resultado,
            string descripcion,
            string tablaAfectada = null,
            string registroAfectado = null,
            int? idUsuario = null,
            int? idRol = null,
            string usuario = null)
        {
            // Si no se reciben datos explícitos, intentamos
            // obtenerlos del usuario autenticado.
            if (!idUsuario.HasValue)
            {
                int valorIdUsuario = Servicios.AutenticacionServicio.ObtenerIdUsuario();

                if (valorIdUsuario > 0)
                    idUsuario = valorIdUsuario;
            }

            if (!idRol.HasValue)
            {
                int valorIdRol = Servicios.AutenticacionServicio.ObtenerIdRol();

                if (valorIdRol > 0)
                    idRol = valorIdRol;
            }

            if (string.IsNullOrWhiteSpace(usuario))
            {
                usuario = Servicios.AutenticacionServicio.ObtenerCorreo();
            }

            Bitacora evento = new Bitacora
            {
                IdUsuario = idUsuario,
                Usuario = usuario,
                IdRol = idRol,
                Operacion = operacion,
                Modulo = modulo,
                Resultado = resultado,
                Descripcion = descripcion,
                TablaAfectada = tablaAfectada,
                RegistroAfectado = registroAfectado
            };

            string mensaje;

            bool registroExitoso =
                new CN_Bitacora().Registrar(
                    evento,
                    out mensaje
                );

            if (registroExitoso)
            {
                string mensajeIntegridad;

                new Servicios.IntegridadServicio()
                    .InicializarTabla(
                        "Bitacora",
                        out mensajeIntegridad
                    );
            }

            return registroExitoso;
        }
    }
}