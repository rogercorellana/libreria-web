<%@ Page Title="Gestión de Ingredientes" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ABMIngredientes.aspx.cs"
    Inherits="RestauranteApp.PaginasWebmaster.ABMIngredientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2 id="lblGestionIngredientes" runat="server" class="mb-4"></h2>

    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"
        CssClass="alert d-block mb-3" />
    <%-- FORMULARIO --%>
    <div class="card mb-4">
        <div class="card-header bg-dark text-white">
            <asp:Label ID="lblTituloFormulario" runat="server" Text="Nuevo Ingrediente" />
        </div>
        <div class="card-body">
            <div class="row">

                <div class="col-md-4 mb-3">
                    <label id="lblNombre" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"
                        placeholder="Nombre del ingrediente" />
                    <asp:RequiredFieldValidator ID="rfvNombre" runat="server"
                        ControlToValidate="txtNombre"
                        ErrorMessage="El nombre es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="col-md-4 mb-3">
                    <label id="lblUnidadMedida" runat="server" class="form-label fw-bold"></label>
                    <asp:DropDownList ID="ddlUnidadMedida" runat="server" CssClass="form-select">
                        <asp:ListItem Text="kg"      Value="kg"      />
                        <asp:ListItem Text="g"       Value="g"       />
                        <asp:ListItem Text="litros"  Value="litros"  />
                        <asp:ListItem Text="ml"      Value="ml"      />
                        <asp:ListItem Text="unidades" Value="unidades" />
                    </asp:DropDownList>
                </div>

                <div class="col-md-4 mb-3">
                    <label id="lblStockActual" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtStockActual" runat="server" CssClass="form-control"
                        placeholder="0.00" />
                    <asp:RequiredFieldValidator ID="rfvStockActual" runat="server"
                        ControlToValidate="txtStockActual"
                        ErrorMessage="El stock actual es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>

                <div class="col-md-4 mb-3">
                    <label id="lblStockMinimo" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtStockMinimo" runat="server" CssClass="form-control"
                        placeholder="0.00" />
                    <asp:RequiredFieldValidator ID="rfvStockMinimo" runat="server"
                        ControlToValidate="txtStockMinimo"
                        ErrorMessage="El stock mínimo es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>

                <div class="col-md-4 mb-3">
                    <label id="lblPrecioCosto" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtPrecioCosto" runat="server" CssClass="form-control"
                        placeholder="0.00" />
                    <asp:RequiredFieldValidator ID="rfvPrecioCosto" runat="server"
                        ControlToValidate="txtPrecioCosto"
                        ErrorMessage="El precio de costo es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>

            </div>

            <asp:HiddenField ID="hfIDIngrediente" runat="server" Value="0" />

            <asp:Button ID="btnGuardar" runat="server" Text="Guardar"
                CssClass="btn btn-success me-2" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar"
                CssClass="btn btn-secondary" OnClick="btnCancelar_Click"
                CausesValidation="false" />

        </div>
    </div>

    <%-- ALERTA STOCK BAJO --%>
    <asp:Panel ID="pnlAlertaStock" runat="server" Visible="false">
        <div class="alert alert-warning mb-4">
            <h5>⚠ <span id="lblIngredientesStockBajo" runat="server"></span></h5>
            <asp:GridView ID="gvStockBajo" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-warning mb-0">
                <Columns>
                    <asp:BoundField DataField="Nombre"       HeaderText="Ingrediente"   />
                    <asp:BoundField DataField="StockActual"  HeaderText="Stock actual"  />
                    <asp:BoundField DataField="StockMinimo"  HeaderText="Stock mínimo"  />
                    <asp:BoundField DataField="UnidadMedida" HeaderText="Unidad"        />
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>

    <%-- GRILLA --%>
    <div class="card">
      <div id="lblListaIngredientes" runat="server" class="card-header bg-dark text-white">
        </div>
        <div class="card-body">
            <asp:GridView ID="gvIngredientes" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-striped table-hover"
                OnRowCommand="gvIngredientes_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ID_Ingrediente" HeaderText="ID"           />
                    <asp:BoundField DataField="Nombre"         HeaderText="Nombre"       />
                    <asp:BoundField DataField="UnidadMedida"   HeaderText="Unidad"       />
                    <asp:BoundField DataField="StockActual"    HeaderText="Stock actual" />
                    <asp:BoundField DataField="StockMinimo"    HeaderText="Stock mínimo" />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:Button ID="btnEditar" runat="server" Text="Editar"
                                CommandName="Editar"
                                CommandArgument='<%# Eval("ID_Ingrediente") %>'
                                CssClass="btn btn-primary btn-sm"
                                CausesValidation="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>