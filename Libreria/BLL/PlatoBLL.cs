using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;



namespace BLL
{
    public class PlatoBLL
    {
        private PlatoDAL platoDAL = new PlatoDAL();
        private DigitoVerificadorBLL dvBLL = new DigitoVerificadorBLL();
        private DigitoVerificadorDAL dvDAL = new DigitoVerificadorDAL();
        private AuditoriaDAL auditoriaDAL = new AuditoriaDAL();

        public List<Plato> ObtenerTodos() { return platoDAL.ObtenerTodos(); }
        public List<Plato> ObtenerDisponibles() { return platoDAL.ObtenerDisponibles(); }
        public Plato ObtenerPorID(int idPlato) { return platoDAL.ObtenerPorID(idPlato); }

        public bool Agregar(Plato plato)
        {
            if (string.IsNullOrEmpty(plato.Nombre))
                throw new Exception("El nombre del plato es obligatorio.");
            if (plato.PrecioVenta <= 0)
                throw new Exception("El precio de venta debe ser mayor a cero.");

            bool resultado = platoDAL.Agregar(plato);
            if (resultado)
            {
                dvBLL.RecalcularDVV("PLATO", "PrecioVenta");
                dvDAL.ActualizarConteoFilas("PLATO", platoDAL.ObtenerTodos().Count);

                // Guardar snapshot del nuevo plato
                Plato nuevo = platoDAL.ObtenerTodos().Find(p => p.Nombre == plato.Nombre);
                if (nuevo != null)
                    GuardarSnapshotPlato(nuevo);
            }
            return resultado;
        }

        public bool Modificar(Plato plato)
        {
            if (string.IsNullOrEmpty(plato.Nombre))
                throw new Exception("El nombre del plato es obligatorio.");
            if (plato.PrecioVenta <= 0)
                throw new Exception("El precio de venta debe ser mayor a cero.");

            bool resultado = platoDAL.Modificar(plato);
            if (resultado)
            {
                dvBLL.RecalcularDVV("PLATO", "PrecioVenta");

                // Actualizar snapshot con los nuevos valores
                Plato actualizado = platoDAL.ObtenerPorID(plato.ID_Plato);
                if (actualizado != null)
                    GuardarSnapshotPlato(actualizado);
            }
            return resultado;
        }

        public bool CambiarDisponibilidad(int idPlato, bool disponible)
        {
            bool resultado = platoDAL.CambiarDisponibilidad(idPlato, disponible);
            if (resultado)
            {
                dvBLL.RecalcularDVV("PLATO", "PrecioVenta");

                // Actualizar snapshot
                Plato actualizado = platoDAL.ObtenerPorID(idPlato);
                if (actualizado != null)
                    GuardarSnapshotPlato(actualizado);
            }
            return resultado;
        }

        private void GuardarSnapshotPlato(Plato p)
        {
            // Eliminar snapshot anterior de este plato específico
            auditoriaDAL.EliminarSnapshotRegistro("PLATO", p.ID_Plato);

            auditoriaDAL.GuardarSnapshot("PLATO", p.ID_Plato, "Nombre", p.Nombre);
            auditoriaDAL.GuardarSnapshot("PLATO", p.ID_Plato, "Descripcion", p.Descripcion ?? "");
            auditoriaDAL.GuardarSnapshot("PLATO", p.ID_Plato, "PrecioVenta", p.PrecioVenta.ToString());
            auditoriaDAL.GuardarSnapshot("PLATO", p.ID_Plato, "ID_Categoria", p.ID_Categoria.ToString());
            auditoriaDAL.GuardarSnapshot("PLATO", p.ID_Plato, "Disponible", p.Disponible ? "1" : "0");
        }
    }
}