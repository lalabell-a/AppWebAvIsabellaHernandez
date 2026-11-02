using System;
using System.IO;
using WebAppPoliMusicV2;
class LabChecks
{
    static int total=0;
    static void Check(bool ok,string name) { total++; if(!ok) throw new Exception(name); Console.WriteLine("PASS "+name); }
    static void Main()
    {
        Check(UtilHash("12345678")=="7C222FB2927D828AF22F592134E8932480637C0D","SHA1 compatible con guia");
        Check(SignUp.IsValidEmail("laboratorio@example.test"),"correo valido");
        Check(!SignUp.IsValidEmail("correo-invalido"),"correo invalido");
        SignUp page=new SignUp();
        WebAppPoliMusicV2.Entities.User user=new WebAppPoliMusicV2.Entities.User();
        user.UserName="isabella.hernandez"; user.Email="laboratorio@example.test"; user.Password="Prueba123";
        Check(page.UserFieldsRequiredValidations(user)==String.Empty,"campos validos");
        user.UserName=""; Check(page.UserFieldsRequiredValidations(user)!="","usuario requerido");
        user.UserName="../archivo"; Check(page.UserFieldsRequiredValidations(user)!="","usuario con ruta rechazado");
        user.UserName="usuario"; user.Password="123"; Check(page.UserFieldsRequiredValidations(user)!="","contrasena corta rechazada");
        user.Password="Prueba123"; user.Email=new string('a',51)+"@example.test"; Check(page.UserFieldsRequiredValidations(user)!="","limite correo");
        string pbkdf=WebAppPoliMusicV2.Util.Hash.HashPassword("prueba");
        Check(Convert.FromBase64String(pbkdf).Length==36,"PBKDF2 compatible");
        Console.WriteLine("TOTAL "+total+" PASS");
    }
    static string UtilHash(string value) { return WebAppPoliMusicV2.Util.Hash.GeneratePasswordHash(value); }
}
