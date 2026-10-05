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
    public partial class ABMIngredientes : System.Web.UI.Page, ILanguageObserver
    {
        IngredienteBLL ingredienteBLL = new IngredienteBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            manager.RegistrarObservador(this);

            if (!IsPostBack)
            {
                CargarIngredientes();
                CargarAlertaStockBajo();
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
            lblGestionIngredientes.InnerText = manager.ObtenerTexto("GestionIngredientes");
            if (hfIDIngrediente.Value == "0")
            {
                lblTituloFormulario.Text =
                    manager.ObtenerTexto("NuevoIngrediente");
            }
            else
            {
                lblTituloFormulario.Text =
                    manager.ObtenerTexto("ModificarIngrediente");
            }
            lblNombre.InnerText = manager.ObtenerTexto("Nombre");
            lblUnidadMedida.InnerText = manager.ObtenerTexto("UnidadMedida");
            lblStockActual.InnerText = manager.ObtenerTexto("StockActual");
            lblStockMinimo.InnerText = manager.ObtenerTexto("StockMinimo");
            lblPrecioCosto.InnerText = manager.ObtenerTexto("PrecioCosto");

            rfvNombre.ErrorMessage = manager.ObtenerTexto("ErrorNombreObligatorio");
            rfvStockActual.ErrorMessage = manager.ObtenerTexto("ErrorStockActualObligatorio");
            rfvStockMinimo.ErrorMessage = manager.ObtenerTexto("ErrorStockMinimoObligatorio");
            rfvPrecioCosto.ErrorMessage = manager.ObtenerTexto("ErrorPrecioCostoObligatorio");

            txtNombre.Attributes["placeholder"] = manager.ObtenerTexto("PlaceholderNombreIngrediente");
            ddlUnidadMedida.Items.FindByValue("kg").Text = manager.ObtenerTexto("UnidadKg");
            ddlUnidadMedida.Items.FindByValue("g").Text =  manager.ObtenerTexto("UnidadGramos");
            ddlUnidadMedida.Items.FindByValue("litros").Text = manager.ObtenerTexto("UnidadLitros");
            ddlUnidadMedida.Items.FindByValue("ml").Text = manager.ObtenerTexto("UnidadMl");
            ddlUnidadMedida.Items.FindByValue("unidades").Text = manager.ObtenerTexto("UnidadUnidades");

            btnGuardar.Text = manager.ObtenerTexto("Guardar");
            btnCancelar.Text = manager.ObtenerTexto("Cancelar");

            lblIngredientesStockBajo.InnerText = manager.ObtenerTexto("IngredientesStockBajo");
            lblListaIngredientes.InnerText = manager.ObtenerTexto("ListaIngredientes");
            gvStockBajo.Columns[0].HeaderText = manager.ObtenerTexto("Ingrediente");
            gvStockBajo.Columns[1].HeaderText = manager.ObtenerTexto("StockActual");
            gvStockBajo.Columns[2].HeaderText = manager.ObtenerTexto("StockMinimo");
            gvStockBajo.Columns[3].HeaderText = manager.ObtenerTexto("Unidad");

            if (gvIngredientes.HeaderRow != null)
            {
                gvIngredientes.HeaderRow.Cells[0].Text = manager.ObtenerTexto("ID");
                gvIngredientes.HeaderRow.Cells[1].Text = manager.ObtenerTexto("Nombre");
                gvIngredientes.HeaderRow.Cells[2].Text = manager.ObtenerTexto("Unidad");
                gvIngredientes.HeaderRow.Cells[3].Text = manager.ObtenerTexto("StockActual");
                gvIngredientes.HeaderRow.Cells[4].Text = manager.ObtenerTexto("StockMinimo");
                gvIngredientes.HeaderRow.Cells[5].Text = manager.ObtenerTexto("Acciones");
            }
            foreach (GridViewRow fila in gvIngredientes.Rows)
            {
                Button btnEditar = (Button)fila.FindControl("btnEditar");
                if (btnEditar != null)
                    btnEditar.Text = manager.ObtenerTexto("Editar");
            }
        }

        private void CargarIngredientes()
        {
            gvIngredientes.DataSource = ingredienteBLL.ObtenerTodos();
            gvIngredientes.DataBind();
        }

        private void CargarAlertaStockBajo()
        {
            var bajoStock = ingredienteBLL.ObtenerBajoStock();
            if (bajoStock.Count > 0)
            {
                pnlAlertaStock.Visible = true;
                gvStockBajo.DataSource = bajoStock;
                gvStockBajo.DataBind();
            }
            else
            {
                pnlAlertaStock.Visible = false;
            }
        }

        private void LimpiarFormulario()
        {
            hfIDIngrediente.Value = "0";
            txtNombre.Text = "";
            txtStockActual.Text = "";
            txtStockMinimo.Text = "";
            txtPrecioCosto.Text = "";
            ddlUnidadMedida.SelectedIndex = 0;
            lblTituloFormulario.Text = LanguageManager.ObtenerInstancia().ObtenerTexto("NuevoIngrediente");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            try
            {
                //MostrarMensaje(false, "", "Entré al método btnGuardar_Click");
                //return;
                Ingrediente ingrediente = new Ingrediente();
                ingrediente.Nombre = txtNombre.Text.Trim();
                ingrediente.UnidadMedida = ddlUnidadMedida.SelectedValue;
                ingrediente.StockActual = Convert.ToDecimal(txtStockActual.Text.Trim());
                ingrediente.StockMinimo = Convert.ToDecimal(txtStockMinimo.Text.Trim());
                ingrediente.PrecioCosto = txtPrecioCosto.Text.Trim();

                bool resultado;

                if (hfIDIngrediente.Value == "0")
                {
                    resultado = ingredienteBLL.Agregar(ingrediente);

                    MostrarMensaje(
                        resultado,
                        manager.ObtenerTexto("IngredienteAltaCorrecta"),
                        manager.ObtenerTexto("ErrorIngredienteAlta"));
                }
                else
                {
                    ingrediente.ID_Ingrediente = Convert.ToInt32(hfIDIngrediente.Value);
                    resultado = ingredienteBLL.Modificar(ingrediente);

                    MostrarMensaje(
                        resultado,
                        manager.ObtenerTexto("IngredienteModificacionCorrecta"),
                        manager.ObtenerTexto("ErrorIngredienteModificacion"));
                }

                LimpiarFormulario();
                CargarIngredientes();
                CargarAlertaStockBajo();
            }
            catch (Exception ex)
            {
                MostrarMensaje(
                    false,
                    "",
                    manager.ObtenerTexto("ErrorDetallado") + ": " +
                    ex.Message + " | " +
                    ex.InnerException?.Message);
            }
        }
        //protected void btnGuardar_Click(object sender, EventArgs e)
        //{
        //    Ingrediente ingrediente = new Ingrediente();
        //    ingrediente.Nombre = txtNombre.Text.Trim();
        //    ingrediente.UnidadMedida = ddlUnidadMedida.SelectedValue;
        //    ingrediente.StockActual = Convert.ToDecimal(txtStockActual.Text.Trim());
        //    ingrediente.StockMinimo = Convert.ToDecimal(txtStockMinimo.Text.Trim());
        //    ingrediente.PrecioCosto = txtPrecioCosto.Text.Trim();

        //    bool resultado;

        //    if (hfIDIngrediente.Value == "0")
        //    {
        //        resultado = ingredienteBLL.Agregar(ingrediente);
        //        MostrarMensaje(resultado, "Ingrediente dado de alta correctamente.", "Error al dar de alta el ingrediente.");
        //    }
        //    else
        //    {
        //        ingrediente.ID_Ingrediente = Convert.ToInt32(hfIDIngrediente.Value);
        //        resultado = ingredienteBLL.Modificar(ingrediente);
        //        MostrarMensaje(resultado, "Ingrediente modificado correctamente.", "Error al modificar el ingrediente.");
        //    }

        //    LimpiarFormulario();
        //    CargarIngredientes();
        //    CargarAlertaStockBajo();
        //}

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            lblMensaje.Visible = false;
        }

        protected void gvIngredientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            LanguageManager manager = LanguageManager.ObtenerInstancia();
            if (e.CommandName == "Editar")
            {
                int idIngrediente = Convert.ToInt32(e.CommandArgument);
                Ingrediente ingrediente = ingredienteBLL.ObtenerPorID(idIngrediente);

                if (ingrediente != null)
                {
                    hfIDIngrediente.Value = ingrediente.ID_Ingrediente.ToString();
                    txtNombre.Text = ingrediente.Nombre;
                    ddlUnidadMedida.SelectedValue = ingrediente.UnidadMedida;
                    txtStockActual.Text = ingrediente.StockActual.ToString();
                    txtStockMinimo.Text = ingrediente.StockMinimo.ToString();
                    txtPrecioCosto.Text = ingrediente.PrecioCosto;
                    lblTituloFormulario.Text = manager.ObtenerTexto("ModificarIngrediente");
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