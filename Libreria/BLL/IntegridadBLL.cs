using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BE;
using DAL;


namespace BLL
{
    public class IntegridadBLL
    {
        private UsuarioDAL usuarioDAL = new UsuarioDAL();
        private PedidoDAL pedidoDAL = new PedidoDAL();
        private DetallePedidoDAL detalleDAL = new DetallePedidoDAL();
        private PagoDAL pagoDAL = new PagoDAL();
        private PlatoDAL platoDAL = new PlatoDAL();
        private MovimientoStockDAL movDAL = new MovimientoStockDAL();
        private DigitoVerificadorBLL dvBLL = new DigitoVerificadorBLL();
        private DigitoVerificadorDAL dvDAL = new DigitoVerificadorDAL();
        private AuditoriaDAL auditoriaDAL = new AuditoriaDAL();
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();
        private BackupDAL backupDAL = new BackupDAL();

        public List<string> ValidarIntegridadWebmaster()
        {
            List<string> errores = new List<string>();
            var erroresPlatos = ValidarDVHPlatosDetallado();
            var erroresMovimientos = ValidarDVHMovimientosDetallado();
            var erroresDetalles = ValidarDVHDetallesDetallado();
            errores.AddRange(erroresPlatos);
            errores.AddRange(erroresMovimientos);
            errores.AddRange(erroresDetalles);
            errores.AddRange(ValidarDVVDetallado("PLATO", "PrecioVenta", platoDAL.ObtenerTodos().Count, erroresPlatos.Count));
            errores.AddRange(ValidarDVVDetallado("MOVIMIENTO_STOCK", "Cantidad", movDAL.ObtenerTodos().Count, erroresMovimientos.Count));
            errores.AddRange(ValidarDVVPorTablaDetalle("DETALLE_PEDIDO", detalleDAL.ObtenerTodos().Count, erroresDetalles.Count));
            return errores;
        }

        public List<string> ValidarIntegridadAdmin()
        {
            List<string> errores = new List<string>();
            var erroresUsuarios = ValidarDVHUsuariosDetallado();
            var erroresPedidos = ValidarDVHPedidosDetallado();
            var erroresPagos = ValidarDVHPagosDetallado();
            errores.AddRange(erroresUsuarios);
            errores.AddRange(erroresPedidos);
            errores.AddRange(erroresPagos);

            // DVV por tabla — una sola verificación por tabla para evitar mensajes duplicados
            errores.AddRange(ValidarDVVPorTabla("USUARIO", usuarioDAL.ObtenerTodos().Count, erroresUsuarios.Count));
            errores.AddRange(ValidarDVVDetallado("PEDIDO", "Total", pedidoDAL.ObtenerTodos().Count, erroresPedidos.Count));
            errores.AddRange(ValidarDVVDetallado("PAGO", "Monto", pagoDAL.ObtenerTodos().Count, erroresPagos.Count));
            return errores;
        }

        // Valida DVV de USUARIO agrupando Activo y Rol — un solo mensaje por tabla
        private List<string> ValidarDVVPorTabla(string tabla, int filasActuales, int erroresDVHCount)
        {
            bool activoOk = dvBLL.VerificarDVV(tabla, "Activo");
            bool rolOk = dvBLL.VerificarDVV(tabla, "Rol");
            if (activoOk && rolOk)
                return new List<string>();

            // Al menos una columna DVV falla — generar un solo mensaje
            return ValidarDVVDetallado(tabla, "Activo", filasActuales, erroresDVHCount);
        }

        // ============================================
        // HELPER — genera el mensaje según si hay snapshot
        // ============================================
        private string GenerarMensajeDVH(string tabla, string idInfo, Dictionary<string, string> snapshot, Dictionary<string, string> actuales)
        {
            if (snapshot == null || snapshot.Count == 0)
                return $"Se agregó un registro no autorizado en la tabla {tabla} — {idInfo}.";

            List<string> cambios = DetectarCambios(snapshot, actuales);
            string detalle = cambios.Count > 0
                ? string.Join(", ", cambios)
                : "valores alterados (no se detectaron cambios específicos)";
            return $"Se modificó en la tabla {tabla} — {idInfo}. {detalle}.";
        }

