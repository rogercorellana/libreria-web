<%@ Page Title="Gestión de Platos" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ABMPlatos.aspx.cs"
    Inherits="RestauranteApp.PaginasWebmaster.ABMPlatos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

 <h2 id="lblGestionPlatos" runat="server" class="mb-4"></h2>

    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"
        CssClass="alert d-block mb-3" />

    <%-- FORMULARIO --%>
    <div class="card mb-4">
        <div class="card-header bg-dark text-white">
            <asp:Label ID="lblTituloFormulario" runat="server" Text="Nuevo Plato" />
        </div>
        <div class="card-body">
            <div class="row">

                <div class="col-md-4 mb-3">
                 <label id="lblNombre" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"
                        placeholder="Nombre del plato" />
                    <asp:RequiredFieldValidator ID="rfvNombre" runat="server"
                        ControlToValidate="txtNombre"
                        ErrorMessage="El nombre es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="col-md-4 mb-3">
                    <label id="lblDescripcion" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control"
                        placeholder="Descripción del plato" />
                </div>
                <div class="col-md-4 mb-3">
                    <label id="lblPrecioVenta" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtPrecioVenta" runat="server" CssClass="form-control"
                        placeholder="0.00" />
                    <asp:RequiredFieldValidator ID="rfvPrecioVenta" runat="server"
                        ControlToValidate="txtPrecioVenta"
                        ErrorMessage="El precio es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                    <%--<asp:RangeValidator ID="rvPrecioVenta" runat="server"
                        ControlToValidate="txtPrecioVenta"
                        MinimumValue="0.01" MaximumValue="99999"
                        Type="Currency"
                        ErrorMessage="El precio debe ser mayor a cero."
                        CssClass="text-danger small" Display="Dynamic" />--%>
                </div>
                <div class="col-md-4 mb-3">
                    <label id="lblCategoria" runat="server" class="form-label fw-bold"></label>
                    <asp:DropDownList ID="ddlCategoria" runat="server"
                        CssClass="form-select"
                        DataTextField="Nombre"
                        DataValueField="ID_Categoria" />
                </div>
                <div class="col-md-4 mb-3">
                    <label id="lblDisponible" runat="server" class="form-label fw-bold"></label>
                    <asp:CheckBox ID="chkDisponible" runat="server" Checked="true" />
                </div>
            </div>
            <asp:HiddenField ID="hfIDPlato" runat="server" Value="0" />
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar"
                CssClass="btn btn-success me-2" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar"
                CssClass="btn btn-secondary" OnClick="btnCancelar_Click"
                CausesValidation="false" />
        </div>
    </div>

    <%-- PANEL DE INGREDIENTES DEL PLATO --%>
    <asp:HiddenField ID="hfIDPlatoIngredientes" runat="server" Value="0" />
    <asp:Panel ID="pnlIngredientes" runat="server" Visible="false">
        <div class="card-header bg-secondary text-white">
             <span id="lblIngredientesDelPlato" runat="server"></span>
             <asp:Label ID="lblNombrePlato" runat="server" />
        </div>
        <div class="card-body">
            <%-- Agregar ingrediente --%>
            <div class="row mb-3">
                <div class="col-md-5">
                    <label id="lblIngrediente" runat="server" class="form-label fw-bold"></label>
                    <asp:DropDownList ID="ddlIngrediente" runat="server"
                        CssClass="form-select"
                        DataTextField="Nombre"
                        DataValueField="ID_Ingrediente" />
                </div>
                <div class="col-md-3">
                    <label id="lblCantidad" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtCantidadIngrediente" runat="server"
                        CssClass="form-control" placeholder="0.00" />
                </div>
                <div class="col-md-4 d-flex align-items-end">
                    <asp:Button ID="btnAgregarIngrediente" runat="server"
                        Text="Agregar ingrediente"
                        CssClass="btn btn-secondary w-100"
                        OnClick="btnAgregarIngrediente_Click"
                        CausesValidation="false" />
                </div>
            </div>

            <%-- Lista de ingredientes del plato --%>
            <asp:GridView ID="gvIngredientesPlato" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-striped"
                OnRowCommand="gvIngredientesPlato_RowCommand">
                <Columns>
                    <asp:BoundField DataField="NombreIngrediente" HeaderText="Ingrediente" />
                    <asp:BoundField DataField="CantidadUsada"     HeaderText="Cantidad"    />
                    <asp:BoundField DataField="UnidadMedida"      HeaderText="Unidad"      />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:Button ID="btnEliminarIngrediente" runat="server" Text="Eliminar"
                                CommandName="EliminarIngrediente"
                                CommandArgument='<%# Eval("ID_PlatoIngrediente") %>'
                                CssClass="btn btn-danger btn-sm"
                                CausesValidation="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Button ID="btnCerrarIngredientes" runat="server"
                Text="Cerrar"
                CssClass="btn btn-dark"
                OnClick="btnCerrarIngredientes_Click"
                CausesValidation="false" />

        </div>
    </div>
</asp:Panel>

    <%-- GRILLA --%>
    <div class="card">
       <div id="lblListaPlatos" runat="server" class="card-header bg-dark text-white">
       </div>
        <div class="card-body">
            <asp:GridView ID="gvPlatos" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-striped table-hover"
                OnRowCommand="gvPlatos_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ID_Plato"        HeaderText="ID"          />
                    <asp:BoundField DataField="Nombre"          HeaderText="Nombre"      />
                    <asp:BoundField DataField="Descripcion"     HeaderText="Descripción" />
                    <asp:BoundField DataField="PrecioVenta"     HeaderText="Precio"      DataFormatString="${0:N2}" />
                    <asp:BoundField DataField="NombreCategoria" HeaderText="Categoría"   />
                    <asp:TemplateField HeaderText="Disponible">
                        <ItemTemplate>
                             <asp:Label ID="lblDisponible" runat="server" Text='<%# Eval("Disponible") %>'
                               CssClass='<%# Convert.ToBoolean(Eval("Disponible")) ? "badge bg-success" : "badge bg-danger" %>'
                               Attributes-Data-Disponible='<%# Eval("Disponible") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:Button ID="btnEditar" runat="server" Text="Editar"
                                CommandName="Editar"
                                CommandArgument='<%# Eval("ID_Plato") %>'
                                CssClass="btn btn-primary btn-sm me-1"
                                CausesValidation="false" />
                           <asp:Button ID="btnIngredientes" runat="server" Text="Ingredientes"
                                CommandName="VerIngredientes"
                                CommandArgument='<%# Eval("ID_Plato") %>'
                                CssClass="btn btn-secondary btn-sm me-1"
                                CausesValidation="false" />
                            <asp:Button ID="btnCambiarDisponibilidad" runat="server" Text="Cambiar disponibilidad"
                                CommandName="CambiarDisponibilidad"
                                CommandArgument='<%# Eval("ID_Plato") %>'
                                CssClass="btn btn-warning btn-sm"
                                CausesValidation="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>