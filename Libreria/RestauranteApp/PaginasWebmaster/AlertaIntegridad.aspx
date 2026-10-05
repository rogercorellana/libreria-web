<%@ Page Title="Alerta de Integridad" Language="C#" 
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" 
    CodeBehind="AlertaIntegridad.aspx.cs" 
    Inherits="RestauranteApp.PaginasWebmaster.AlertaIntegridad" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="container mt-4">

    <div class="alert alert-danger">
        <h4 class="alert-heading">
            <span class="glyphicon glyphicon-warning-sign"></span>
            Se detectó corrupción en la base de datos
        </h4>
        <p>Los siguientes registros fueron alterados fuera del sistema:</p>
        <ul runat="server" id="listaErrores"></ul>
    </div>

    <div class="panel panel-default">
        <div class="panel-heading"><strong>¿Qué desea hacer?</strong></div>
        <div class="panel-body">
            <p class="text-muted">Seleccione una de las siguientes opciones para resolver la situación:</p>

            <div style="display: flex; gap: 12px; flex-wrap: wrap; margin-top: 16px;">

                <asp:Button ID="btnRestaurar" runat="server"
                    Text="Restaurar backup anterior"
                    CssClass="btn btn-danger"
                    OnClick="btnRestaurar_Click"
                    OnClientClick="return confirm('¿Restaurar la base de datos desde el último backup? Se perderán los cambios realizados después del backup.');" />

                <asp:Button ID="btnRecalcular" runat="server"
                    Text="Aceptar modificación y recalcular"
                    CssClass="btn btn-warning"
                    OnClick="btnRecalcular_Click"
                    OnClientClick="return confirm('¿Recalcular todos los dígitos verificadores? Esto acepta los datos actuales como válidos y quedará registrado en la bitácora.');" />

                <asp:Button ID="btnCancelar" runat="server"
                    Text="Continuar sin resolver"
                    CssClass="btn btn-default"
                    OnClick="btnCancelar_Click" />

            </div>
        </div>
    </div>

    <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert d-block mt-3" />

</div>

</asp:Content>
