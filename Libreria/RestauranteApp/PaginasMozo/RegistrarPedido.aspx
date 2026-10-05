<%@ Page Title="Registrar Pedido" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="RegistrarPedido.aspx.cs"
    Inherits="RestauranteApp.PaginasMozo.RegistrarPedido" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 id="lblRegistrarPedido" runat="server" class="mb-4"></h2>
    <div class="alert alert-info mb-4">
           <span id="lblMesaTexto" runat="server"></span>
           <strong><asp:Label ID="lblMesa" runat="server" /></strong>
    </div>

    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"
        CssClass="alert d-block mb-3" />

    <%-- FORMULARIO AGREGAR PLATO --%>
    <div class="card mb-4">
    <div id="lblAgregarPlato" runat="server"
          class="card-header bg-dark text-white">
    </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-6 mb-3">
                    <label id="lblPlato" runat="server" class="form-label fw-bold"></label>
                    <asp:DropDownList ID="ddlPlatos" runat="server"
                        CssClass="form-select"
                        DataTextField="Nombre"
                        DataValueField="ID_Plato" />
                </div>
                <div class="col-md-3 mb-3">
                    <label id="lblCantidad" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtCantidad" runat="server"
                        CssClass="form-control" placeholder="1" />
                    <asp:RequiredFieldValidator ID="rfvCantidad" runat="server"
                        ControlToValidate="txtCantidad"
                        ErrorMessage="La cantidad es obligatoria."
                        CssClass="text-danger small" Display="Dynamic" />
                    <asp:RangeValidator ID="rvCantidad" runat="server"
                        ControlToValidate="txtCantidad"
                        MinimumValue="1" MaximumValue="99"
                        Type="Integer"
                        ErrorMessage="La cantidad debe ser entre 1 y 99."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="col-md-3 mb-3 d-flex align-items-end">
                    <asp:Button ID="btnAgregarPlato" runat="server"
                        Text="Agregar plato"
                        CssClass="btn btn-primary w-100"
                        OnClick="btnAgregarPlato_Click" />
                </div>
            </div>
        </div>
    </div>

    <%-- DETALLE DEL PEDIDO --%>
    <div class="card mb-4">
        <div id="lblDetallePedido" runat="server"
            class="card-header bg-dark text-white">
        </div>
        <div class="card-body">
            <asp:GridView ID="gvDetalle" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-striped"
                OnRowCommand="gvDetalle_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ID_Detalle"     HeaderText="ID"       Visible="false" />
                    <asp:BoundField DataField="NombrePlato"    HeaderText="Plato"    />
                    <asp:BoundField DataField="Cantidad"       HeaderText="Cantidad" />
                    <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio"   DataFormatString="${0:N2}" />
                    <asp:BoundField DataField="Subtotal"       HeaderText="Subtotal" DataFormatString="${0:N2}" />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:Button ID="btnEliminar" runat="server" Text="Eliminar"
                                CommandName="EliminarDetalle"
                                CommandArgument='<%# Eval("ID_Detalle") %>'
                                CssClass="btn btn-danger btn-sm"
                                CausesValidation="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div class="text-end mt-3">
                <h4><span id="lblTotalTexto" runat="server"></span>
                <asp:Label ID="lblTotal" runat="server" Text="$0.00" /></h4>
            </div>
        </div>
    </div>

    <%-- BOTONES --%>
    <asp:Button ID="btnConfirmarPedido" runat="server"
        Text="Confirmar pedido"
        CssClass="btn btn-success me-2"
        OnClick="btnConfirmarPedido_Click"
        CausesValidation="false" />
    <asp:Button ID="btnSolicitarCuenta" runat="server"
        Text="Solicitar cuenta"
        CssClass="btn btn-warning me-2"
        OnClick="btnSolicitarCuenta_Click"
        CausesValidation="false" />
    <asp:Button ID="btnVolver" runat="server"
        Text="Volver al mapa de mesas"
        CssClass="btn btn-secondary"
        OnClick="btnVolver_Click"
        CausesValidation="false" />
</asp:Content>