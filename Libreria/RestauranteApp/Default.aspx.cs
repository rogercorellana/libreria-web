using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RestauranteApp
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Si no hay sesión activa, mandar al login
            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Si hay sesión, redirigir según el rol
            string rol = Session["Rol"].ToString();

            switch (rol)
            {
                case "ROL-01":
                    Response.Redirect("~/PaginasMozo/MapaMesas.aspx");
                    break;
                case "ROL-02":
                    Response.Redirect("~/PaginasWebmaster/ABMPlatos.aspx");
                    break;
                case "ROL-03":
                    Response.Redirect("~/PaginasAdmin/ABMUsuarios.aspx");
                    break;
                default:
                    // Si el rol no coincide con ninguno, mandar al login
                    Session.Clear();
                    Session.Abandon();
                    Response.Redirect("~/Login.aspx");
                    break;
            }
        }
    }
}