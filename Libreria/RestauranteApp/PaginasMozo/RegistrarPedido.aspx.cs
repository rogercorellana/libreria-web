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
    public partial class RegistrarPedido : System.Web.UI.Page, ILanguageObserver
    {
        PlatoBLL platoBLL = new PlatoBLL();
        PedidoBLL pedidoBLL = new PedidoBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);

            // Si no hay mesa seleccionada, volver al mapa
            if (Session["IDMesaSeleccionada"] == null)
            {
                Response.Redirect("~/PaginasMozo/MapaMesas.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblMesa.Text = "Nro. " + Session["IDMesaSeleccionada"].ToString();
                CargarPlatos();
                ActualizarDetalle();
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
            lblRegistrarPedido.InnerText = manager.ObtenerTexto("RegistrarPedido");

            // Información de la mesa
            lblMesaTexto.InnerText = manager.ObtenerTexto("MesaTexto");

            // Encabezado agregar plato
            lblAgregarPlato.InnerText = manager.ObtenerTexto("AgregarPlatoPedido");

            // Campos del formulario
            lblPlato.InnerText = manager.ObtenerTexto("Plato");

            lblCantidad.InnerText = manager.ObtenerTexto("Cantidad");

            // Botón agregar
            btnAgregarPlato.Text = manager.ObtenerTexto("AgregarPlato");

            // Detalle del pedido
            lblDetallePedido.InnerText = manager.ObtenerTexto("DetallePedido");
            
            // Encabezados del detalle del pedido
            if (gvDetalle.HeaderRow != null)
            {
                gvDetalle.HeaderRow.Cells[1].Text = manager.ObtenerTexto("Plato");
                gvDetalle.HeaderRow.Cells[2].Text = manager.ObtenerTexto("Cantidad");
                gvDetalle.HeaderRow.Cells[3].Text = manager.ObtenerTexto("Precio");
                gvDetalle.HeaderRow.Cells[4].Text = manager.ObtenerTexto("Subtotal");
                gvDetalle.HeaderRow.Cells[5].Text = manager.ObtenerTexto("Acciones");
            }
            foreach (GridViewRow fila in gvDetalle.Rows)
            {
                Button btnEliminar = (Button)fila.FindControl("btnEliminar");

                if (btnEliminar != null)
                {
                    btnEliminar.Text = manager.ObtenerTexto("Eliminar");
                }
            }

            // Total
            lblTotalTexto.InnerText = manager.ObtenerTexto("Total");

            // Botones
            btnConfirmarPedido.Text = manager.ObtenerTexto("ConfirmarPedido");
            btnSolicitarCuenta.Text = manager.ObtenerTexto("SolicitarCuenta");
            btnVolver.Text = manager.ObtenerTexto("VolverMapaMesas");
        }

        private void CargarPlatos()
        {
            ddlPlatos.DataSource = platoBLL.ObtenerDisponibles();
            ddlPlatos.DataBind();
        }

        private void ActualizarDetalle()
        {
            int idMesa = Convert.ToInt32(Session["IDMesaSeleccionada"]);
            List<DetallePedido> detalle = pedidoBLL.ObtenerDetallePorMesa(idMesa);

            if (detalle != null && detalle.Count > 0)
            {
                gvDetalle.DataSource = detalle;
                gvDetalle.DataBind();
                decimal total = pedidoBLL.ObtenerTotalPorMesa(idMesa);
                lblTotal.Text = "$" + total.ToString("N2");
            }
            else
            {
                gvDetalle.DataSource = null;
                gvDetalle.DataBind();
                lblTotal.Text = "$0.00";
            }
        }

        protected void gvDetalle_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EliminarDetalle")
            {
                int idDetalle = Convert.ToInt32(e.CommandArgument);
                int idMesa = Convert.ToInt32(Session["IDMesaSeleccionada"]);

                bool resultado = pedidoBLL.EliminarDetalle(idDetalle, idMesa);

                if (resultado)
                {
                    BitacoraBLL bitacoraBLL = new BitacoraBLL();
                    int idUsuario = ((Usuario)Session["Usuario"]).ID_Usuario;
                    string username = ((Usuario)Session["Usuario"]).Username;
                    bitacoraBLL.Registrar(idUsuario, username, 11,
                        "Eliminación de plato del pedido en mesa: " + idMesa);
                    ActualizarDetalle();
                }
                else
                {
                    LanguageManager manager = LanguageManager.ObtenerInstancia();
                    MostrarMensaje(false, "", manager.ObtenerTexto("ErrorEliminarPlatoPedido"));
                }
            }
        }
        protected void btnAgregarPlato_Click(object sender, EventArgs e)
        {
            int idMesa = Convert.ToInt32(Session["IDMesaSeleccionada"]);
            int idUsuario = ((Usuario)Session["Usuario"]).ID_Usuario;
            int idPlato = Convert.ToInt32(ddlPlatos.SelectedValue);
            int cantidad = Convert.ToInt32(txtCantidad.Text.Trim());

            bool resultado = pedidoBLL.AgregarDetalle(idMesa, idUsuario, idPlato, cantidad);

            BitacoraBLL bitacoraBLL = new BitacoraBLL();
            string username = ((Usuario)Session["Usuario"]).Username;
            bitacoraBLL.Registrar(idUsuario, username, 10,
                "Registro de pedido en mesa: " + Session["IDMesaSeleccionada"].ToString());


            if (resultado)
            {
                LanguageManager manager = LanguageManager.ObtenerInstancia();

                MostrarMensaje(
                    true,
                    manager.ObtenerTexto("PlatoAgregadoCorrectamente"),
                    ""
                );

                txtCantidad.Text = "";
                ActualizarDetalle();
            }
            else
            {
                MostrarMensaje(false, "", "Error al agregar el plato.");
            }
        }

        protected void btnConfirmarPedido_Click(object sender, EventArgs e)
        {
            int idMesa = Convert.ToInt32(Session["IDMesaSeleccionada"]);
            List<DetallePedido> detalle = pedidoBLL.ObtenerDetallePorMesa(idMesa);

            if (detalle.Count == 0)
            {
                MostrarMensaje(false, "", "Debe agregar al menos un plato antes de confirmar.");
                return;
            }

            Response.Redirect("~/PaginasMozo/MapaMesas.aspx");
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PaginasMozo/MapaMesas.aspx");
        }

        protected void btnSolicitarCuenta_Click(object sender, EventArgs e)
        {
            int idMesa = Convert.ToInt32(Session["IDMesaSeleccionada"]);

            List<DetallePedido> detalle = pedidoBLL.ObtenerDetallePorMesa(idMesa);

            BitacoraBLL bitacoraBLL = new BitacoraBLL();
            int idUsuario = ((Usuario)Session["Usuario"]).ID_Usuario;
            string username = ((Usuario)Session["Usuario"]).Username;
            bitacoraBLL.Registrar(idUsuario, username, 13,
                "Solicitud de cuenta en mesa: " + Session["IDMesaSeleccionada"].ToString());


            if (detalle.Count == 0)
            {
                LanguageManager manager = LanguageManager.ObtenerInstancia();
                MostrarMensaje(false, "", manager.ObtenerTexto("ErrorSolicitarCuentaSinPedidos"));
                return;
            }

            MesaBLL mesaBLL = new MesaBLL();
            mesaBLL.SolicitarCuenta(idMesa);

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