<%@ Page Title="Cerrar Cuenta" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="CerrarCuenta.aspx.cs"
    Inherits="RestauranteApp.PaginasMozo.CerrarCuenta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 id="lblCerrarCuenta" runat="server" class="mb-4"></h2>
    <div class="alert alert-info mb-4">
        <span id="lblMesaTexto" runat="server"></span>
        <strong><asp:Label ID="lblMesa" runat="server" /></strong>
    </div>
    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"
        CssClass="alert d-block mb-3" />

    <%-- DETALLE DEL PEDIDO --%>
    <div class="card mb-4">
     <div id="lblDetallePedido" runat="server"
     class="card-header bg-dark text-white">
    </div>
        <div class="card-body">
            <asp:GridView ID="gvDetalle" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-striped">
                <Columns>
                    <asp:BoundField DataField="NombrePlato"    HeaderText="Plato"    />
                    <asp:BoundField DataField="Cantidad"       HeaderText="Cantidad" />
                    <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio"   DataFormatString="${0:N2}" />
                    <asp:BoundField DataField="Subtotal"       HeaderText="Subtotal" DataFormatString="${0:N2}" />
                </Columns>
            </asp:GridView>
            <div class="text-end mt-3">
                <h4>
                     <span id="lblTotalTexto" runat="server"></span>
                     <asp:Label ID="lblTotal" runat="server" Text="$0.00" />
                </h4>
            </div>
        </div>
    </div>
    <%-- MÉTODO DE PAGO --%>
    <div class="card mb-4">
         <div id="lblMetodoPago" runat="server"
             class="card-header bg-dark text-white">
         </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-4">
                    <asp:DropDownList ID="ddlMetodoPago" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Efectivo" Value="Efectivo" />
                        <asp:ListItem Text="Tarjeta"  Value="Tarjeta"  />
                    </asp:DropDownList>
                </div>
            </div>
        </div>
    </div>

    <%-- BOTONES --%>
    <asp:Button ID="btnConfirmarPago" runat="server"
        Text="Confirmar pago"
        CssClass="btn btn-success me-2"
        OnClick="btnConfirmarPago_Click"
        CausesValidation="false" />
    <asp:Button ID="btnVolver" runat="server"
        Text="Volver al mapa de mesas"
        CssClass="btn btn-secondary"
        OnClick="btnVolver_Click"
        CausesValidation="false" />
        <asp:Button ID="btnLiberarMesa" runat="server"
        Text="Liberar mesa"
        CssClass="btn btn-warning me-2"
        OnClick="btnLiberarMesa_Click"
        Visible="false"
        CausesValidation="false" />
</asp:Content>