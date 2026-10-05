using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using RestauranteApp.Idiomas;

namespace RestauranteApp.PaginasMozo
{
    public partial class CerrarCuenta : System.Web.UI.Page, ILanguageObserver
    {
        PedidoBLL pedidoBLL = new PedidoBLL();
        PagoBLL pagoBLL = new PagoBLL();
        MesaBLL mesaBLL = new MesaBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);

            if (Session["IDMesaSeleccionada"] == null)
            {
                Response.Redirect("~/PaginasMozo/MapaMesas.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblMesa.Text = "Nro. " + Session["IDMesaSeleccionada"].ToString();
                CargarDetalle();
            }
        }

        public void ActualizarIdioma()
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();

            // Título principal
            lblCerrarCuenta.InnerText = manager.ObtenerTexto("CerrarCuenta");

            // Información de la mesa
            lblMesaTexto.InnerText = manager.ObtenerTexto("MesaTexto");

            // Detalle del pedido
            lblDetallePedido.InnerText = manager.ObtenerTexto("DetallePedido");

            // Encabezados del detalle
            if (gvDetalle.HeaderRow != null)
            {
                gvDetalle.HeaderRow.Cells[0].Text = manager.ObtenerTexto("Plato");
                gvDetalle.HeaderRow.Cells[1].Text = manager.ObtenerTexto("Cantidad");
                gvDetalle.HeaderRow.Cells[2].Text = manager.ObtenerTexto("Precio");
                gvDetalle.HeaderRow.Cells[3].Text = manager.ObtenerTexto("Subtotal");
            }

            // Total
            lblTotalTexto.InnerText = manager.ObtenerTexto("Total");

            // Método de pago
            lblMetodoPago.InnerText = manager.ObtenerTexto("MetodoPago");

            // Opciones de pago
            ddlMetodoPago.Items[0].Text = manager.ObtenerTexto("Efectivo");
            ddlMetodoPago.Items[1].Text = manager.ObtenerTexto("Tarjeta");

            // Botones
            btnConfirmarPago.Text = manager.ObtenerTexto("ConfirmarPago");
            btnVolver.Text = manager.ObtenerTexto("VolverMapaMesas");
            btnLiberarMesa.Text = manager.ObtenerTexto("LiberarMesa");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ActualizarIdioma();
        }



        private void CargarDetalle()
        {
            int idMesa = Convert.ToInt32(Session["IDMesaSeleccionada"]);
            List<DetallePedido> detalle = pedidoBLL.ObtenerDetallePorMesa(idMesa);

            if (detalle == null || detalle.Count == 0)
            {
                // No hay pedido activo — mostrar opción de liberar mesa
                gvDetalle.DataSource = null;
                gvDetalle.DataBind();
                lblTotal.Text = "$0.00";
                btnConfirmarPago.Visible = false;
                ddlMetodoPago.Visible = false;
                btnLiberarMesa.Visible = true;
                MostrarMensaje(false, "", "No hay pedidos activos para esta mesa. Podés liberar la mesa directamente.");
            }
            else
            {
                gvDetalle.DataSource = detalle;
                gvDetalle.DataBind();
                decimal total = pedidoBLL.ObtenerTotalPorMesa(idMesa);
                lblTotal.Text = "$" + total.ToString("N2");
                btnLiberarMesa.Visible = false;
            }
        }

        protected void btnConfirmarPago_Click(object sender, EventArgs e)
        {
            int idMesa = Convert.ToInt32(Session["IDMesaSeleccionada"]);
            int idUsuario = ((Usuario)Session["Usuario"]).ID_Usuario;
            string metodoPago = ddlMetodoPago.SelectedValue;

            bool resultado = pagoBLL.RegistrarPago(idMesa, idUsuario, metodoPago);

            if (resultado)
            {
                Session.Remove("IDMesaSeleccionada");
                Session.Remove("EstadoMesaSeleccionada");
                Response.Redirect("~/PaginasMozo/MapaMesas.aspx");
            }
            else
            {
                MostrarMensaje(false, "", "Error al registrar el pago. Intente nuevamente.");
            }
        }

        protected void btnLiberarMesa_Click(object sender, EventArgs e)
        {
            int idMesa = Convert.ToInt32(Session["IDMesaSeleccionada"]);
            mesaBLL.LiberarMesa(idMesa);
            Session.Remove("IDMesaSeleccionada");
            Session.Remove("EstadoMesaSeleccionada");
            Response.Redirect("~/PaginasMozo/MapaMesas.aspx");
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PaginasMozo/MapaMesas.aspx");
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