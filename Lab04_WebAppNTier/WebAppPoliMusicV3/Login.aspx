<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebAppPoliMusicV3.Login" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>Iniciar sesión | PoliMusic</title>
<link rel="stylesheet" href="Content/site.css" />
</head>
<body class="auth-body">
<main class="auth-shell">
<section class="auth-art">
<a class="brand" href="Login.aspx">
<span class="brand-icon">♪</span> POLIMUSIC<span class="brand-dot">.</span>
</a>
<div>
<span class="eyebrow">TU ESPACIO PARA ESCUCHAR</span>
<h1>Un buen día<br />empieza con<br />
<em>una canción.</em>
</h1>
<p>Explora tu biblioteca, encuentra tu ritmo y dale play a lo que te mueve.</p>
<div class="record">
<div class="record-center">♪</div>
</div>
</div>
<p class="small">Aplicaciones Web Avanzadas · Laboratorio 04</p>
</section>
<section class="auth-panel">
<div class="auth-content">
<span class="eyebrow muted">BIENVENIDA DE NUEVO</span>
<h2>Tu música te espera.</h2>
<p class="muted">Ingresa para descubrir la biblioteca.</p>
<form id="formLogin" runat="server" defaultbutton="btnLogin">
<label for="txtLogin">Usuario</label>
<asp:TextBox ID="txtLogin" runat="server" MaxLength="50" autocomplete="username" />
<label for="txtPassword">Contraseña</label>
<asp:TextBox ID="txtPassword" runat="server" TextMode="Password" MaxLength="128" autocomplete="current-password" />
<asp:Label ID="lblMessage" runat="server" CssClass="message" role="alert" />
<asp:Button ID="btnLogin" runat="server" Text="Iniciar sesión →" CssClass="primary" OnClick="BtnLogin_Click" />
<p class="auth-switch">¿Primera vez aquí? <a href="SignUp.aspx">Crear una cuenta</a>
</p>
</form>
</div>
</section>
</main>
</body>
</html>
