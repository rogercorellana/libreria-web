using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using RestauranteApp.Idiomas;

namespace RestauranteApp.PaginasWebmaster
{
    public partial class ABMProveedores : System.Web.UI.Page, ILanguageObserver
    {
        ProveedorBLL proveedorBLL = new ProveedorBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);

            if (!IsPostBack)
                CargarProveedores();
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
            lblGestionProveedores.InnerText = manager.ObtenerTexto("GestionProveedores");

            // Título del formulario
            if (hfIDProveedor.Value == "0")
            {
                lblTituloFormulario.Text = manager.ObtenerTexto("NuevoProveedor");
            }
            else
            {
                lblTituloFormulario.Text = manager.ObtenerTexto("ModificarProveedor");
            }

            // Etiquetas
            lblNombre.InnerText = manager.ObtenerTexto("Nombre");
            lblCUIT.InnerText = manager.ObtenerTexto("CUIT");
            lblTelefono.InnerText = manager.ObtenerTexto("Telefono");
            lblEmail.InnerText = manager.ObtenerTexto("Email");

            // Placeholders
            txtNombre.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderNombreProveedor");
            txtCUIT.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderCUIT");
            txtTelefono.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderTelefono");
            txtEmail.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderEmail");

            // Validadores
            rfvNombre.ErrorMessage = manager.ObtenerTexto("ErrorNombreProveedorObligatorio");
            rfvCUIT.ErrorMessage = manager.ObtenerTexto("ErrorCUITObligatorio");
            revEmail.ErrorMessage = manager.ObtenerTexto("ErrorEmailFormato");

            // Botones
            btnGuardar.Text = manager.ObtenerTexto("Guardar");
            btnCancelar.Text = manager.ObtenerTexto("Cancelar");

            // Lista
            lblListaProveedores.InnerText = manager.ObtenerTexto("ListaProveedores");

            // Encabezados del GridView
            if (gvProveedores.HeaderRow != null)
            {
                gvProveedores.HeaderRow.Cells[0].Text = manager.ObtenerTexto("ID");
                gvProveedores.HeaderRow.Cells[1].Text = manager.ObtenerTexto("Nombre");
                gvProveedores.HeaderRow.Cells[2].Text = manager.ObtenerTexto("CUIT");
                gvProveedores.HeaderRow.Cells[3].Text = manager.ObtenerTexto("Telefono");
                gvProveedores.HeaderRow.Cells[4].Text = manager.ObtenerTexto("Email");
                gvProveedores.HeaderRow.Cells[5].Text = manager.ObtenerTexto("Acciones");
            }

            // Botones Editar
            foreach (GridViewRow fila in gvProveedores.Rows)
            {
                Button btnEditar = (Button)fila.FindControl("btnEditar");

                if (btnEditar != null)
                {
                    btnEditar.Text =
                        manager.ObtenerTexto("Editar");
                }
            }
        }

        private void CargarProveedores()
        {
            gvProveedores.DataSource = proveedorBLL.ObtenerTodos();
            gvProveedores.DataBind();
        }

        private void LimpiarFormulario()
        {
            hfIDProveedor.Value = "0";
            txtNombre.Text = "";
            txtCUIT.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            lblTituloFormulario.Text = "Nuevo Proveedor";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();

            Proveedor proveedor = new Proveedor();
            proveedor.Nombre = txtNombre.Text.Trim();
            proveedor.CUIT = txtCUIT.Text.Trim();
            proveedor.Telefono = txtTelefono.Text.Trim();
            proveedor.Email = txtEmail.Text.Trim();

            bool resultado;

            if (hfIDProveedor.Value == "0")
            {
                resultado = proveedorBLL.Agregar(proveedor);
                MostrarMensaje(resultado,manager.ObtenerTexto("ProveedorAltaCorrecta"),manager.ObtenerTexto("ErrorProveedorAlta"));
            }
            else
            {
                proveedor.ID_Proveedor = Convert.ToInt32(hfIDProveedor.Value);
                resultado = proveedorBLL.Modificar(proveedor);
                MostrarMensaje(resultado, manager.ObtenerTexto("ProveedorModificacionCorrecta"), manager.ObtenerTexto("ErrorProveedorModificacion"));
            }

            LimpiarFormulario();
            CargarProveedores();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            lblMensaje.Visible = false;
        }

        protected void gvProveedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();

            if (e.CommandName == "Editar")
            {
                int idProveedor = Convert.ToInt32(e.CommandArgument);
                Proveedor proveedor = proveedorBLL.ObtenerPorID(idProveedor);

                if (proveedor != null)
                {
                    hfIDProveedor.Value = proveedor.ID_Proveedor.ToString();
                    txtNombre.Text = proveedor.Nombre;
                    txtCUIT.Text = proveedor.CUIT;
                    txtTelefono.Text = proveedor.Telefono;
                    txtEmail.Text = proveedor.Email;
                    lblTituloFormulario.Text = manager.ObtenerTexto("ModificarProveedor");
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
    }
}