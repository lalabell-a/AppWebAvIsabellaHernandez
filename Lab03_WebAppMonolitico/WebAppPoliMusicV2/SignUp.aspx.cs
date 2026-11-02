using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
namespace WebAppPoliMusicV2
{
    public partial class SignUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender,EventArgs e) { }
        protected void btnSubmit_Click(object sender,EventArgs e)
        {
            lblMessage.CssClass="message";
            Entities.User user=new Entities.User();
            user.UserName=txtUserName.Text.Trim(); user.Email=txtEmail.Text.Trim(); user.Password=txtPassword.Text;
            user.UserType=Util.Constants.ID_USER_NORMAL;
            string message=UserFieldsRequiredValidations(user);
            if(message.Length>0) { lblMessage.Text=message; return; }
            if(user.Password!=txtConfirmPassword.Text) { lblMessage.Text="Las contraseñas no coinciden."; return; }
            DateTime birthday;
            if(txtBirthDate.Text.Trim().Length>0)
            {
                if(!DateTime.TryParseExact(txtBirthDate.Text.Trim(),"dd/MM/yyyy",CultureInfo.InvariantCulture,DateTimeStyles.None,out birthday)||birthday.Date>DateTime.Today||birthday.Year<1753)
                { lblMessage.Text="Ingresa una fecha válida, no futura, con formato dd/MM/yyyy."; return; }
                user.Birthday=birthday.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
            }
            string uploadedPath=null;
            try
            {
                BLL.User service=new BLL.User(ConfigurationManager.ConnectionStrings["BDD_PoliMusicConnectionString"].ConnectionString);
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
                user.Password=Util.Hash.GeneratePasswordHash(user.Password);
                service.Save(user);
                lblMessage.CssClass="message success"; lblMessage.Text="Cuenta creada correctamente. Ya puedes iniciar sesión.";
                btnSubmit.Enabled=false;
            }
            catch(SqlException ex)
            {
                if(uploadedPath!=null && File.Exists(uploadedPath)) File.Delete(uploadedPath);
                lblMessage.Text=ex.Number==2601||ex.Number==2627?"El usuario o correo ya está registrado.":"No fue posible guardar la cuenta. Revisa la conexión a SQL Server.";
            }
        }
        public string UserFieldsRequiredValidations(Entities.User user)
        {
            if(String.IsNullOrEmpty(user.UserName)) return "El usuario es obligatorio.";
            if(!Regex.IsMatch(user.UserName,@"^[A-Za-z0-9_.-]{3,50}$")) return "Usa de 3 a 50 letras, números, puntos, guiones o guion bajo para el usuario.";
            if(String.IsNullOrEmpty(user.Email)) return "El correo electrónico es obligatorio.";
            if(user.Email.Length>50||!IsValidEmail(user.Email)) return "Ingresa un correo electrónico válido de hasta 50 caracteres.";
            if(String.IsNullOrEmpty(user.Password)||user.Password.Length<8||user.Password.Length>128) return "La contraseña debe tener entre 8 y 128 caracteres.";
            return string.Empty;
        }
        public static bool IsValidEmail(string email)
        { return !String.IsNullOrEmpty(email)&&Regex.IsMatch(email,@"^[^@\s]+@[^@\s]+\.[^@\s]+$",RegexOptions.IgnoreCase); }
    }
}
