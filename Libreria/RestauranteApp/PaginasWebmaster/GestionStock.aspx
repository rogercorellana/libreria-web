<%@ Page Title="Gestión de Stock" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="GestionStock.aspx.cs"
    Inherits="RestauranteApp.PaginasWebmaster.GestionStock" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 id="lblGestionStock" runat="server" class="mb-4"></h2>
    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"
        CssClass="alert d-block mb-3" />

    <%-- FORMULARIO INGRESO DE MERCADERÍA --%>
    <div class="card mb-4">
         <div id="lblRegistrarIngreso" runat="server"
             class="card-header bg-dark text-white">
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-4 mb-3">
                    <label id="lblIngrediente" runat="server" class="form-label fw-bold"></label>
                    <asp:DropDownList ID="ddlIngrediente" runat="server"
                        CssClass="form-select"
                        DataTextField="Nombre"
                        DataValueField="ID_Ingrediente" />
                </div>
                <div class="col-md-4 mb-3">
                    <label id="lblProveedor" runat="server" class="form-label fw-bold"></label>
                    <asp:DropDownList ID="ddlProveedor" runat="server"
                        CssClass="form-select"
                        DataTextField="Nombre"
                        DataValueField="ID_Proveedor" />
                </div>
                <div class="col-md-4 mb-3">
                    <label id="lblCantidad" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control"
                        placeholder="0.00" />
                    <asp:RequiredFieldValidator ID="rfvCantidad" runat="server"
                        ControlToValidate="txtCantidad"
                        ErrorMessage="La cantidad es obligatoria."
                        CssClass="text-danger small" Display="Dynamic" />
                    <%--<asp:RangeValidator ID="rvCantidad" runat="server"
                        ControlToValidate="txtCantidad"
                        MinimumValue="0.01" MaximumValue="99999"
                        Type="Double"
                        ErrorMessage="La cantidad debe ser mayor a cero."
                        CssClass="text-danger small" Display="Dynamic" />--%>
                </div>
            </div>
            <asp:Button ID="btnRegistrar" runat="server" Text="Registrar ingreso"
                CssClass="btn btn-success" OnClick="btnRegistrar_Click" />
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
                    <asp:BoundField DataField="Nombre"       HeaderText="Ingrediente"  />
                    <asp:BoundField DataField="StockActual"  HeaderText="Stock actual" />
                    <asp:BoundField DataField="StockMinimo"  HeaderText="Stock mínimo" />
                    <asp:BoundField DataField="UnidadMedida" HeaderText="Unidad"       />
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>

    <%-- STOCK ACTUAL --%>
    <div class="card">
         <div id="lblStockActualIngredientes" runat="server"
             class="card-header bg-dark text-white">
        </div>
        <div class="card-body">
            <asp:GridView ID="gvStock" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-striped table-hover">
                <Columns>
                    <asp:BoundField DataField="Nombre"       HeaderText="Ingrediente"  />
                    <asp:BoundField DataField="StockActual"  HeaderText="Stock actual" />
                    <asp:BoundField DataField="StockMinimo"  HeaderText="Stock mínimo" />
                    <asp:BoundField DataField="UnidadMedida" HeaderText="Unidad"       />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>