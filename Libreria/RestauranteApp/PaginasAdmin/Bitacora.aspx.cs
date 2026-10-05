using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using RestauranteApp.Idiomas;


namespace RestauranteApp.PaginasAdmin
{
    public partial class Bitacora : System.Web.UI.Page, ILanguageObserver
    {
        BitacoraBLL bitacoraBLL = new BitacoraBLL();
        UsuarioBLL usuarioBLL = new UsuarioBLL();
        ActividadBLL actividadBLL = new ActividadBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);

            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            Usuario usuario = (Usuario)Session["Usuario"];
            if (usuario.Rol != "ROL-03")
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarFiltros();
                CargarBitacora();
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
            lblBitacora.InnerText = manager.ObtenerTexto("BitacoraSistema");

            // Filtros
            lblFiltros.InnerText = manager.ObtenerTexto("Filtros");
            lblUsuario.InnerText = manager.ObtenerTexto("Usuario");
            lblTipoActividad.InnerText = manager.ObtenerTexto("TipoActividad");
            lblFechaDesde.InnerText = manager.ObtenerTexto("FechaDesde");
            lblFechaHasta.InnerText = manager.ObtenerTexto("FechaHasta");
            if (ddlUsuario.Items.Count > 0)
            {
                ddlUsuario.Items[0].Text = manager.ObtenerTexto("Todos");
            }

            if (ddlActividad.Items.Count > 0)
            {
                ddlActividad.Items[0].Text = manager.ObtenerTexto("Todas");
            }

            // Botones
            btnFiltrar.Text = manager.ObtenerTexto("Filtrar");
            btnLimpiar.Text = manager.ObtenerTexto("LimpiarFiltros");

            // Grilla
            lblRegistroAcciones.InnerText = manager.ObtenerTexto("RegistroAcciones");

            // Encabezados del GridView
            if (gvBitacora.HeaderRow != null)
            {
                gvBitacora.HeaderRow.Cells[0].Text = manager.ObtenerTexto("ID");
                gvBitacora.HeaderRow.Cells[1].Text = manager.ObtenerTexto("FechaHora");
                gvBitacora.HeaderRow.Cells[2].Text = manager.ObtenerTexto("Usuario");
                gvBitacora.HeaderRow.Cells[3].Text = manager.ObtenerTexto("Actividad");
                gvBitacora.HeaderRow.Cells[4].Text = manager.ObtenerTexto("Descripcion");
                gvBitacora.HeaderRow.Cells[5].Text = manager.ObtenerTexto("Criticidad");
            }
            foreach (GridViewRow fila in gvBitacora.Rows)
            {
                Label lblNivel = (Label)fila.FindControl("lblNivel");

                if (lblNivel == null)
                    continue;

                if (lblNivel.Text.Contains("Nivel 1") ||
                    lblNivel.Text.Contains("Level 1"))
                {
                    lblNivel.Text = manager.ObtenerTexto("NivelInformativo");
                }
                else if (lblNivel.Text.Contains("Nivel 2") ||
                         lblNivel.Text.Contains("Level 2"))
                {
                    lblNivel.Text = manager.ObtenerTexto("NivelAdvertencia");
                }
                else if (lblNivel.Text.Contains("Nivel 3") ||
                         lblNivel.Text.Contains("Level 3"))
                {
                    lblNivel.Text = manager.ObtenerTexto("NivelCritico");
                }
            }
        }

        private void CargarFiltros()
        {
            var usuarios = usuarioBLL.ObtenerTodos();
            usuarios.Insert(0, new Usuario { ID_Usuario = 0, Username = "Todos" });
            ddlUsuario.DataSource = usuarios;
            ddlUsuario.DataBind();

            var actividades = actividadBLL.ObtenerTodas();
            actividades.Insert(0, new Actividad { ID_Actividad = 0, TipoActividad = "Todas" });
            ddlActividad.DataSource = actividades;
            ddlActividad.DataBind();
        }

        private void CargarBitacora()
        {
            int? idUsuario = ddlUsuario.SelectedValue == "0" ? (int?)null : Convert.ToInt32(ddlUsuario.SelectedValue);
            int? idActividad = ddlActividad.SelectedValue == "0" ? (int?)null : Convert.ToInt32(ddlActividad.SelectedValue);
            DateTime? desde = string.IsNullOrEmpty(txtFechaDesde.Text) ? (DateTime?)null : Convert.ToDateTime(txtFechaDesde.Text);
            DateTime? hasta = string.IsNullOrEmpty(txtFechaHasta.Text) ? (DateTime?)null : Convert.ToDateTime(txtFechaHasta.Text).AddHours(23).AddMinutes(59);

            gvBitacora.DataSource = bitacoraBLL.ObtenerFiltrado(idUsuario, desde, hasta, idActividad);
            gvBitacora.DataBind();
        }

        protected void gvBitacora_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();

            if (e.Row.RowType != DataControlRowType.DataRow) return;

            BE.Bitacora bitacora = (BE.Bitacora)e.Row.DataItem;
            Label lblNivel = (Label)e.Row.FindControl("lblNivel");
            if (lblNivel == null) return;

            /*switch (bitacora.NivelCriticidad)
            {
                case 1:
                    lblNivel.Text = "● Nivel 1 — Informativo";
                    lblNivel.ForeColor = System.Drawing.Color.FromArgb(25, 135, 84);
                    break;
                case 2:
                    lblNivel.Text = "● Nivel 2 — Advertencia";
                    lblNivel.ForeColor = System.Drawing.Color.FromArgb(255, 153, 0);
                    break;
                case 3:
                    lblNivel.Text = "● Nivel 3 — Crítico";
                    lblNivel.ForeColor = System.Drawing.Color.FromArgb(220, 53, 69);
                    break;
            }*/
            switch (bitacora.NivelCriticidad)
            {
                case 1:
                    lblNivel.Text = manager.ObtenerTexto("NivelInformativo");
                    lblNivel.ForeColor = System.Drawing.Color.FromArgb(25, 135, 84);
                    break;
                case 2:
                    lblNivel.Text = manager.ObtenerTexto("NivelAdvertencia");
                    lblNivel.ForeColor = System.Drawing.Color.FromArgb(255, 153, 0);
                    break;
                case 3:
                    lblNivel.Text = manager.ObtenerTexto("NivelCritico");
                    lblNivel.ForeColor = System.Drawing.Color.FromArgb(220, 53, 69);
                    break;
            }

        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            CargarBitacora();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            ddlUsuario.SelectedIndex = 0;
            ddlActividad.SelectedIndex = 0;
            txtFechaDesde.Text = "";
            txtFechaHasta.Text = "";
            CargarBitacora();
        }
    }
}