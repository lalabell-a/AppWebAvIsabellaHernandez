<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Song.aspx.cs" Inherits="WebAppPoliMusicV2.Song" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>Biblioteca | Music Lab</title>
<link rel="stylesheet" href="Content/site.css" />
</head>
<body>
<form id="formSong" runat="server" defaultbutton="btnSearch">
<aside class="sidebar">
<a class="brand" href="Song.aspx">
<span class="brand-icon">♪</span> MUSIC LAB<span class="brand-dot">.</span>
</a>
<span class="nav-caption">TU MÚSICA</span>
<a class="nav-item active" href="Song.aspx">♫ &nbsp; Biblioteca</a>
<asp:HyperLink ID="lnkAdmin" runat="server" CssClass="nav-item" NavigateUrl="~/DesktopAdmin.aspx" Visible="false">◈ &nbsp; Administración</asp:HyperLink>
<div class="sidebar-bottom">
<span class="avatar">♫</span>
<span>
<strong>
<asp:Literal ID="litUser" runat="server" Mode="Encode" />
</strong>
<small>Tu espacio musical</small>
</span>
<a href="Logout.aspx" title="Cerrar sesión" aria-label="Cerrar sesión">↗</a>
</div>
</aside>
<main class="library-main">
<header class="topbar">
<span class="breadcrumb">Tu música <span>/</span>
<strong>Biblioteca</strong>
</span>
<span class="status">
<i>
</i> LISTO PARA ESCUCHAR</span>
</header>
<section class="library-hero">
<div>
<span class="eyebrow">EL SOUNDTRACK DE TU DÍA</span>
<h1>Encuentra tu ritmo.<br />
<em>Hazlo tuyo.</em>
</h1>
<p>Todas tus canciones, en un solo lugar.<br />Descubre, elige y dale play.</p>
<a href="#songs" class="hero-button">Explorar canciones ↓</a>
</div>
<div class="hero-vinyl" aria-hidden="true">
<div>♪<small>MUSIC LAB</small>
</div>
</div>
<span class="hero-number">VOL. 03</span>
</section>
<section id="songs" class="songs-section">
<div class="section-title">
<div>
<span class="eyebrow muted">DALE PLAY A LO QUE TE GUSTA</span>
<h2>Tu biblioteca <span class="count">
<asp:Literal ID="litCount" runat="server" />
</span>
</h2>
</div>
<div class="search-group">
<label class="sr-only" for="txtSearch">Buscar canción</label>
<asp:TextBox ID="txtSearch" runat="server" MaxLength="100" placeholder="Buscar una canción..." />
<asp:Button ID="btnSearch" runat="server" Text="Buscar" CssClass="search-button" OnClick="btnSearch_Click" />
<asp:LinkButton ID="btnClear" runat="server" CssClass="clear-button" OnClick="btnClear_Click">Limpiar</asp:LinkButton>
</div>
</div>
<asp:Label ID="lblMessage" runat="server" CssClass="message" role="alert" />
<div class="grid-wrap">
<asp:GridView ID="gvSongs" runat="server" AutoGenerateColumns="false" CssClass="song-grid" GridLines="None" AllowPaging="true" PageSize="8" DataKeyNames="ID_SONG" OnPageIndexChanging="gvSongs_PageIndexChanging" EmptyDataText="No hay canciones que coincidan con la búsqueda." UseAccessibleHeader="true">
<Columns>
<asp:BoundField DataField="ID_SONG" HeaderText="#" />
<asp:TemplateField HeaderText="CANCIÓN">
<ItemTemplate>
<div class="track">
<span class="track-cover" aria-hidden="true">♫</span>
<div>
<strong>
<%# System.Web.HttpUtility.HtmlEncode(Convert.ToString(Eval("SONG_NAME"))) %>
</strong>
<small>Biblioteca Music Lab · MP3</small>
</div>
</div>
</ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="PLAYS" HeaderText="REPRODUCCIONES" />
<asp:TemplateField HeaderText="REPRODUCTOR">
<ItemTemplate>
<audio controls="controls" preload="none" aria-label='<%# AudioLabel(Eval("SONG_NAME")) %>' src='<%# AudioUrl(Eval("ID_SONG")) %>'>Tu navegador no admite audio HTML5.</audio>
<span class="audio-error" role="status">
</span>
</ItemTemplate>
</asp:TemplateField>
</Columns>
<PagerStyle CssClass="pager" />
<PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última" />
</asp:GridView>
</div>
<p class="library-note">Pausa, avanza o ajusta el volumen desde cada reproductor.</p>
</section>
<footer>Hecho para escuchar. <span>Music Lab · Isabella Hernández</span>
</footer>
</main>
</form>
<script src="Content/player.js">
</script>
</body>
</html>
