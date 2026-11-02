using System;
namespace WebAppPoliMusicV2
{
    public partial class DesktopAdmin : Util.AuthPage
    {
        protected void Page_Load(object sender,EventArgs e)
        { if(CurrentUser.UserType!=Util.Constants.ID_USER_ADMIN) { Response.Redirect("~/Song.aspx"); return; } litAdmin.Text=CurrentUser.UserName; }
    }
}
