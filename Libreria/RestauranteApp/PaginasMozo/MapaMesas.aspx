<%@ Page Title="Mapa de Mesas" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="MapaMesas.aspx.cs"
    Inherits="RestauranteApp.PaginasMozo.MapaMesas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 id="lblMapaMesas" runat="server" class="mb-4"></h2>
    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"
        CssClass="alert d-block mb-3" />

    <%-- GRILLA DE MESAS --%>
    <div class="card mb-4">
         <div id="lblEstadoMesas" runat="server"
             class="card-header bg-dark text-white">
         </div>
        <div class="card-body">
            <asp:GridView ID="gvMesas" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-striped table-hover"
                OnRowCommand="gvMesas_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ID_Mesa"    HeaderText="ID"        />
                    <asp:BoundField DataField="NumeroMesa" HeaderText="Nro. Mesa" />
                    <asp:BoundField DataField="Capacidad"  HeaderText="Capacidad" />
                    <asp:BoundField DataField="Estado"     HeaderText="Estado"    />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                             <asp:Button ID="btnSeleccionar" runat="server" Text="Seleccionar"
                                 CommandName="Seleccionar"
                                 CommandArgument='<%# Eval("ID_Mesa") %>'
                                 CssClass="btn btn-dark btn-sm"
                                 CausesValidation="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <%-- PANEL DE ACCIONES --%>
    <asp:Panel ID="pnlAcciones" runat="server" Visible="false">
        <div class="card">
            <div class="card-header bg-dark text-white">
                    <span id="lblMesaSeleccionadaTexto" runat="server"></span>
                    <asp:Label ID="lblMesaSeleccionada" runat="server" />
            </div>
            <div class="card-body">
                <asp:Button ID="btnAbrirMesa" runat="server"
                    Text="Abrir mesa"
                    CssClass="btn btn-success me-2"
                    OnClick="btnAbrirMesa_Click"
                    Visible="false"
                    CausesValidation="false" />
                <asp:Button ID="btnRegistrarPedido" runat="server"
                    Text="Registrar pedido"
                    CssClass="btn btn-primary me-2"
                    OnClick="btnRegistrarPedido_Click"
                    Visible="false"
                    CausesValidation="false" />
                <asp:Button ID="btnCerrarCuenta" runat="server"
                    Text="Cerrar cuenta"
                    CssClass="btn btn-danger"
                    OnClick="btnCerrarCuenta_Click"
                    Visible="false"
                    CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>