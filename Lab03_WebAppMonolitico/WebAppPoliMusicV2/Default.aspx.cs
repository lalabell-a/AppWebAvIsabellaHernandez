using System;
namespace WebAppPoliMusicV2
{
    public partial class Default : Util.AuthPage
    { protected void Page_Load(object sender,EventArgs e) { if(!IsPostBack) Response.Redirect("~/Song.aspx"); } }
}