        private List<string> DetectarCambios(Dictionary<string, string> snapshot, Dictionary<string, string> actuales)
        {
            List<string> cambios = new List<string>();
            foreach (var campo in actuales)
            {
                if (snapshot.ContainsKey(campo.Key) && snapshot[campo.Key] != campo.Value)
                    cambios.Add($"{campo.Key} cambió de '{snapshot[campo.Key]}' a '{campo.Value}'");
            }
            return cambios;
        }

        // ============================================
        // VALIDACIONES DVH
        // ============================================
        private List<string> ValidarDVHPlatosDetallado()
        {
            List<string> errores = new List<string>();
            foreach (var p in platoDAL.ObtenerTodos())
            {
                string dvhEsperado = DigitoVerificador.CalcularDVH(
                    p.Nombre, p.Descripcion ?? "", p.PrecioVenta, p.ID_Categoria, p.Disponible ? 1 : 0);
                if (p.DVH != dvhEsperado)
                {
                    var snap = auditoriaDAL.ObtenerUltimosSnapshots("PLATO", p.ID_Plato);
                    errores.Add(GenerarMensajeDVH("PLATO", $"ID {p.ID_Plato} (Nombre: {p.Nombre})", snap,
                        new Dictionary<string, string> {
                            { "Nombre", p.Nombre },
                            { "Descripcion", p.Descripcion ?? "" },
                            { "PrecioVenta", p.PrecioVenta.ToString() },
                            { "ID_Categoria", p.ID_Categoria.ToString() },
                            { "Disponible", p.Disponible ? "1" : "0" }
                        }));
                }
            }
            return errores;
        }

        private List<string> ValidarDVHUsuariosDetallado()
        {
            List<string> errores = new List<string>();
            foreach (var u in usuarioDAL.ObtenerTodos())
            {
                string dvhEsperado = DigitoVerificador.CalcularDVH(
                    u.Nombre, u.Apellido, u.Username, u.Mail, u.Rol, u.Activo ? 1 : 0, u.IntentosFallidos);
                if (u.DVH != dvhEsperado)
                {
                    var snap = auditoriaDAL.ObtenerUltimosSnapshots("USUARIO", u.ID_Usuario);
                    errores.Add(GenerarMensajeDVH("USUARIO", $"ID {u.ID_Usuario} (Username: {u.Username})", snap,
                        new Dictionary<string, string> {
                            { "Nombre", u.Nombre },
                            { "Apellido", u.Apellido },
                            { "Username", u.Username },
                            { "Mail", u.Mail },
                            { "Rol", u.Rol },
                            { "Activo", u.Activo ? "1" : "0" },
                            { "IntentosFallidos", u.IntentosFallidos.ToString() }
                        }));
                }
            }
            return errores;
        }

        private List<string> ValidarDVHPedidosDetallado()
        {
            List<string> errores = new List<string>();
            foreach (var p in pedidoDAL.ObtenerTodos())
            {
                string dvhEsperado = DigitoVerificador.CalcularDVH(
                    p.ID_Mesa, p.ID_Usuario, p.Estado, p.Total);
                if (p.DVH != dvhEsperado)
                {
                    var snap = auditoriaDAL.ObtenerUltimosSnapshots("PEDIDO", p.ID_Pedido);
                    errores.Add(GenerarMensajeDVH("PEDIDO", $"ID {p.ID_Pedido}", snap,
                        new Dictionary<string, string> {
                            { "ID_Mesa", p.ID_Mesa.ToString() },
                            { "ID_Usuario", p.ID_Usuario.ToString() },
                            { "Estado", p.Estado },
                            { "Total", p.Total.ToString() }
                        }));
                }
            }
            return errores;
        }

