using BE;
using BLL;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RestauranteApp.Idiomas;


namespace RestauranteApp.PaginasAdmin
{
    public partial class ABMUsuarios : System.Web.UI.Page, ILanguageObserver
    {
        UsuarioBLL usuarioBLL = new UsuarioBLL();
        BitacoraBLL bitacoraBLL = new BitacoraBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);
            if (!IsPostBack)
                CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            gvUsuarios.DataSource = usuarioBLL.ObtenerTodos();
            gvUsuarios.DataBind();
        }

        private void LimpiarFormulario()
        {
            hfIDUsuario.Value = "0";
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtMail.Text = "";
            ddlRol.SelectedIndex = 0;
            lblTituloFormulario.Text = "Nuevo Usuario";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Usuario usuario = new Usuario();
            usuario.Nombre = txtNombre.Text.Trim();
            usuario.Apellido = txtApellido.Text.Trim();
            usuario.Username = txtUsername.Text.Trim();
            usuario.Password = txtPassword.Text.Trim();
            usuario.Mail = txtMail.Text.Trim();
            usuario.Rol = ddlRol.SelectedValue;
            usuario.Activo = true;

            bool resultado;
            int idAdmin = ((Usuario)Session["Usuario"]).ID_Usuario;
            string usAdmin = ((Usuario)Session["Usuario"]).Username;

            if (hfIDUsuario.Value == "0")
            {
                resultado = usuarioBLL.Agregar(usuario);
                MostrarMensaje(resultado, "Usuario dado de alta correctamente.", "Error al dar de alta el usuario.");
                if (resultado)
                    bitacoraBLL.Registrar(idAdmin, usAdmin, 4, "Alta de usuario: " + usuario.Username, 2);
            }
            else
            {
                usuario.ID_Usuario = Convert.ToInt32(hfIDUsuario.Value);
                resultado = usuarioBLL.Modificar(usuario);
                MostrarMensaje(resultado, "Usuario modificado correctamente.", "Error al modificar el usuario.");
                if (resultado)
                    bitacoraBLL.Registrar(idAdmin, usAdmin, 6, "Modificación de usuario: " + usuario.Username, 2);
            }

            LimpiarFormulario();
            CargarUsuarios();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            lblMensaje.Visible = false;
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idUsuario = Convert.ToInt32(e.CommandArgument);
            int idAdmin = ((Usuario)Session["Usuario"]).ID_Usuario;
            string usAdmin = ((Usuario)Session["Usuario"]).Username;

            if (e.CommandName == "Editar")
            {
                Usuario usuario = usuarioBLL.ObtenerPorID(idUsuario);
                if (usuario != null)
                {
                    hfIDUsuario.Value = usuario.ID_Usuario.ToString();
                    txtNombre.Text = usuario.Nombre;
                    txtApellido.Text = usuario.Apellido;
                    txtUsername.Text = usuario.Username;
                    txtPassword.Text = "";
                    txtMail.Text = usuario.Mail;
                    ddlRol.SelectedValue = usuario.Rol;
                    lblTituloFormulario.Text = "Modificar Usuario";
                }
            }
            else if (e.CommandName == "CambiarEstado")
            {
                Usuario usuario = usuarioBLL.ObtenerPorID(idUsuario);
                if (usuario != null)
                {
                    bool resultado;
                    if (usuario.Activo)
                    {
                        resultado = usuarioBLL.Bloquear(idUsuario);
                        MostrarMensaje(resultado, "Usuario bloqueado correctamente.", "Error al bloquear el usuario.");
                        if (resultado)
                            bitacoraBLL.Registrar(idAdmin, usAdmin, 7, "Bloqueo de usuario: " + usuario.Username, 3);
                    }
                    else
                    {
                        resultado = usuarioBLL.Desbloquear(idUsuario);
                        MostrarMensaje(resultado, "Usuario desbloqueado correctamente.", "Error al desbloquear el usuario.");
                        if (resultado)
                            bitacoraBLL.Registrar(idAdmin, usAdmin, 8, "Desbloqueo de usuario: " + usuario.Username, 2);
                    }
                    CargarUsuarios();
                }
            }
        }

        private void MostrarMensaje(bool exito, string mensajeExito, string mensajeError)
        {
            lblMensaje.Visible = true;
            lblMensaje.Text = exito ? mensajeExito : mensajeError;
            lblMensaje.CssClass = exito ? "alert alert-success d-block mb-3"
                                        : "alert alert-danger d-block mb-3";
        }

        public void ActualizarIdioma()
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            lblGestionUsuarios.InnerText = manager.ObtenerTexto("GestionUsuarios");
            lblNombre.InnerText = manager.ObtenerTexto("Nombre");
            lblApellido.InnerText = manager.ObtenerTexto("Apellido");
            lblUsername.InnerText = manager.ObtenerTexto("Username");
            lblPassword.InnerText = manager.ObtenerTexto("Password");
            lblMail.InnerText = manager.ObtenerTexto("Mail");
            lblRol.InnerText = manager.ObtenerTexto("Rol");
            lblListaUsuarios.Text = manager.ObtenerTexto("ListaUsuarios");
            btnGuardar.Text = manager.ObtenerTexto("Guardar");
            btnCancelar.Text = manager.ObtenerTexto("Cancelar");

            foreach (GridViewRow fila in gvUsuarios.Rows)
            {
                Button btnEditar = (Button)fila.FindControl("btnEditar");
                Button btnCambiarEstado = (Button)fila.FindControl("btnCambiarEstado");

                if (btnEditar != null)
                    btnEditar.Text = manager.ObtenerTexto("Editar");

                if (btnCambiarEstado != null)
                    btnCambiarEstado.Text = manager.ObtenerTexto("BloquearDesbloquear");
            }

            lblTituloFormulario.Text = manager.ObtenerTexto("NuevoUsuario");

            // Placeholders
            txtNombre.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderNombre");
            txtApellido.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderApellido");
            txtUsername.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderUsername");
            txtPassword.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderPassword");
            txtMail.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderMail");

            // Opciones del rol
            ddlRol.Items.FindByValue("ROL-01").Text = manager.ObtenerTexto("RolMozo");
            ddlRol.Items.FindByValue("ROL-02").Text = manager.ObtenerTexto("RolWebmaster");
            ddlRol.Items.FindByValue("ROL-03").Text = manager.ObtenerTexto("RolAdministrador");

            // Encabezados de la grilla
            if (gvUsuarios.HeaderRow != null)
            {
                gvUsuarios.HeaderRow.Cells[0].Text = manager.ObtenerTexto("ID");
                gvUsuarios.HeaderRow.Cells[1].Text = manager.ObtenerTexto("Nombre");
                gvUsuarios.HeaderRow.Cells[2].Text = manager.ObtenerTexto("Apellido");
                gvUsuarios.HeaderRow.Cells[3].Text = manager.ObtenerTexto("Username");
                gvUsuarios.HeaderRow.Cells[4].Text = manager.ObtenerTexto("Mail");
                gvUsuarios.HeaderRow.Cells[5].Text = manager.ObtenerTexto("Rol");
                gvUsuarios.HeaderRow.Cells[6].Text = manager.ObtenerTexto("Estado");
                gvUsuarios.HeaderRow.Cells[7].Text = manager.ObtenerTexto("Acciones");
            }
        }
    }
}