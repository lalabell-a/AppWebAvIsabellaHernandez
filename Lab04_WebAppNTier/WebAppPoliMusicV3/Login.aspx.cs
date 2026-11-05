using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Web.Security;
namespace WebAppPoliMusicV3
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender,EventArgs e) { if(!IsPostBack) lblMessage.Text=string.Empty; }
        protected void BtnLogin_Click(object sender,EventArgs e)
        {
            string login=txtLogin.Text.Trim(), password=txtPassword.Text;
            if(login.Length==0||password.Length==0) { lblMessage.Text="Escribe tu usuario y contraseña."; return; }
            if(login.Length>50||password.Length>128) { lblMessage.Text="Usuario o contraseña incorrectos."; return; }
            try
            {
                string connection=ConfigurationManager.ConnectionStrings["BDD_PoliMusicConnectionString"].ConnectionString;
                DataTable table=new BLL.PoliMusicV3.User(connection).AuthenticateUser(login,EntityLayer.PoliMusicV3.Util.Hash.GeneratePasswordHash(password));
                if(table.Rows.Count!=1) { lblMessage.Text="Usuario o contraseña incorrectos."; return; }
                DataRow row=table.Rows[0];
                int type=Convert.ToInt32(row["USER_TYPE"]);
                if(type!=EntityLayer.PoliMusicV3.Util.Constants.ID_USER_NORMAL&&type!=EntityLayer.PoliMusicV3.Util.Constants.ID_USER_ADMIN) { lblMessage.Text="Este usuario no tiene un perfil válido."; return; }
                EntityLayer.PoliMusicV3.User user=new EntityLayer.PoliMusicV3.User(Convert.ToInt32(row["ID_USER"]),row["USERNAME"].ToString(),null,row["EMAIL"].ToString(),row["BIRTHDAY"].ToString(),type,row["USER_PHOTO"].ToString());
                Session.Clear(); Session[EntityLayer.PoliMusicV3.Util.Constants.USER]=user;
                FormsAuthentication.SetAuthCookie(user.UserName,false);
                Response.Redirect(type==EntityLayer.PoliMusicV3.Util.Constants.ID_USER_ADMIN?"~/DesktopAdmin.aspx":"~/Default.aspx",false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch(DbException) { lblMessage.Text="No fue posible conectar con SQL Server. Revisa la instancia y la base del laboratorio."; }
        }
    }
}
