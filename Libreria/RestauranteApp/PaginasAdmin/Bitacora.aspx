<%@ Page Title="Bitácora" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Bitacora.aspx.cs"
    Inherits="RestauranteApp.PaginasAdmin.Bitacora" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 id="lblBitacora" runat="server" class="mb-4"></h2>
    <%-- FILTROS --%>
    <div class="card mb-4">
         <div id="lblFiltros" runat="server" class="card-header bg-dark text-white">
         </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-3 mb-3">
                    <label id="lblUsuario" runat="server" class="form-label fw-bold"></label>
                    <asp:DropDownList ID="ddlUsuario" runat="server"
                        CssClass="form-select"
                        DataTextField="Username"
                        DataValueField="ID_Usuario">
                    </asp:DropDownList>
                </div>
                <div class="col-md-3 mb-3">
                    <label id="lblTipoActividad" runat="server" class="form-label fw-bold"></label>
                    <asp:DropDownList ID="ddlActividad" runat="server"
                        CssClass="form-select"
                        DataTextField="TipoActividad"
                        DataValueField="ID_Actividad">
                    </asp:DropDownList>
                </div>
                <div class="col-md-2 mb-3">
                    <label id="lblFechaDesde" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtFechaDesde" runat="server"
                        CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-2 mb-3">
                    <label id="lblFechaHasta" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtFechaHasta" runat="server"
                        CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-2 mb-3 d-flex align-items-end">
                    <asp:Button ID="btnFiltrar" runat="server"
                        Text="Filtrar"
                        CssClass="btn btn-dark w-100"
                        OnClick="btnFiltrar_Click"
                        CausesValidation="false" />
                </div>
            </div>
            <asp:Button ID="btnLimpiar" runat="server"
                Text="Limpiar filtros"
                CssClass="btn btn-secondary"
                OnClick="btnLimpiar_Click"
                CausesValidation="false" />
        </div>
    </div>
    <%-- GRILLA --%>
    <div class="card">
        <div id="lblRegistroAcciones" runat="server" class="card-header bg-dark text-white">
        </div>
        <div class="card-body">
            <asp:GridView ID="gvBitacora" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-striped table-hover"
                OnRowDataBound="gvBitacora_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="ID_Log"            HeaderText="ID"          />
                    <asp:BoundField DataField="FechaHora"         HeaderText="Fecha/Hora"  DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                    <asp:BoundField DataField="UsernameIngresado"  HeaderText="Usuario"     />
                    <asp:BoundField DataField="TipoActividad"     HeaderText="Actividad"   />
                    <asp:BoundField DataField="Descripcion"       HeaderText="Descripción" />
                    <asp:TemplateField HeaderText="Criticidad">
                        <ItemTemplate>
                            <asp:Label ID="lblNivel" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>

