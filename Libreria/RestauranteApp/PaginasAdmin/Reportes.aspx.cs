using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RestauranteApp.Idiomas;


namespace RestauranteApp.PaginasAdmin
{
    public partial class Reportes : System.Web.UI.Page, ILanguageObserver
    {
        ReporteBLL reporteBLL = new ReporteBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);
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
            lblReportes.InnerText = manager.ObtenerTexto("Reportes");

            // Filtros
            lblFiltrarFecha.InnerText = manager.ObtenerTexto("FiltrarPorFecha");
            lblFechaDesde.InnerText = manager.ObtenerTexto("FechaDesde");
            lblFechaHasta.InnerText = manager.ObtenerTexto("FechaHasta");
            rfvFechaDesde.ErrorMessage = manager.ObtenerTexto("ErrorFechaDesdeObligatoria");
            rfvFechaHasta.ErrorMessage = manager.ObtenerTexto("ErrorFechaHastaObligatoria");

            // Botón
            btnGenerar.Text = manager.ObtenerTexto("GenerarReporte");
            // Mensaje de error
            if (lblMensaje.Visible)
            {
                lblMensaje.Text =
                    manager.ObtenerTexto("ErrorFechaDesdeMayor");
            }

            // Resumen de ventas
            lblTotalVendido.InnerText = manager.ObtenerTexto("TotalVendido");
            lblPagosEfectivo.InnerText = manager.ObtenerTexto("PagosEfectivo");
            lblPagosTarjeta.InnerText = manager.ObtenerTexto("PagosTarjeta");


            // Platos más vendidos
            lblPlatosMasVendidos.InnerText = manager.ObtenerTexto("PlatosMasVendidos");

            // Encabezados del GridView
            if (gvPlatosVendidos.HeaderRow != null)
            {
                gvPlatosVendidos.HeaderRow.Cells[0].Text = manager.ObtenerTexto("Plato");
                gvPlatosVendidos.HeaderRow.Cells[1].Text = manager.ObtenerTexto("UnidadesVendidas");
                gvPlatosVendidos.HeaderRow.Cells[2].Text = manager.ObtenerTexto("TotalRecaudado");
            }
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();

            DateTime desde = Convert.ToDateTime(txtFechaDesde.Text);
            DateTime hasta = Convert.ToDateTime(txtFechaHasta.Text).AddHours(23).AddMinutes(59);

            if (desde > hasta)
            {
                lblMensaje.Text = manager.ObtenerTexto("ErrorFechaDesdeMayor");
                lblMensaje.CssClass = "alert alert-danger d-block mb-3";
                lblMensaje.Visible = true;
                pnlResultados.Visible = false;
                return;
            }

            // Totales
            decimal totalVentas = reporteBLL.ObtenerTotalVentas(desde, hasta);
            decimal totalEfectivo = reporteBLL.ObtenerTotalPorMetodo(desde, hasta, "Efectivo");
            decimal totalTarjeta = reporteBLL.ObtenerTotalPorMetodo(desde, hasta, "Tarjeta");

            lblTotalVentas.Text = "$" + totalVentas.ToString("N2");
            lblTotalEfectivo.Text = "$" + totalEfectivo.ToString("N2");
            lblTotalTarjeta.Text = "$" + totalTarjeta.ToString("N2");

            // Platos vendidos
            gvPlatosVendidos.DataSource = reporteBLL.ObtenerPlatosVendidos(desde, hasta);
            gvPlatosVendidos.DataBind();

            pnlResultados.Visible = true;
            lblMensaje.Visible = false;

            BitacoraBLL bitacoraBLL = new BitacoraBLL();
            Usuario usuarioActual = (Usuario)Session["Usuario"];
            bitacoraBLL.Registrar(usuarioActual.ID_Usuario, usuarioActual.Username, 19,
                "Generación de reporte desde " + txtFechaDesde.Text + " hasta " + txtFechaHasta.Text);

        }
    }
}