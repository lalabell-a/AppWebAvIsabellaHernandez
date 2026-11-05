using System;
using EntityLayer.PoliMusicV3.Util;
namespace WebAppPoliMusicV3 {
 public partial class SiteMaster : System.Web.UI.MasterPage {
  protected void Page_Load(object sender,EventArgs e) {
   var user=Session[Constants.USER] as EntityLayer.PoliMusicV3.User;
   litUser.Text=user==null?String.Empty:user.UserName;
   lnkAdmin.Visible=user!=null && user.UserType==Constants.ID_USER_ADMIN;
  }
 }
}