        private List<string> ValidarDVHPagosDetallado()
        {
            List<string> errores = new List<string>();
            foreach (var p in pagoDAL.ObtenerTodos())
            {
                string dvhEsperado = DigitoVerificador.CalcularDVH(
                    p.ID_Pedido, p.MetodoPago, p.Monto, p.ID_Usuario);
                if (p.DVH != dvhEsperado)
                {
                    var snap = auditoriaDAL.ObtenerUltimosSnapshots("PAGO", p.ID_Pago);
                    errores.Add(GenerarMensajeDVH("PAGO", $"ID {p.ID_Pago}", snap,
                        new Dictionary<string, string> {
                            { "ID_Pedido", p.ID_Pedido.ToString() },
                            { "MetodoPago", p.MetodoPago },
                            { "Monto", p.Monto.ToString() },
                            { "ID_Usuario", p.ID_Usuario.ToString() }
                        }));
                }
            }
            return errores;
        }

        private List<string> ValidarDVHMovimientosDetallado()
        {
            List<string> errores = new List<string>();
            foreach (var m in movDAL.ObtenerTodos())
            {
                string dvhEsperado = DigitoVerificador.CalcularDVH(
                    m.ID_Ingrediente, m.ID_Proveedor, m.Cantidad, m.ID_Usuario);
                if (m.DVH != dvhEsperado)
                {
                    var snap = auditoriaDAL.ObtenerUltimosSnapshots("MOVIMIENTO_STOCK", m.ID_Movimiento);
                    errores.Add(GenerarMensajeDVH("MOVIMIENTO_STOCK", $"ID {m.ID_Movimiento}", snap,
                        new Dictionary<string, string> {
                            { "ID_Ingrediente", m.ID_Ingrediente.ToString() },
                            { "ID_Proveedor", m.ID_Proveedor.ToString() },
                            { "Cantidad", m.Cantidad.ToString() },
                            { "ID_Usuario", m.ID_Usuario.ToString() }
                        }));
                }
            }
            return errores;
        }

        private List<string> ValidarDVHDetallesDetallado()
        {
            List<string> errores = new List<string>();
            foreach (var d in detalleDAL.ObtenerTodos())
            {
                string dvhEsperado = DigitoVerificador.CalcularDVH(
                    d.ID_Pedido, d.ID_Plato, d.Cantidad, d.PrecioUnitario, d.Subtotal);
                if (d.DVH != dvhEsperado)
                {
                    var snap = auditoriaDAL.ObtenerUltimosSnapshots("DETALLE_PEDIDO", d.ID_Detalle);
                    errores.Add(GenerarMensajeDVH("DETALLE_PEDIDO", $"ID {d.ID_Detalle} (Pedido: {d.ID_Pedido})", snap,
                        new Dictionary<string, string> {
                            { "ID_Pedido", d.ID_Pedido.ToString() },
                            { "ID_Plato", d.ID_Plato.ToString() },
                            { "Cantidad", d.Cantidad.ToString() },
                            { "PrecioUnitario", d.PrecioUnitario.ToString() },
                            { "Subtotal", d.Subtotal.ToString() }
                        }));
                }
            }
            return errores;
        }

        private List<string> ValidarDVVDetallado(string tabla, string columna, int filasActuales, int erroresDVHCount)
        {
            List<string> errores = new List<string>();
            if (dvBLL.VerificarDVV(tabla, columna))
                return errores;

            int filasGuardadas = dvDAL.ObtenerConteoFilas(tabla);

            if (filasGuardadas == -1)
            {
                errores.Add($"Se detectó una alteración en la columna {columna} de la tabla {tabla}. No se puede determinar el tipo de cambio.");
            }
            else if (filasActuales < filasGuardadas)
            {
                // Detectar qué IDs fueron eliminados comparando snapshot vs actuales
                List<int> idsEnSnapshot = auditoriaDAL.ObtenerIDsEnSnapshot(tabla);
                List<int> idsActuales = ObtenerIDsActuales(tabla);
                List<int> idsEliminados = idsEnSnapshot.FindAll(id => !idsActuales.Contains(id));

                if (idsEliminados.Count > 0)
                {
                    foreach (int id in idsEliminados)
                    {
                        var snap = auditoriaDAL.ObtenerUltimosSnapshots(tabla, id);
                        string info = ObtenerInfoRegistro(tabla, id, snap);
                        errores.Add($"Se eliminó el registro {info} de la tabla {tabla}.");
                    }
                }
                else
                {
                    errores.Add($"Se eliminaron {filasGuardadas - filasActuales} registro(s) de la tabla {tabla}.");
                }
            }
            else if (filasActuales > filasGuardadas)
            {
                errores.Add($"Se agregaron {filasActuales - filasGuardadas} registro(s) no autorizado(s) en la tabla {tabla}.");
            }
            else if (erroresDVHCount == 0)
            {
                errores.Add($"Se detectó una alteración en la columna {columna} de la tabla {tabla}. El verificador vertical no coincide.");
            }

            return errores;
        }

