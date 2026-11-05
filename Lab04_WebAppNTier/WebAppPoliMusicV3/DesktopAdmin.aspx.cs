using System;
namespace WebAppPoliMusicV3
{
    public partial class DesktopAdmin : Util.AuthPage
    {
        protected void Page_Load(object sender,EventArgs e)
        { if(CurrentUser.UserType!=EntityLayer.PoliMusicV3.Util.Constants.ID_USER_ADMIN) { Response.Redirect("~/Song.aspx"); return; } litAdmin.Text=CurrentUser.UserName; }
    }
}
