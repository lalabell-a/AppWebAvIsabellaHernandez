using System;
using System.Reflection;
using BLL.PoliMusicV3;
using EntityLayer.PoliMusicV3.Util;
class PruebasCapas {
 static int count;
 static void Check(bool ok,string name) { if(!ok) throw new Exception("FALLO: "+name); Console.WriteLine("APROBADA: "+name); count++; }
 static EntityLayer.PoliMusicV3.User Valid() { return new EntityLayer.PoliMusicV3.User { UserName="isabella.hernandez",Email="lab04@example.test",Password="Laboratorio04!2026" }; }
 static string Validate(EntityLayer.PoliMusicV3.User u,string confirmation=null,string date="") { return RegistrationRules.Validate(u,confirmation??u.Password,date); }
 static void Main() {
  var u=Valid(); Check(Validate(u)=="","registro válido");
  u=Valid(); u.UserName="  isabella.hernandez  "; Check(Validate(u)==""&&u.UserName=="isabella.hernandez","normalizar usuario");
  u=Valid(); u.UserName="ab"; Check(Validate(u)!="","usuario corto");
  u=Valid(); u.UserName=new String('a',51); Check(Validate(u)!="","usuario largo");
  u=Valid(); u.UserName="../foto"; Check(Validate(u)!="","usuario con ruta rechazado");
  u=Valid(); u.Email="correo-sin-arroba"; Check(Validate(u)!="","correo inválido");
  u=Valid(); u.Email=new String('a',45)+"@x.test"; Check(Validate(u)!="","límite del correo");
  u=Valid(); u.Password="1234567"; Check(Validate(u)!="","contraseña corta");
  u=Valid(); u.Password=new String('x',129); Check(Validate(u)!="","contraseña larga");
  Check(Validate(Valid(),"diferente")!="","confirmación diferente");
  u=Valid(); Check(Validate(u,null,"29/02/2000")==""&&u.Birthday=="2000-02-29","fecha bisiesta normalizada");
  Check(Validate(Valid(),null,"31/02/2000")!="","fecha imposible");
  Check(Validate(Valid(),null,"01/01/1700")!="","fecha anterior a SQL datetime");
  Check(Validate(Valid(),null,DateTime.Today.AddDays(1).ToString("dd/MM/yyyy"))!="","fecha futura");
  u=Valid(); Check(Validate(u)==""&&u.Birthday==null,"fecha opcional");
  Check(Hash.GeneratePasswordHash("abc")=="A9993E364706816ABA3E25717850C26C9CD0D89D","SHA1 compatible con la guía");
  string one=Hash.HashPassword("prueba"),two=Hash.HashPassword("prueba");
  Check(Convert.FromBase64String(one).Length==36&&one!=two,"PBKDF2 con sal aleatoria");
  Check(typeof(EntityLayer.PoliMusicV3.User).Assembly.GetName().Name=="EntityLayer.PoliMusicV3","entidad en DLL independiente");
  Check(typeof(BLL.PoliMusicV3.User).Assembly.GetName().Name=="BLL.PoliMusicV3","negocio en DLL independiente");
  var web=Assembly.Load("WebAppPoliMusicV3"); bool hasBll=false,hasDal=false;
  foreach(var r in web.GetReferencedAssemblies()) { if(r.Name=="BLL.PoliMusicV3")hasBll=true; if(r.Name=="DAL.PoliMusicV3")hasDal=true; }
  Check(hasBll&&!hasDal,"Web referencia BLL y no DAL");
  var entity=typeof(EntityLayer.PoliMusicV3.User).Assembly; bool invalid=false;
  foreach(var r in entity.GetReferencedAssemblies()) if(r.Name.StartsWith("BLL.")||r.Name.StartsWith("DAL.")||r.Name=="System.Web")invalid=true;
  Check(!invalid,"entidades sin dependencia de infraestructura ni presentación");
  Console.WriteLine("TOTAL: "+count+" pruebas aprobadas.");
 }
}