        // Obtiene los IDs actuales de una tabla
        private List<int> ObtenerIDsActuales(string tabla)
        {
            List<int> ids = new List<int>();
            string pkColumna = ObtenerPK(tabla);
            using (var con = DAL.Conexion.ObtenerConexion())
            {
                var cmd = new System.Data.SqlClient.SqlCommand($"SELECT {pkColumna} FROM {tabla}", con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    ids.Add(Convert.ToInt32(reader[0]));
            }
            return ids;
        }

        // Devuelve el nombre de la PK según la tabla
        private string ObtenerPK(string tabla)
        {
            switch (tabla)
            {
                case "PLATO": return "ID_Plato";
                case "USUARIO": return "ID_Usuario";
                case "PEDIDO": return "ID_Pedido";
                case "PAGO": return "ID_Pago";
                case "DETALLE_PEDIDO": return "ID_Detalle";
                case "MOVIMIENTO_STOCK": return "ID_Movimiento";
                default: return "ID";
            }
        }

        // Construye un string descriptivo del registro eliminado usando el snapshot
        private string ObtenerInfoRegistro(string tabla, int id, Dictionary<string, string> snap)
        {
            switch (tabla)
            {
                case "PLATO":
                    string nombre = snap.ContainsKey("Nombre") ? snap["Nombre"] : "?";
                    string precio = snap.ContainsKey("PrecioVenta") ? snap["PrecioVenta"] : "?";
                    return $"ID {id} (Nombre: {nombre}, PrecioVenta: {precio})";
                case "USUARIO":
                    string username = snap.ContainsKey("Username") ? snap["Username"] : "?";
                    string rol = snap.ContainsKey("Rol") ? snap["Rol"] : "?";
                    return $"ID {id} (Username: {username}, Rol: {rol})";
                case "PEDIDO":
                    string mesa = snap.ContainsKey("ID_Mesa") ? snap["ID_Mesa"] : "?";
                    string total = snap.ContainsKey("Total") ? snap["Total"] : "?";
                    return $"ID {id} (Mesa: {mesa}, Total: {total})";
                case "PAGO":
                    string monto = snap.ContainsKey("Monto") ? snap["Monto"] : "?";
                    return $"ID {id} (Monto: {monto})";
                case "DETALLE_PEDIDO":
                    string pedido = snap.ContainsKey("ID_Pedido") ? snap["ID_Pedido"] : "?";
                    string cantidad = snap.ContainsKey("Cantidad") ? snap["Cantidad"] : "?";
                    return $"ID {id} (Pedido: {pedido}, Cantidad: {cantidad})";
                case "MOVIMIENTO_STOCK":
                    string ingrediente = snap.ContainsKey("ID_Ingrediente") ? snap["ID_Ingrediente"] : "?";
                    string cant = snap.ContainsKey("Cantidad") ? snap["Cantidad"] : "?";
                    return $"ID {id} (Ingrediente: {ingrediente}, Cantidad: {cant})";
                default:
                    return $"ID {id}";
            }
        }

        // Valida DVV de DETALLE_PEDIDO agrupando Cantidad y Subtotal — un solo mensaje
        private List<string> ValidarDVVPorTablaDetalle(string tabla, int filasActuales, int erroresDVHCount)
        {
            bool cantidadOk = dvBLL.VerificarDVV(tabla, "Cantidad");
            bool subtotalOk = dvBLL.VerificarDVV(tabla, "Subtotal");
            if (cantidadOk && subtotalOk)
                return new List<string>();

            return ValidarDVVDetallado(tabla, "Cantidad", filasActuales, erroresDVHCount);
        }

        public void GuardarSnapshotCompleto()
        {
            try
            {
                auditoriaDAL.EliminarSnapshots("PLATO");
                auditoriaDAL.EliminarSnapshots("USUARIO");
                auditoriaDAL.EliminarSnapshots("PEDIDO");
                auditoriaDAL.EliminarSnapshots("PAGO");
                auditoriaDAL.EliminarSnapshots("DETALLE_PEDIDO");
                auditoriaDAL.EliminarSnapshots("MOVIMIENTO_STOCK");

                foreach (var p in platoDAL.ObtenerTodos())
                {
                    auditoriaDAL.GuardarSnapshot("PLATO", p.ID_Plato, "Nombre", p.Nombre);
                    auditoriaDAL.GuardarSnapshot("PLATO", p.ID_Plato, "Descripcion", p.Descripcion ?? "");
                    auditoriaDAL.GuardarSnapshot("PLATO", p.ID_Plato, "PrecioVenta", p.PrecioVenta.ToString());
                    auditoriaDAL.GuardarSnapshot("PLATO", p.ID_Plato, "ID_Categoria", p.ID_Categoria.ToString());
                    auditoriaDAL.GuardarSnapshot("PLATO", p.ID_Plato, "Disponible", p.Disponible ? "1" : "0");
                }

                foreach (var u in usuarioDAL.ObtenerTodos())
                {
                    auditoriaDAL.GuardarSnapshot("USUARIO", u.ID_Usuario, "Nombre", u.Nombre);
                    auditoriaDAL.GuardarSnapshot("USUARIO", u.ID_Usuario, "Apellido", u.Apellido);
                    auditoriaDAL.GuardarSnapshot("USUARIO", u.ID_Usuario, "Username", u.Username);
                    auditoriaDAL.GuardarSnapshot("USUARIO", u.ID_Usuario, "Mail", u.Mail);
                    auditoriaDAL.GuardarSnapshot("USUARIO", u.ID_Usuario, "Rol", u.Rol);
                    auditoriaDAL.GuardarSnapshot("USUARIO", u.ID_Usuario, "Activo", u.Activo ? "1" : "0");
                    auditoriaDAL.GuardarSnapshot("USUARIO", u.ID_Usuario, "IntentosFallidos", u.IntentosFallidos.ToString());
                }

                foreach (var p in pedidoDAL.ObtenerTodos())
                {
                    auditoriaDAL.GuardarSnapshot("PEDIDO", p.ID_Pedido, "ID_Mesa", p.ID_Mesa.ToString());
                    auditoriaDAL.GuardarSnapshot("PEDIDO", p.ID_Pedido, "ID_Usuario", p.ID_Usuario.ToString());
                    auditoriaDAL.GuardarSnapshot("PEDIDO", p.ID_Pedido, "Estado", p.Estado);
                    auditoriaDAL.GuardarSnapshot("PEDIDO", p.ID_Pedido, "Total", p.Total.ToString());
                }

                foreach (var p in pagoDAL.ObtenerTodos())
                {
                    auditoriaDAL.GuardarSnapshot("PAGO", p.ID_Pago, "ID_Pedido", p.ID_Pedido.ToString());
                    auditoriaDAL.GuardarSnapshot("PAGO", p.ID_Pago, "MetodoPago", p.MetodoPago);
                    auditoriaDAL.GuardarSnapshot("PAGO", p.ID_Pago, "Monto", p.Monto.ToString());
                    auditoriaDAL.GuardarSnapshot("PAGO", p.ID_Pago, "ID_Usuario", p.ID_Usuario.ToString());
                }

                foreach (var d in detalleDAL.ObtenerTodos())
                {
                    auditoriaDAL.GuardarSnapshot("DETALLE_PEDIDO", d.ID_Detalle, "ID_Pedido", d.ID_Pedido.ToString());
                    auditoriaDAL.GuardarSnapshot("DETALLE_PEDIDO", d.ID_Detalle, "ID_Plato", d.ID_Plato.ToString());
                    auditoriaDAL.GuardarSnapshot("DETALLE_PEDIDO", d.ID_Detalle, "Cantidad", d.Cantidad.ToString());
                    auditoriaDAL.GuardarSnapshot("DETALLE_PEDIDO", d.ID_Detalle, "PrecioUnitario", d.PrecioUnitario.ToString());
                    auditoriaDAL.GuardarSnapshot("DETALLE_PEDIDO", d.ID_Detalle, "Subtotal", d.Subtotal.ToString());
                }

                foreach (var m in movDAL.ObtenerTodos())
                {
                    auditoriaDAL.GuardarSnapshot("MOVIMIENTO_STOCK", m.ID_Movimiento, "ID_Ingrediente", m.ID_Ingrediente.ToString());
                    auditoriaDAL.GuardarSnapshot("MOVIMIENTO_STOCK", m.ID_Movimiento, "ID_Proveedor", m.ID_Proveedor.ToString());
                    auditoriaDAL.GuardarSnapshot("MOVIMIENTO_STOCK", m.ID_Movimiento, "Cantidad", m.Cantidad.ToString());
                    auditoriaDAL.GuardarSnapshot("MOVIMIENTO_STOCK", m.ID_Movimiento, "ID_Usuario", m.ID_Usuario.ToString());
                }
            }
            catch { }
        }

        public bool Restaurar(int idUsuario, string usernameUsuario, out string mensaje)
        {
            bool resultado = backupDAL.RestaurarUltimoBackup(out mensaje);
            if (resultado)
                bitacoraBLL.Registrar(idUsuario, usernameUsuario, 20,
                    "Restauración de backup por corrupción detectada en la base de datos.", 3);
            return resultado;
        }

        public bool RecalcularTodo(int idUsuario, string usernameUsuario)
        {
            try
            {
                usuarioDAL.RecalcularTodosDVH();
                pedidoDAL.RecalcularTodosDVH();
                detalleDAL.RecalcularTodosDVH();
                pagoDAL.RecalcularTodosDVH();
                platoDAL.RecalcularTodosDVH();
                movDAL.RecalcularTodosDVH();

                dvBLL.RecalcularDVV("USUARIO", "Activo");
                dvBLL.RecalcularDVV("USUARIO", "Rol");
                dvBLL.RecalcularDVV("PEDIDO", "Total");
                dvBLL.RecalcularDVV("DETALLE_PEDIDO", "Cantidad");
                dvBLL.RecalcularDVV("DETALLE_PEDIDO", "Subtotal");
                dvBLL.RecalcularDVV("PAGO", "Monto");
                dvBLL.RecalcularDVV("MOVIMIENTO_STOCK", "Cantidad");
                dvBLL.RecalcularDVV("PLATO", "PrecioVenta");

                dvDAL.ActualizarConteoFilas("USUARIO", usuarioDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("PEDIDO", pedidoDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("DETALLE_PEDIDO", detalleDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("PAGO", pagoDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("PLATO", platoDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("MOVIMIENTO_STOCK", movDAL.ObtenerTodos().Count);

                GuardarSnapshotCompleto();

                bitacoraBLL.Registrar(idUsuario, usernameUsuario, 20,
                    "Recálculo manual de dígitos verificadores por corrupción detectada.", 3);
                return true;
            }
            catch { return false; }
        }

        public bool GenerarBackupPreventivo(out string rutaArchivo)
        {
            try
            {
                dvDAL.ActualizarConteoFilas("USUARIO", usuarioDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("PEDIDO", pedidoDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("DETALLE_PEDIDO", detalleDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("PAGO", pagoDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("PLATO", platoDAL.ObtenerTodos().Count);
                dvDAL.ActualizarConteoFilas("MOVIMIENTO_STOCK", movDAL.ObtenerTodos().Count);
            }
            catch { }
            return backupDAL.GenerarBackup(out rutaArchivo);
        }
    }
}
