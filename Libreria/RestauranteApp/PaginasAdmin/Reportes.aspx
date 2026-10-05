<%@ Page Title="Reportes" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Reportes.aspx.cs"
    Inherits="RestauranteApp.PaginasAdmin.Reportes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2 id="lblReportes" runat="server" class="mb-4"></h2>

    <asp:Label ID="lblMensaje" runat="server" Text="" Visible="false"
        CssClass="alert d-block mb-3" />

    <%-- FILTROS --%>
    <div class="card mb-4">
         <div id="lblFiltrarFecha" runat="server" class="card-header bg-dark text-white">
         </div>
        <div class="card-body">
            <div class="row">

                <div class="col-md-4 mb-3">
                    <label id="lblFechaDesde" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtFechaDesde" runat="server"
                        CssClass="form-control" TextMode="Date" />
                    <asp:RequiredFieldValidator ID="rfvFechaDesde" runat="server"
                        ControlToValidate="txtFechaDesde"
                        ErrorMessage="La fecha desde es obligatoria."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>

                <div class="col-md-4 mb-3">
                    <label id="lblFechaHasta" runat="server" class="form-label fw-bold"></label>
                    <asp:TextBox ID="txtFechaHasta" runat="server"
                        CssClass="form-control" TextMode="Date" />
                    <asp:RequiredFieldValidator ID="rfvFechaHasta" runat="server"
                        ControlToValidate="txtFechaHasta"
                        ErrorMessage="La fecha hasta es obligatoria."
                        CssClass="text-danger small" Display="Dynamic" />
                </div>

                <div class="col-md-4 mb-3 d-flex align-items-end">
                    <asp:Button ID="btnGenerar" runat="server"
                        Text="Generar reporte"
                        CssClass="btn btn-dark w-100"
                        OnClick="btnGenerar_Click" />
                </div>

            </div>
        </div>
    </div>

    <%-- RESULTADOS --%>
    <asp:Panel ID="pnlResultados" runat="server" Visible="false">

        <%-- RESUMEN DE VENTAS --%>
        <div class="row mb-4">
            <div class="col-md-4">
                <div class="card text-white bg-success">
                    <div class="card-body text-center">
                        <h5 id="lblTotalVendido" runat="server" class="card-title"></h5>
                        <h3><asp:Label ID="lblTotalVentas" runat="server" Text="$0.00" /></h3>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card text-white bg-primary">
                    <div class="card-body text-center">
                        <h5 id="lblPagosEfectivo" runat="server" class="card-title"></h5>
                        <h3><asp:Label ID="lblTotalEfectivo" runat="server" Text="$0.00" /></h3>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card text-white bg-info">
                    <div class="card-body text-center">
                        <h5 id="lblPagosTarjeta" runat="server" class="card-title"></h5>
                        <h3><asp:Label ID="lblTotalTarjeta" runat="server" Text="$0.00" /></h3>
                    </div>
                </div>
            </div>
        </div>

        <%-- PLATOS MÁS VENDIDOS --%>
        <div class="card">
             <div id="lblPlatosMasVendidos" runat="server" class="card-header bg-dark text-white">
             </div>
            <div class="card-body">
                <asp:GridView ID="gvPlatosVendidos" runat="server"
                    AutoGenerateColumns="false"
                    CssClass="table table-bordered table-striped table-hover">
                    <Columns>
                        <asp:BoundField DataField="NombrePlato"    HeaderText="Plato"             />
                        <asp:BoundField DataField="TotalUnidades"  HeaderText="Unidades vendidas" />
                        <asp:BoundField DataField="TotalRecaudado" HeaderText="Total recaudado"   DataFormatString="${0:N2}" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

    </asp:Panel>

</asp:Content>

