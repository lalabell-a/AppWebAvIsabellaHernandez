using System;
using System.Web.Security;
namespace WebAppPoliMusicV3.Util
{
    public class AuthPage : System.Web.UI.Page
    {
        protected EntityLayer.PoliMusicV3.User CurrentUser { get { return Session[EntityLayer.PoliMusicV3.Util.Constants.USER] as EntityLayer.PoliMusicV3.User; } }
        protected override void OnInit(EventArgs e)
        {
            if(!Request.IsAuthenticated || CurrentUser==null)
            { FormsAuthentication.SignOut(); Response.Redirect("~/Login.aspx",true); return; }
            ViewStateUserKey=Session.SessionID;
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            base.OnInit(e);
        }
    }
}
