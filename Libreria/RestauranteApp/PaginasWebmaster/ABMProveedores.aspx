<%@ Page Title="Gestión de Proveedores" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ABMProveedores.aspx.cs"
    Inherits="RestauranteApp.PaginasWebmaster.ABMProveedores" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 id="lblGestionProveedores" runat="server" class="mb-4"></h2>
    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"
        CssClass="alert d-block mb-3" />

    <%-- FORMULARIO --%>
    <div class="card mb-4">
        <div class="card-header bg-dark text-white">
            <asp:Label ID="lblTituloFormulario" runat="server" Text="Nuevo Proveedor" />
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-4 mb-3">
                    <label id="lblNombre" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"
                        placeholder="" />
                    <asp:RequiredFieldValidator ID="rfvNombre" runat="server"
                        ControlToValidate="txtNombre"
                        ErrorMessage="El nombre es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="col-md-4 mb-3">
                    <label id="lblCUIT" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtCUIT" runat="server" CssClass="form-control"
                        placeholder="" />
                    <asp:RequiredFieldValidator ID="rfvCUIT" runat="server"
                        ControlToValidate="txtCUIT"
                        ErrorMessage="El CUIT es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="col-md-4 mb-3">
                    <label id="lblTelefono" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control"
                        placeholder="" />
                </div>
                <div class="col-md-4 mb-3">
                    <label id="lblEmail" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"
                        placeholder="" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server"
                        ControlToValidate="txtEmail"
                        ValidationExpression="^[\w\.-]+@[\w\.-]+\.\w{2,}$"
                        ErrorMessage="El email no tiene un formato válido."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>
            </div>
            <asp:HiddenField ID="hfIDProveedor" runat="server" Value="0" />
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar"
                CssClass="btn btn-success me-2" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar"
                CssClass="btn btn-secondary" OnClick="btnCancelar_Click"
                CausesValidation="false" />
        </div>
    </div>

    <%-- GRILLA --%>
    <div class="card">
        <div id="lblListaProveedores" runat="server" class="card-header bg-dark text-white">
        </div>
        <div class="card-body">
            <asp:GridView ID="gvProveedores" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-striped table-hover"
                OnRowCommand="gvProveedores_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ID_Proveedor" HeaderText="ID"       />
                    <asp:BoundField DataField="Nombre"       HeaderText="Nombre"   />
                    <asp:BoundField DataField="CUIT"         HeaderText="CUIT"     />
                    <asp:BoundField DataField="Telefono"     HeaderText="Teléfono" />
                    <asp:BoundField DataField="Email"        HeaderText="Email"    />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:Button ID="btnEditar" runat="server" Text="Editar"
                                CommandName="Editar"
                                CommandArgument='<%# Eval("ID_Proveedor") %>'
                                CssClass="btn btn-primary btn-sm"
                                CausesValidation="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>