using System;
using System.Web.Security;
namespace WebAppPoliMusicV2.Util
{
    public class AuthPage : System.Web.UI.Page
    {
        protected Entities.User CurrentUser { get { return Session[Constants.USER] as Entities.User; } }
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
