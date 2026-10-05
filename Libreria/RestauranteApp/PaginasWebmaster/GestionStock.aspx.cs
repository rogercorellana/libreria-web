using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using RestauranteApp.Idiomas;

namespace RestauranteApp.PaginasWebmaster
{
    public partial class GestionStock : System.Web.UI.Page, ILanguageObserver
    {
        IngredienteBLL ingredienteBLL = new IngredienteBLL();
        ProveedorBLL proveedorBLL = new ProveedorBLL();
        MovimientoStockBLL movimientoStockBLL = new MovimientoStockBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);

            if (!IsPostBack)
            {
                CargarIngredientes();
                CargarProveedores();
                CargarStock();
                CargarAlertaStockBajo();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ActualizarIdioma();
        }

        public void ActualizarIdioma()
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();

            // Título principal
            lblGestionStock.InnerText = manager.ObtenerTexto("GestionStock");

            // Formulario
            lblRegistrarIngreso.InnerText = manager.ObtenerTexto("RegistrarIngresoMercaderia");

            // Etiquetas
            lblIngrediente.InnerText = manager.ObtenerTexto("Ingrediente");
            lblProveedor.InnerText = manager.ObtenerTexto("Proveedor");
            lblCantidad.InnerText = manager.ObtenerTexto("Cantidad");

            // Placeholder
            txtCantidad.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderCantidad");

            // Validador
            rfvCantidad.ErrorMessage = manager.ObtenerTexto("ErrorCantidadObligatoria");

            // Botón
            btnRegistrar.Text = manager.ObtenerTexto("RegistrarIngreso");

            // Alerta de stock bajo
            lblIngredientesStockBajo.InnerText = manager.ObtenerTexto("IngredientesStockBajo");

            // Stock actual
            lblStockActualIngredientes.InnerText = manager.ObtenerTexto("StockActualIngredientes");

            // Encabezados del GridView de stock bajo
            if (gvStockBajo.HeaderRow != null)
            {
                gvStockBajo.HeaderRow.Cells[0].Text = manager.ObtenerTexto("Ingrediente");
                gvStockBajo.HeaderRow.Cells[1].Text = manager.ObtenerTexto("StockActual");
                gvStockBajo.HeaderRow.Cells[2].Text = manager.ObtenerTexto("StockMinimo");
                gvStockBajo.HeaderRow.Cells[3].Text = manager.ObtenerTexto("Unidad");
            }

            // Encabezados del GridView de stock actual
            if (gvStock.HeaderRow != null)
            {
                gvStock.HeaderRow.Cells[0].Text = manager.ObtenerTexto("Ingrediente");
                gvStock.HeaderRow.Cells[1].Text = manager.ObtenerTexto("StockActual");
                gvStock.HeaderRow.Cells[2].Text = manager.ObtenerTexto("StockMinimo");
                gvStock.HeaderRow.Cells[3].Text = manager.ObtenerTexto("Unidad");
            }
            foreach (GridViewRow fila in gvStockBajo.Rows)
            {
                fila.Cells[3].Text =
                    TraducirUnidad(fila.Cells[3].Text, manager);
            }
            foreach (GridViewRow fila in gvStock.Rows)
            {
                fila.Cells[3].Text =
                    TraducirUnidad(fila.Cells[3].Text, manager);
            }
        }

        private string TraducirUnidad(string unidad, LanguageManager manager)
        {
            switch (unidad)
            {
                case "kg":
                    return manager.ObtenerTexto("UnidadKg");
                case "g":
                    return manager.ObtenerTexto("UnidadGramos");
                case "litros":
                case "liters":
                    return manager.ObtenerTexto("UnidadLitros");
                case "ml":
                    return manager.ObtenerTexto("UnidadMl");
                case "unidades":
                case "units":
                    return manager.ObtenerTexto("UnidadUnidades");
                default:
                    return unidad;
            }
        }

        private void CargarIngredientes()
        {
            ddlIngrediente.DataSource = ingredienteBLL.ObtenerTodos();
            ddlIngrediente.DataBind();
        }

        private void CargarProveedores()
        {
            ddlProveedor.DataSource = proveedorBLL.ObtenerTodos();
            ddlProveedor.DataBind();
        }

        private void CargarStock()
        {
            gvStock.DataSource = ingredienteBLL.ObtenerTodos();
            gvStock.DataBind();
        }

        private void CargarAlertaStockBajo()
        {
            var bajoStock = ingredienteBLL.ObtenerBajoStock();
            if (bajoStock.Count > 0)
            {
                pnlAlertaStock.Visible = true;
                gvStockBajo.DataSource = bajoStock;
                gvStockBajo.DataBind();
            }
            else
            {
                pnlAlertaStock.Visible = false;
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            int idIngrediente = Convert.ToInt32(ddlIngrediente.SelectedValue);
            int idProveedor = Convert.ToInt32(ddlProveedor.SelectedValue);
            decimal cantidad = Convert.ToDecimal(txtCantidad.Text.Trim());
            int idUsuario = ((Usuario)Session["Usuario"]).ID_Usuario;

            bool resultado = movimientoStockBLL.RegistrarIngreso(
                idIngrediente, idProveedor, cantidad, idUsuario);

            BitacoraBLL bitacoraBLL = new BitacoraBLL();
            bitacoraBLL.Registrar(idUsuario, ((Usuario)Session["Usuario"]).Username, 17,
                "Ingreso de stock - Ingrediente ID: " + idIngrediente + " - Cantidad: " + cantidad);


            MostrarMensaje(resultado,
                "Ingreso de mercadería registrado correctamente.",
                "Error al registrar el ingreso.");

            txtCantidad.Text = "";
            CargarStock();
            CargarAlertaStockBajo();
        }

        private void MostrarMensaje(bool exito, string mensajeExito, string mensajeError)
        {
            lblMensaje.Visible = true;
            lblMensaje.Text = exito ? mensajeExito : mensajeError;
            lblMensaje.CssClass = exito ? "alert alert-success d-block mb-3"
                                        : "alert alert-danger d-block mb-3";
        }
    }
}