using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BE;
using BLL;
using System.Web.UI.HtmlControls;

namespace RestauranteApp.PaginasWebmaster
{
    public partial class AlertaIntegridad : Page
    {
        IntegridadBLL integridadBLL = new IntegridadBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Sin sesión => login
            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // El mozo no puede ver esta página
            Usuario usuario = (Usuario)Session["Usuario"];
            if (usuario.Rol == "ROL-01")
            {
                Response.Redirect("~/PaginasMozo/MapaMesas.aspx");
                return;
            }

            if (!IsPostBack)
                MostrarErrores();
        }

        private void MostrarErrores()
        {
            List<string> errores = Session["ErroresIntegridad"] as List<string>;
            if (errores == null || errores.Count == 0) return;

            foreach (string error in errores)
            {
                HtmlGenericControl li = new HtmlGenericControl("li");
                li.InnerText = error;
                listaErrores.Controls.Add(li);
            }
        }

        // ============================================
        // OPCIÓN 1 — RESTAURAR
        // ============================================
        protected void btnRestaurar_Click(object sender, EventArgs e)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            string mensaje;
            bool resultado = integridadBLL.Restaurar(usuario.ID_Usuario, usuario.Username, out mensaje);

            if (resultado)
            {
                Session["ErroresIntegridad"] = null;
                lblMensaje.Visible = true;
                lblMensaje.CssClass = "alert alert-success d-block mt-3";
                lblMensaje.Text = "Restauración completada: " + mensaje + " Redirigiendo...";

                string url = usuario.Rol == "ROL-03"
                    ? "~/PaginasAdmin/ABMUsuarios.aspx"
                    : "~/PaginasWebmaster/ABMPlatos.aspx";

                Response.AddHeader("Refresh", "3;url=" + ResolveUrl(url));
            }
            else
            {
                lblMensaje.Visible = true;
                lblMensaje.CssClass = "alert alert-danger d-block mt-3";
                lblMensaje.Text = "Error al restaurar: " + mensaje;
            }
        }

        // ============================================
        // OPCIÓN 2 — RECALCULAR
        // ============================================
        protected void btnRecalcular_Click(object sender, EventArgs e)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            bool resultado = integridadBLL.RecalcularTodo(usuario.ID_Usuario, usuario.Username);

            if (resultado)
            {
                Session["ErroresIntegridad"] = null;
                lblMensaje.Visible = true;
                lblMensaje.CssClass = "alert alert-success d-block mt-3";
                lblMensaje.Text = "Dígitos verificadores recalculados correctamente. " +
                    "La intervención quedó registrada en la bitácora. Redirigiendo...";

                string url = usuario.Rol == "ROL-03"
                    ? "~/PaginasAdmin/ABMUsuarios.aspx"
                    : "~/PaginasWebmaster/ABMPlatos.aspx";

                Response.AddHeader("Refresh", "3;url=" + ResolveUrl(url));
            }
            else
            {
                lblMensaje.Visible = true;
                lblMensaje.CssClass = "alert alert-danger d-block mt-3";
                lblMensaje.Text = "Error al recalcular los dígitos verificadores. Contacte al administrador.";
            }
        }

        // ============================================
        // OPCIÓN 3 — CANCELAR / CONTINUAR
        // ============================================
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session["ErroresIntegridad"] = null;

            btnRestaurar.Enabled = false;
            btnRecalcular.Enabled = false;
            btnCancelar.Enabled = false;

            lblMensaje.Visible = true;
            lblMensaje.CssClass = "alert alert-warning d-block mt-3";
            lblMensaje.Text = "La integridad de la base de datos no fue resuelta. " +
                "Contacte al administrador del sistema antes de continuar operando.";
        }
    }
}