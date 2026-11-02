<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="WebAppPoliMusicV2.SignUp" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>Crear cuenta | Music Lab</title>
<link rel="stylesheet" href="Content/site.css" />
</head>
<body class="auth-body">
<main class="signup-shell">
<a class="brand" href="Login.aspx">
<span class="brand-icon">♪</span> MUSIC LAB<span class="brand-dot">.</span>
</a>
<section class="signup-card">
<span class="eyebrow muted">DALE PLAY A TU NUEVA CUENTA</span>
<h1>Tu próximo descubrimiento<br />empieza aquí.</h1>
<p class="muted">Los campos con * son obligatorios.</p>
<form id="formSignUp" runat="server" enctype="multipart/form-data" defaultbutton="btnSubmit">
<div class="form-grid">
<div>
<label for="txtUserName">Usuario *</label>
<asp:TextBox ID="txtUserName" runat="server" MaxLength="50" autocomplete="username" />
</div>
<div>
<label for="txtEmail">Correo electrónico *</label>
<asp:TextBox ID="txtEmail" runat="server" MaxLength="50" autocomplete="email" />
</div>
<div>
<label for="txtPassword">Contraseña *</label>
<asp:TextBox ID="txtPassword" runat="server" TextMode="Password" MaxLength="128" autocomplete="new-password" />
<span class="hint">Entre 8 y 128 caracteres.</span>
</div>
<div>
<label for="txtConfirmPassword">Confirmar contraseña *</label>
<asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" MaxLength="128" autocomplete="new-password" />
</div>
<div>
<label for="txtBirthDate">Fecha de nacimiento</label>
<asp:TextBox ID="txtBirthDate" runat="server" MaxLength="10" placeholder="dd/MM/yyyy" />
<span class="hint">Opcional. Ejemplo: 15/12/2000.</span>
</div>
<div>
<label for="fupPhoto">Foto de perfil</label>
<asp:FileUpload ID="fupPhoto" runat="server" accept="image/jpeg,image/png" />
<span class="hint">Opcional. JPG o PNG, máximo 10 MB.</span>
</div>
</div>
<asp:Label ID="lblMessage" runat="server" CssClass="message" role="alert" />
<asp:Button ID="btnSubmit" runat="server" Text="Crear mi cuenta →" CssClass="primary" OnClick="btnSubmit_Click" />
<p class="auth-switch">¿Ya tienes una cuenta? <a href="Login.aspx">Iniciar sesión</a>
</p>
</form>
</section>
</main>
</body>
</html>
