<%@ Page Title="Canciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Song.aspx.cs" Inherits="WebAppPoliMusicV3.Song" %>
<asp:Content ID="contentSong" ContentPlaceHolderID="MainContent" runat="server">
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
<div>♪<small>POLIMUSIC</small>
</div>
</div>
<span class="hero-number">VOL. 04</span>
</section>
<asp:UpdatePanel ID="songsUpdate" runat="server" UpdateMode="Conditional"><ContentTemplate><section id="songs" class="songs-section">
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
<small>Biblioteca PoliMusic · MP3</small>
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
</ContentTemplate></asp:UpdatePanel>
</asp:Content>
