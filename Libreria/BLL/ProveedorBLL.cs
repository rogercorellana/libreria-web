using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class ProveedorBLL
    {
        private ProveedorDAL proveedorDAL = new ProveedorDAL();

        public List<Proveedor> ObtenerTodos()
        {
            return proveedorDAL.ObtenerTodos();
        }

        public Proveedor ObtenerPorID(int idProveedor)
        {
            return proveedorDAL.ObtenerPorID(idProveedor);
        }

        public bool Agregar(Proveedor proveedor)
        {
            if (string.IsNullOrEmpty(proveedor.Nombre))
                throw new Exception("El nombre del proveedor es obligatorio.");

            if (string.IsNullOrEmpty(proveedor.CUIT))
                throw new Exception("El CUIT del proveedor es obligatorio.");

            return proveedorDAL.Agregar(proveedor);
        }

        public bool Modificar(Proveedor proveedor)
        {
            if (string.IsNullOrEmpty(proveedor.Nombre))
                throw new Exception("El nombre del proveedor es obligatorio.");

            if (string.IsNullOrEmpty(proveedor.CUIT))
                throw new Exception("El CUIT del proveedor es obligatorio.");

            return proveedorDAL.Modificar(proveedor);
        }
    }
}