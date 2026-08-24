using System;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Bitacora
    {
        private CD_Bitacora objCD_Bitacora = new CD_Bitacora();

        public bool Registrar(Bitacora evento, out string mensaje)
        {
            return objCD_Bitacora.Registrar(evento,out mensaje);
        }
    }
}
