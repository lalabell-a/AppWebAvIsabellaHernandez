using System;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
namespace WebAppPoliMusicV3
{
    public partial class SignUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender,EventArgs e) { }
        protected void btnSubmit_Click(object sender,EventArgs e)
        {
            lblMessage.CssClass="message";
            EntityLayer.PoliMusicV3.User user=new EntityLayer.PoliMusicV3.User();
            user.UserName=txtUserName.Text.Trim(); user.Email=txtEmail.Text.Trim(); user.Password=txtPassword.Text;
            user.UserType=EntityLayer.PoliMusicV3.Util.Constants.ID_USER_NORMAL;
            string message=BLL.PoliMusicV3.RegistrationRules.Validate(user,txtConfirmPassword.Text,txtBirthDate.Text);
            if(message.Length>0) { lblMessage.Text=message; return; }
            string uploadedPath=null;
            try
            {
                BLL.PoliMusicV3.User service=new BLL.PoliMusicV3.User(ConfigurationManager.ConnectionStrings["BDD_PoliMusicConnectionString"].ConnectionString);
                message=service.ValidationsDuplicated(user);
                if(message.Length>0) { lblMessage.Text=message; return; }
                user.Photo=ConfigurationManager.AppSettings["userPhotoPath"]+"default.jpg";
                if(fupPhoto.HasFile)
                {
                    string name=Guid.NewGuid().ToString("N"), folder=ConfigurationManager.AppSettings["userPhotoPath"];
                    message=new FileManagement().SaveImageOnServer(fupPhoto,folder,name);
                    if(message.Length>0) { lblMessage.Text=message; return; }
                    user.Photo=folder+name+Path.GetExtension(fupPhoto.FileName).ToLowerInvariant();
                    uploadedPath=Server.MapPath(user.Photo);
                }
                user.Password=EntityLayer.PoliMusicV3.Util.Hash.GeneratePasswordHash(user.Password);
                service.Save(user);
                lblMessage.CssClass="message success"; lblMessage.Text="Cuenta creada correctamente. Ya puedes iniciar sesión.";
                btnSubmit.Enabled=false;
            }
            catch(DbException)
            {
                if(uploadedPath!=null && File.Exists(uploadedPath)) File.Delete(uploadedPath);
                lblMessage.Text="No fue posible guardar la cuenta. Revisa la conexión a SQL Server.";
            }
        }
        public string UserFieldsRequiredValidations(EntityLayer.PoliMusicV3.User user) { return BLL.PoliMusicV3.RegistrationRules.Validate(user,user.Password,txtBirthDate.Text); }
        public static bool IsValidEmail(string email) { return BLL.PoliMusicV3.RegistrationRules.IsValidEmail(email); }
    }
}
