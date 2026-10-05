<%@ Page Title="Gestión de Usuarios" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ABMUsuarios.aspx.cs"
    Inherits="RestauranteApp.PaginasAdmin.ABMUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2 id="lblGestionUsuarios" runat="server" class="mb-4">Gestión de Usuarios</h2>

    <%-- Mensaje de éxito o error --%>
    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"
        CssClass="alert d-block mb-3" />

    <%-- FORMULARIO ALTA/MODIFICACIÓN --%>
    <div class="card mb-4">
        <div class="card-header bg-dark text-white">
            <asp:Label ID="lblTituloFormulario" runat="server" Text="Nuevo Usuario" />
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-4 mb-3">
                    <label id="lblNombre" runat="server" class="form-label fw-bold">Nombre</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"
                        placeholder="Ingresá el nombre" />
                    <asp:RequiredFieldValidator ID="rfvNombre" runat="server"
                        ControlToValidate="txtNombre"
                        ErrorMessage="El nombre es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>

                <div class="col-md-4 mb-3">
                    <label id="lblApellido" runat="server" class="form-label fw-bold">Apellido</label>
                    <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control"
                        placeholder="Ingresá el apellido" />
                    <asp:RequiredFieldValidator ID="rfvApellido" runat="server"
                        ControlToValidate="txtApellido"
                        ErrorMessage="El apellido es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>

                <div class="col-md-4 mb-3">
                    <label id="lblUsername" runat="server" class="form-label fw-bold">Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"
                        placeholder="Ingresá el username" />
                    <asp:RequiredFieldValidator ID="rfvUsername" runat="server"
                        ControlToValidate="txtUsername"
                        ErrorMessage="El username es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>

                <div class="col-md-4 mb-3">
                    <label id="lblPassword" runat="server" class="form-label fw-bold">Contraseña</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control"
                        TextMode="Password" placeholder="Mínimo 8 caracteres y un carácter especial" />
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                        ControlToValidate="txtPassword"
                        ErrorMessage="La contraseña es obligatoria."
                        CssClass="text-danger small" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revPassword" runat="server"
                        ControlToValidate="txtPassword"
                        ValidationExpression="^(?=.*[!@#$%^&*(),.?&quot;:{}|&lt;&gt;]).{8,}$"
                        ErrorMessage="Mínimo 8 caracteres y un carácter especial."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>

                <div class="col-md-4 mb-3">
                    <label id="lblMail" runat="server" class="form-label fw-bold">Mail</label>
                    <asp:TextBox ID="txtMail" runat="server" CssClass="form-control"
                        placeholder="Ingresá el mail" />
                    <asp:RequiredFieldValidator ID="rfvMail" runat="server"
                        ControlToValidate="txtMail"
                        ErrorMessage="El mail es obligatorio."
                        CssClass="text-danger small" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revMail" runat="server"
                        ControlToValidate="txtMail"
                        ValidationExpression="^[\w\.-]+@[\w\.-]+\.\w{2,}$"
                        ErrorMessage="El mail no tiene un formato válido."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>

                <div class="col-md-4 mb-3">
                    <label id="lblRol" runat="server" class="form-label fw-bold">Rol</label>
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Usuario Común (Mozo)"  Value="ROL-01" />
                        <asp:ListItem Text="Webmaster (Encargado)" Value="ROL-02" />
                        <asp:ListItem Text="Administrador"         Value="ROL-03" />
                    </asp:DropDownList>
                </div>

            </div>

            <%-- Botones del formulario --%>
            <asp:HiddenField ID="hfIDUsuario" runat="server" Value="0" />

            <asp:Button ID="btnGuardar" runat="server" Text="Guardar"
                CssClass="btn btn-success me-2" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar"
                CssClass="btn btn-secondary" OnClick="btnCancelar_Click"
                CausesValidation="false" />

        </div>
    </div>

    <%-- GRILLA DE USUARIOS --%>
    <div class="card">
        <div class="card-header bg-dark text-white">
        <asp:Label ID="lblListaUsuarios" runat="server" Text="Lista de Usuarios" />
    </div>
        <div class="card-body">
            <asp:GridView ID="gvUsuarios" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-striped table-hover"
                OnRowCommand="gvUsuarios_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ID_Usuario" HeaderText="ID"       />
                    <asp:BoundField DataField="Nombre"     HeaderText="Nombre"   />
                    <asp:BoundField DataField="Apellido"   HeaderText="Apellido" />
                    <asp:BoundField DataField="Username"   HeaderText="Username" />
                    <asp:BoundField DataField="Mail"       HeaderText="Mail"     />
                    <asp:BoundField DataField="Rol"        HeaderText="Rol"      />
                    <asp:TemplateField HeaderText="Estado">
                        <ItemTemplate>
                            <asp:Label ID="lblActivo" runat="server"
                                Text='<%# Eval("Activo") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Acciones">
    <ItemTemplate>
        <asp:Button ID="btnEditar" runat="server" Text="Editar"
            CommandName="Editar"
            CommandArgument='<%# Eval("ID_Usuario") %>'
            CssClass="btn btn-primary btn-sm me-1"
            CausesValidation="false" />
        <asp:Button ID="btnCambiarEstado" runat="server" Text="Bloquear/Desbloquear"
            CommandName="CambiarEstado"
            CommandArgument='<%# Eval("ID_Usuario") %>'
            CssClass="btn btn-warning btn-sm"
            CausesValidation="false" />
    </ItemTemplate>
</asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>