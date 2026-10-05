<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs"
    Inherits="RestauranteApp.Login" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Iniciar sesión — GastroControl</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <style>
        html, body { height: 100%; margin: 0; }
        body {
            background-image: url('Content/Images/FondoApp2.jpg');
            background-size: cover;
            background-position: center;
            background-repeat: no-repeat;
            background-attachment: fixed;
        }
        .login-overlay {
            min-height: 100vh;
            background-color: rgba(0, 0, 0, 0.55);
            display: flex;
            align-items: center;
            justify-content: center;
            flex-direction: column;
        }
        .card {
            border: none;
            border-radius: 14px;
            overflow: hidden;
            box-shadow: 0 8px 32px rgba(0,0,0,0.55);
        }
        .copyright {
            color: rgba(255,255,255,0.55);
            font-size: 13px;
            margin-top: 16px;
        }
        .card-body {
    background-color: rgba(20, 20, 20, 0.92);
    color: #fff;
}
.card-body .form-label {
    color: #ddd;
}
.card-body .form-control {
    background-color: rgba(255,255,255,0.08);
    border: 1px solid rgba(255,255,255,0.2);
    color: #fff;
}
.card-body .form-control::placeholder {
    color: rgba(255,255,255,0.35);
}
.card-body .form-control:focus {
    background-color: rgba(255,255,255,0.12);
    border-color: rgba(255,255,255,0.4);
    color: #fff;
    box-shadow: none;
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-overlay">
            <div style="width:100%; max-width:460px; padding: 0 16px;">
                <div class="card shadow-lg">
                    <div class="card-header bg-dark text-white text-center py-3">
                        <h4 class="mb-0">🍽️ GastroControl</h4>
                        <small>Ingresá tus credenciales</small>
                    </div>
                    <div class="card-body p-4">
                        <asp:Label ID="lblMensaje" runat="server"
                            CssClass="alert alert-danger d-block mb-3"
                            Text="" Visible="false" />
                        <div class="mb-3">
                            <label class="form-label fw-bold">Usuario</label>
                            <asp:TextBox ID="txtUsername" runat="server"
                                CssClass="form-control"
                                placeholder="Ingresá tu usuario" />
                            <asp:RequiredFieldValidator ID="rfvUsername" runat="server"
                                ControlToValidate="txtUsername"
                                ErrorMessage="El usuario es obligatorio."
                                CssClass="text-danger small"
                                Display="Dynamic" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label fw-bold">Contraseña</label>
                            <asp:TextBox ID="txtPassword" runat="server"
                                TextMode="Password"
                                CssClass="form-control"
                                placeholder="Ingresá tu contraseña" />
                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                                ControlToValidate="txtPassword"
                                ErrorMessage="La contraseña es obligatoria."
                                CssClass="text-danger small"
                                Display="Dynamic" />
                        </div>
                        <div class="d-grid mt-3">
                            <asp:Button ID="btnIngresar" runat="server"
                                Text="Ingresar"
                                CssClass="btn btn-dark btn-lg"
                                OnClick="btnIngresar_Click" />
                        </div>
                    </div>
                </div>
            </div>
            <p class="copyright">GastroControl © 2026</p>
        </div>
        <script src="~/Scripts/bootstrap.bundle.min.js"></script>
    </form>
</body>
</html>