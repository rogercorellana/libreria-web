using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using RestauranteApp.Idiomas;



namespace RestauranteApp
{
    public partial class SiteMaster : System.Web.UI.MasterPage, ILanguageObserver
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);
            if (!IsPostBack)
            {
                ddlIdioma.SelectedValue = manager.IdiomaActual;
            }

            lblUsuario.Text = manager.ObtenerTexto("Usuario") + ": " + Session["Username"].ToString();
            lblRol.Text = " | " + manager.ObtenerTexto("Rol") + ": " + Session["Rol"].ToString();
            string rol = Session["Rol"].ToString();

            if (rol == "ROL-01")
                pnlMenuMozo.Visible = true;
            else if (rol == "ROL-02")
                pnlMenuWebmaster.Visible = true;
            else if (rol == "ROL-03")
                pnlMenuAdmin.Visible = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ActualizarIdioma();
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            if (Session["Usuario"] != null)
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                BitacoraBLL bitacoraBLL = new BitacoraBLL();
                IntegridadBLL integridadBLL = new IntegridadBLL();

                // Validar integridad antes de generar el backup
                List<string> erroresWebmaster = integridadBLL.ValidarIntegridadWebmaster();
                List<string> erroresAdmin = integridadBLL.ValidarIntegridadAdmin();
                bool hayCorrupcion = erroresWebmaster.Count > 0 || erroresAdmin.Count > 0;

                // Siempre actualizar el snapshot al cerrar sesión
                // para que los cambios legítimos (pedidos, pagos) queden registrados
                integridadBLL.GuardarSnapshotCompleto();

                if (!hayCorrupcion)
                {
                    string rutaBackup;
                    integridadBLL.GenerarBackupPreventivo(out rutaBackup);
                    bitacoraBLL.Registrar(usuario.ID_Usuario, usuario.Username, 2,
                        "Cierre de sesión. Backup generado correctamente.", 1);
                }
                else
                {
                    bitacoraBLL.Registrar(usuario.ID_Usuario, usuario.Username, 2,
                        "Cierre de sesión. Backup NO generado: se detectó corrupción en la base de datos.", 3);
                }
            }
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect("~/Login.aspx");
        }

        public void ActualizarIdioma()
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            lblUsuario.Text = manager.ObtenerTexto("Usuario") + ": " +
                  Session["Username"].ToString();

            lblRol.Text = " | " + manager.ObtenerTexto("Rol") + ": " +
                          Session["Rol"].ToString();
            hlMesas.Text = manager.ObtenerTexto("Mesas");
            hlPlatos.Text = manager.ObtenerTexto("Platos");
            hlIngredientes.Text = manager.ObtenerTexto("Ingredientes");
            hlProveedores.Text = manager.ObtenerTexto("Proveedores");
            hlStock.Text = manager.ObtenerTexto("Stock");
            hlUsuarios.Text = manager.ObtenerTexto("Usuarios");
            hlReportes.Text = manager.ObtenerTexto("Reportes");
            hlBitacora.Text = manager.ObtenerTexto("Bitacora");
            btnCerrarSesion.Text = manager.ObtenerTexto("CerrarSesion");
        }

        protected void ddlIdioma_SelectedIndexChanged(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.CambiarIdioma(ddlIdioma.SelectedValue);
        }
    }
}