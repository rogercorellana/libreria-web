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
    public partial class ABMPlatos : System.Web.UI.Page, ILanguageObserver
    {
        PlatoBLL platoBLL = new PlatoBLL();
        CategoriaBLL categoriaBLL = new CategoriaBLL();
        BitacoraBLL bitacoraBLL = new BitacoraBLL();

        IngredienteBLL ingredienteBLL = new IngredienteBLL();
        PlatoIngredienteBLL platoIngredienteBLL = new PlatoIngredienteBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);

            if (!IsPostBack)
            {
                CargarCategorias();
                CargarPlatos();
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
            if (hfIDPlato.Value == "0")
            {
                lblTituloFormulario.Text =
                    manager.ObtenerTexto("NuevoPlato");
            }
            else
            {
                lblTituloFormulario.Text =
                    manager.ObtenerTexto("ModificarPlato");
            }

            lblGestionPlatos.InnerText = manager.ObtenerTexto("GestionPlatos");
            lblNombre.InnerText = manager.ObtenerTexto("Nombre");
            lblDescripcion.InnerText = manager.ObtenerTexto("Descripcion");
            lblPrecioVenta.InnerText = manager.ObtenerTexto("PrecioVenta");
            lblCategoria.InnerText = manager.ObtenerTexto("Categoria");
            lblDisponible.InnerText = manager.ObtenerTexto("Disponible");
            foreach (ListItem item in ddlCategoria.Items)
            {
                switch (item.Value)
                {
                    case "4":
                        item.Text = manager.ObtenerTexto("CategoriaBebidas");
                        break;

                    case "1":
                        item.Text = manager.ObtenerTexto("CategoriaEntradas");
                        break;

                    case "2":
                        item.Text = manager.ObtenerTexto("CategoriaPlatosPrincipales");
                        break;

                    case "3":
                        item.Text = manager.ObtenerTexto("CategoriaPostres");
                        break;
                }
            }
            lblIngredientesDelPlato.InnerText = manager.ObtenerTexto("IngredientesDelPlato");
            lblIngrediente.InnerText = manager.ObtenerTexto("Ingrediente");
            lblCantidad.InnerText = manager.ObtenerTexto("Cantidad");
            lblListaPlatos.InnerText = manager.ObtenerTexto("ListaPlatos");

            txtNombre.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderNombrePlato");
            txtDescripcion.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderDescripcionPlato");
            txtPrecioVenta.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderPrecioVenta");

            rfvNombre.ErrorMessage = manager.ObtenerTexto("ErrorNombrePlatoObligatorio");
            rfvPrecioVenta.ErrorMessage = manager.ObtenerTexto("ErrorPrecioVentaObligatorio");

            btnGuardar.Text = manager.ObtenerTexto("Guardar");
            btnCancelar.Text = manager.ObtenerTexto("Cancelar");
            btnAgregarIngrediente.Text = manager.ObtenerTexto("AgregarIngrediente");
            btnCerrarIngredientes.Text = manager.ObtenerTexto("Cerrar");

            // GridView de ingredientes del plato
            if (gvIngredientesPlato.HeaderRow != null)
            {
                gvIngredientesPlato.HeaderRow.Cells[0].Text =
                    manager.ObtenerTexto("Ingrediente");

                gvIngredientesPlato.HeaderRow.Cells[1].Text =
                    manager.ObtenerTexto("Cantidad");

                gvIngredientesPlato.HeaderRow.Cells[2].Text =
                    manager.ObtenerTexto("Unidad");

                gvIngredientesPlato.HeaderRow.Cells[3].Text =
                    manager.ObtenerTexto("Acciones");
            }

            foreach (GridViewRow fila in gvIngredientesPlato.Rows)
            {
                Button btnEliminar = (Button)fila.FindControl("btnEliminarIngrediente");

                if (btnEliminar != null)
                {
                    btnEliminar.Text =
                        manager.ObtenerTexto("Eliminar");
                }
            }

            // GridView principal de platos
            if (gvPlatos.HeaderRow != null)
            {
                gvPlatos.HeaderRow.Cells[0].Text = manager.ObtenerTexto("ID");
                gvPlatos.HeaderRow.Cells[1].Text = manager.ObtenerTexto("Nombre");
                gvPlatos.HeaderRow.Cells[2].Text = manager.ObtenerTexto("Descripcion");
                gvPlatos.HeaderRow.Cells[3].Text = manager.ObtenerTexto("Precio");
                gvPlatos.HeaderRow.Cells[4].Text = manager.ObtenerTexto("Categoria");
                gvPlatos.HeaderRow.Cells[5].Text = manager.ObtenerTexto("Disponible");
                gvPlatos.HeaderRow.Cells[6].Text = manager.ObtenerTexto("Acciones");
            }

            foreach (GridViewRow fila in gvPlatos.Rows)
            {
                Button btnEditar = (Button)fila.FindControl("btnEditar");

                if (btnEditar != null)
                {
                    btnEditar.Text = manager.ObtenerTexto("Editar");
                }

                Button btnIngredientes = (Button)fila.FindControl("btnIngredientes");

                if (btnIngredientes != null)
                {
                    btnIngredientes.Text = manager.ObtenerTexto("Ingredientes");
                }

                Button btnCambiarDisponibilidad = (Button)fila.FindControl("btnCambiarDisponibilidad");

                if (btnCambiarDisponibilidad != null)
                {
                    btnCambiarDisponibilidad.Text = manager.ObtenerTexto("CambiarDisponibilidad");
                }

                Label lblEstado = (Label)fila.FindControl("lblDisponible");

                if (lblEstado != null)
                {
                    bool disponible = Convert.ToBoolean(
                        lblEstado.Attributes["data-disponible"]
                    );

                    lblEstado.Text = disponible
                        ? manager.ObtenerTexto("Disponible")
                        : manager.ObtenerTexto("NoDisponible");

                    lblEstado.CssClass = disponible
                        ? "badge bg-success"
                        : "badge bg-danger";
                }
            }
        }

        private void CargarCategorias()
        {
            ddlCategoria.DataSource = categoriaBLL.ObtenerTodas();
            ddlCategoria.DataBind();
        }

        private void CargarPlatos()
        {
            gvPlatos.DataSource = platoBLL.ObtenerTodos();
            gvPlatos.DataBind();
        }

        private void LimpiarFormulario()
        {
            hfIDPlato.Value = "0";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtPrecioVenta.Text = "";
            chkDisponible.Checked = true;
            ddlCategoria.SelectedIndex = 0;
            lblTituloFormulario.Text = "Nuevo Plato";
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();

            Plato plato = new Plato();
            plato.Nombre = txtNombre.Text.Trim();
            plato.Descripcion = txtDescripcion.Text.Trim();
            plato.PrecioVenta = Convert.ToDecimal(txtPrecioVenta.Text.Trim());
            plato.ID_Categoria = Convert.ToInt32(ddlCategoria.SelectedValue);
            plato.Disponible = chkDisponible.Checked;

            // Guardar estos valores ANTES de limpiar el formulario
            string nombrePlato = txtNombre.Text.Trim();
            bool esAlta = hfIDPlato.Value == "0";

            bool resultado;

            if (esAlta)
            {
                resultado = platoBLL.Agregar(plato);
                MostrarMensaje(resultado, manager.ObtenerTexto("PlatoAltaCorrecta"), manager.ObtenerTexto("ErrorPlatoAlta"));
            }
            else
            {
                plato.ID_Plato = Convert.ToInt32(hfIDPlato.Value);
                resultado = platoBLL.Modificar(plato);
                MostrarMensaje(resultado, manager.ObtenerTexto("PlatoModificacionCorrecta"), manager.ObtenerTexto("ErrorPlatoModificacion"));
            }

            // Registrar en bitácora ANTES de limpiar
            if (resultado)
            {
                int idUsuario = ((Usuario)Session["Usuario"]).ID_Usuario;
                string username = ((Usuario)Session["Usuario"]).Username;

                if (esAlta)
                    bitacoraBLL.Registrar(idUsuario, username, 14, "Alta de plato: " + nombrePlato, 2);
                else
                    bitacoraBLL.Registrar(idUsuario, username, 15, "Modificación de plato: " + nombrePlato, 2);
            }

            LimpiarFormulario();
            CargarPlatos();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            lblMensaje.Visible = false;
        }

        protected void gvPlatos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();

            int idPlato = Convert.ToInt32(e.CommandArgument);
            int idUsuario = ((Usuario)Session["Usuario"]).ID_Usuario;
            string username = ((Usuario)Session["Usuario"]).Username;

            if (e.CommandName == "Editar")
            {
                Plato plato = platoBLL.ObtenerPorID(idPlato);
                if (plato != null)
                {
                    hfIDPlato.Value = plato.ID_Plato.ToString();
                    txtNombre.Text = plato.Nombre;
                    txtDescripcion.Text = plato.Descripcion;
                    txtPrecioVenta.Text = plato.PrecioVenta.ToString();
                    chkDisponible.Checked = plato.Disponible;
                    ddlCategoria.SelectedValue = plato.ID_Categoria.ToString();
                    lblTituloFormulario.Text = manager.ObtenerTexto("ModificarPlato");
                }
            }
            else if (e.CommandName == "VerIngredientes")
            {
                Plato plato = platoBLL.ObtenerPorID(idPlato);
                if (plato != null)
                {
                    hfIDPlatoIngredientes.Value = idPlato.ToString();
                    lblNombrePlato.Text = plato.Nombre;
                    pnlIngredientes.Visible = true;
                    CargarIngredientesDropdown();
                    CargarIngredientesPlato(idPlato);
                }
            }
            else if (e.CommandName == "CambiarDisponibilidad")
            {
                Plato plato = platoBLL.ObtenerPorID(idPlato);
                if (plato != null)
                {
                    bool resultado = platoBLL.CambiarDisponibilidad(idPlato, !plato.Disponible);
                    MostrarMensaje(resultado, manager.ObtenerTexto("DisponibilidadActualizada"), manager.ObtenerTexto("ErrorDisponibilidad"));
                    if (resultado)
                    {
                        string estado = !plato.Disponible ? "disponible" : "no disponible";
                        bitacoraBLL.Registrar(idUsuario, username, 16,
                            "Cambio de disponibilidad del plato '" + plato.Nombre + "' a " + estado);
                    }
                    CargarPlatos();
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

        private void CargarIngredientesDropdown()
        {
            ddlIngrediente.DataSource = ingredienteBLL.ObtenerTodos();
            ddlIngrediente.DataBind();
        }

        private void CargarIngredientesPlato(int idPlato)
        {
            gvIngredientesPlato.DataSource = platoIngredienteBLL.ObtenerPorPlato(idPlato);
            gvIngredientesPlato.DataBind();
        }

        protected void btnAgregarIngrediente_Click(object sender, EventArgs e)
        {
            int idPlato = Convert.ToInt32(hfIDPlatoIngredientes.Value);
            int idIngrediente = Convert.ToInt32(ddlIngrediente.SelectedValue);
            decimal cantidad = Convert.ToDecimal(txtCantidadIngrediente.Text.Trim());

            PlatoIngrediente pi = new PlatoIngrediente
            {
                ID_Plato = idPlato,
                ID_Ingrediente = idIngrediente,
                CantidadUsada = cantidad
            };

            bool resultado = platoIngredienteBLL.Agregar(pi);
            if (resultado)
            {
                txtCantidadIngrediente.Text = "";
                CargarIngredientesPlato(idPlato);
            }
            else
            {
                MostrarMensaje(false, "", LanguageManager.ObtenerInstancia().ObtenerTexto("ErrorAgregarIngrediente"));
            }
        }

        protected void gvIngredientesPlato_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EliminarIngrediente")
            {
                int idPlatoIngrediente = Convert.ToInt32(e.CommandArgument);
                platoIngredienteBLL.Eliminar(idPlatoIngrediente);
                CargarIngredientesPlato(Convert.ToInt32(hfIDPlatoIngredientes.Value));
            }
        }

        protected void btnCerrarIngredientes_Click(object sender, EventArgs e)
        {
            pnlIngredientes.Visible = false;
            hfIDPlatoIngredientes.Value = "0";
        }


    }
}