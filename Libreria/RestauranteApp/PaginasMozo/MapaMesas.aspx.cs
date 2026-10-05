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
    public partial class MapaMesas : System.Web.UI.Page, ILanguageObserver
    {
        MesaBLL mesaBLL = new MesaBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);

            if (!IsPostBack)
                CargarMesas();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ActualizarIdioma();
        }

        private void CargarMesas()
        {
            gvMesas.DataSource = mesaBLL.ObtenerTodas();
            gvMesas.DataBind();
            pnlAcciones.Visible = false;
        }

        public void ActualizarIdioma()
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();

            // Título principal
            lblMapaMesas.InnerText = manager.ObtenerTexto("MapaMesas");

            // Encabezado de la grilla
            lblEstadoMesas.InnerText = manager.ObtenerTexto("EstadoMesas");

            // Panel de acciones
            lblMesaSeleccionadaTexto.InnerText = manager.ObtenerTexto("MesaSeleccionada");

            // Botones
            btnAbrirMesa.Text = manager.ObtenerTexto("AbrirMesa");
            btnRegistrarPedido.Text = manager.ObtenerTexto("RegistrarPedido");
            btnCerrarCuenta.Text = manager.ObtenerTexto("CerrarCuenta");

            // Encabezados de la grilla
            if (gvMesas.HeaderRow != null)
            {
                gvMesas.HeaderRow.Cells[0].Text = manager.ObtenerTexto("ID");
                gvMesas.HeaderRow.Cells[1].Text = manager.ObtenerTexto("NumeroMesa");
                gvMesas.HeaderRow.Cells[2].Text = manager.ObtenerTexto("Capacidad");
                gvMesas.HeaderRow.Cells[3].Text = manager.ObtenerTexto("Estado");
                gvMesas.HeaderRow.Cells[4].Text = manager.ObtenerTexto("Acciones");
            }
            foreach (GridViewRow fila in gvMesas.Rows)
            {
                fila.Cells[3].Text = TraducirEstado(fila.Cells[3].Text, manager);
            }

            // Botones Seleccionar de la grilla
            foreach (GridViewRow fila in gvMesas.Rows)
            {
                Button btnSeleccionar = (Button)fila.FindControl("btnSeleccionar");

                if (btnSeleccionar != null)
                {
                    btnSeleccionar.Text = manager.ObtenerTexto("Seleccionar");
                }
            }
        }

        private string TraducirEstado(string estado, LanguageManager manager)
        {
            switch (estado)
            {
                case "Ocupada":
                case "Occupied":
                    return manager.ObtenerTexto("EstadoOcupada");

                case "Libre":
                case "Available":
                    return manager.ObtenerTexto("EstadoLibre");

                case "EsperandoCuenta":
                case "Esperando cuenta":
                case "Waiting for account":
                    return manager.ObtenerTexto("EstadoEsperandoCuenta");

                default:
                    return estado;
            }
        }

        protected void gvMesas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Seleccionar")
            {
                int idMesa = Convert.ToInt32(e.CommandArgument);
                Mesa mesa = mesaBLL.ObtenerPorID(idMesa);

                if (mesa != null)
                {
                    Session["IDMesaSeleccionada"] = idMesa;
                    Session["EstadoMesaSeleccionada"] = mesa.Estado;

                    lblMesaSeleccionada.Text = "Nro. " + mesa.NumeroMesa +
                                              " — Estado: " + mesa.Estado;
                    pnlAcciones.Visible = true;

                    // Mostrar solo el botón correspondiente al estado
                    btnAbrirMesa.Visible = (mesa.Estado == "Libre");
                    btnRegistrarPedido.Visible = (mesa.Estado == "Ocupada");
                    btnCerrarCuenta.Visible = (mesa.Estado == "EsperandoCuenta");
                }
            }
        }

        protected void btnAbrirMesa_Click(object sender, EventArgs e)
        {
            int idMesa = Convert.ToInt32(Session["IDMesaSeleccionada"]);
            int idUsuario = ((Usuario)Session["Usuario"]).ID_Usuario;
            BitacoraBLL bitacoraBLL = new BitacoraBLL();
            bitacoraBLL.Registrar(idUsuario, ((Usuario)Session["Usuario"]).Username, 9,
                "Apertura de mesa Nro. " + Session["IDMesaSeleccionada"].ToString());

            mesaBLL.AbrirMesa(idMesa, idUsuario);

            lblMensaje.Text = "Mesa abierta correctamente.";
            lblMensaje.CssClass = "alert alert-success d-block mb-3";
            lblMensaje.Visible = true;
            CargarMesas();
        }

        protected void btnRegistrarPedido_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PaginasMozo/RegistrarPedido.aspx");
        }

        protected void btnCerrarCuenta_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PaginasMozo/CerrarCuenta.aspx");
        }
    }
}