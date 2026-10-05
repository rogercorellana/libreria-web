using System;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;

namespace RestauranteApp
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] != null)
                Response.Redirect("~/Default.aspx");
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            lblMensaje.Visible = false;
            lblMensaje.Text = "";

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            UsuarioBLL usuarioBLL = new UsuarioBLL();
            BitacoraBLL bitacoraBLL = new BitacoraBLL();
            Usuario usuarioVerificado = usuarioBLL.ObtenerPorUsername(username);

            if (usuarioVerificado == null)
            {
                bitacoraBLL.Registrar(null, username, 3, "Intento de login con usuario inexistente: " + username, 2);
                MostrarError("El usuario no existe en el sistema.");
                return;
            }

            if (!usuarioVerificado.Activo)
            {
                MostrarError("Su cuenta está bloqueada. Contacte al administrador.");
                return;
            }

            Usuario usuario = usuarioBLL.ValidarLogin(username, password);

            if (usuario != null)
            {
                IntegridadBLL integridadBLL = new IntegridadBLL();

                // ============================================
                // ROL-02 (Webmaster) — ve el cartel detallado con TODAS las tablas
                // ============================================
                if (usuario.Rol == "ROL-02")
                {
                    List<string> errores = new List<string>();
                    errores.AddRange(integridadBLL.ValidarIntegridadWebmaster());
                    errores.AddRange(integridadBLL.ValidarIntegridadAdmin());
                    if (errores.Count > 0)
                    {
                        bitacoraBLL.Registrar(usuario.ID_Usuario, usuario.Username, 1,
                            "Inicio de sesión. Corrupción detectada en la base de datos.", 3);
                        Session["Usuario"] = usuario;
                        Session["Username"] = usuario.Username;
                        Session["Rol"] = usuario.Rol;
                        Session["Nombre"] = usuario.Nombre + " " + usuario.Apellido;
                        Session["ErroresIntegridad"] = errores;
                        IniciarSesionFormsAuth(usuario);
                        Response.Redirect("~/PaginasWebmaster/AlertaIntegridad.aspx");
                        return;
                    }
                }

                // ============================================
                // ROL-03 (Admin) — no puede entrar si hay corrupción
                // ============================================
                else if (usuario.Rol == "ROL-03")
                {
                    List<string> erroresWebmaster = integridadBLL.ValidarIntegridadWebmaster();
                    List<string> erroresAdmin = integridadBLL.ValidarIntegridadAdmin();

                    if (erroresWebmaster.Count > 0 || erroresAdmin.Count > 0)
                    {
                        bitacoraBLL.Registrar(usuario.ID_Usuario, usuario.Username, 3,
                            "Intento de login bloqueado por corrupción en la base de datos.", 3);
                        MostrarError("⚠ Error de integridad en la base de datos. Contacte al Webmaster/Encargado para solucionar el problema antes de continuar.");
                        return;
                    }
                }

                // ============================================
                // ROL-01 (Mozo) — no puede entrar si hay corrupción
                // ============================================
                else if (usuario.Rol == "ROL-01")
                {
                    List<string> erroresWebmaster = integridadBLL.ValidarIntegridadWebmaster();
                    List<string> erroresAdmin = integridadBLL.ValidarIntegridadAdmin();

                    if (erroresWebmaster.Count > 0 || erroresAdmin.Count > 0)
                    {
                        bitacoraBLL.Registrar(usuario.ID_Usuario, usuario.Username, 3,
                            "Intento de login bloqueado por corrupción en la base de datos.", 3);
                        MostrarError("⚠ Error de integridad en la base de datos. Contacte al Webmaster/Encargado para solucionar el problema antes de continuar.");
                        return;
                    }
                }

                // ============================================
                // Sin errores — iniciar sesión normalmente
                // ============================================
                bitacoraBLL.Registrar(usuario.ID_Usuario, usuario.Username, 1, "Inicio de sesión exitoso.", 1);

                Session["Usuario"] = usuario;
                Session["Username"] = usuario.Username;
                Session["Rol"] = usuario.Rol;
                Session["Nombre"] = usuario.Nombre + " " + usuario.Apellido;

                IniciarSesionFormsAuth(usuario);

                switch (usuario.Rol)
                {
                    case "ROL-01": Response.Redirect("~/PaginasMozo/MapaMesas.aspx"); break;
                    case "ROL-02": Response.Redirect("~/PaginasWebmaster/ABMPlatos.aspx"); break;
                    case "ROL-03": Response.Redirect("~/PaginasAdmin/ABMUsuarios.aspx"); break;
                }
            }
            else
            {
                Usuario usuarioActualizado = usuarioBLL.ObtenerPorUsername(username);

                if (usuarioActualizado != null && !usuarioActualizado.Activo)
                {
                    bitacoraBLL.Registrar(usuarioActualizado.ID_Usuario, username, 7,
                        "Cuenta bloqueada por exceder intentos fallidos.", 3);
                    MostrarError("Su cuenta fue bloqueada por exceder el límite de intentos. Contacte al administrador.");
                }
                else
                {
                    int intentos = usuarioActualizado?.IntentosFallidos ?? 0;
                    bitacoraBLL.Registrar(usuarioActualizado?.ID_Usuario, username, 3,
                        "Intento de login fallido. Intentos: " + intentos, 2);
                    int intentosRestantes = 3 - intentos;
                    MostrarError("Usuario o contraseña incorrectos. Intentos restantes: " + intentosRestantes);
                }
            }
        }

        private void IniciarSesionFormsAuth(Usuario usuario)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1, usuario.Username, DateTime.Now,
                DateTime.Now.AddMinutes(30), false, usuario.Rol);
            string ticketEncriptado = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEncriptado);
            Response.Cookies.Add(cookie);
        }

        private void MostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
        }
    }
}