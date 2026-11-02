<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DesktopAdmin.aspx.cs" Inherits="WebAppPoliMusicV2.DesktopAdmin" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>Administración | Music Lab</title>
<link rel="stylesheet" href="Content/site.css" />
</head>
<body class="auth-body">
<main class="signup-shell">
<a class="brand" href="Song.aspx">
<span class="brand-icon">♪</span> MUSIC LAB.</a>
<section class="signup-card">
<span class="eyebrow muted">PERFIL ADMINISTRADOR</span>
<h1>Tu biblioteca está lista.</h1>
<p>Bienvenida, <asp:Literal ID="litAdmin" runat="server" Mode="Encode" />. Accede a las canciones para consultar el catálogo y reproducir tus archivos.</p>
<a class="hero-button" href="Song.aspx">Abrir biblioteca →</a>
<p>
<a href="Logout.aspx">Cerrar sesión</a>
</p>
</section>
</main>
</body>
</html>
