using System;
using System.Globalization;
using System.Text.RegularExpressions;
namespace BLL.PoliMusicV3 {
 public static class RegistrationRules {
  public static string Validate(EntityLayer.PoliMusicV3.User user,string confirmation,string birthDate) {
   if(user==null) return "El usuario es obligatorio.";
   user.UserName=(user.UserName??String.Empty).Trim(); user.Email=(user.Email??String.Empty).Trim();
   if(!Regex.IsMatch(user.UserName,@"^[A-Za-z0-9_.-]{3,50}$")) return "Usa de 3 a 50 letras, números, puntos, guiones o guion bajo para el usuario.";
   if(user.Email.Length>50 || !IsValidEmail(user.Email)) return "Ingresa un correo electrónico válido de hasta 50 caracteres.";
   if(String.IsNullOrEmpty(user.Password)||user.Password.Length<8||user.Password.Length>128) return "La contraseña debe tener entre 8 y 128 caracteres.";
   if(user.Password!=confirmation) return "Las contraseñas no coinciden.";
   user.Birthday=null;
   if(!String.IsNullOrWhiteSpace(birthDate)) {
    DateTime date;
    if(!DateTime.TryParseExact(birthDate.Trim(),"dd/MM/yyyy",CultureInfo.InvariantCulture,DateTimeStyles.None,out date)||date.Date>DateTime.Today||date.Year<1753) return "Ingresa una fecha válida, no futura, con formato dd/MM/yyyy.";
    user.Birthday=date.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
   }
   return String.Empty;
  }
  public static bool IsValidEmail(string email) { return !String.IsNullOrWhiteSpace(email)&&Regex.IsMatch(email,@"^[^@\s]+@[^@\s]+\.[^@\s]+$",RegexOptions.IgnoreCase); }
 }
}
