using System.Web.Routing;
namespace WebAppPoliMusicV3.App_Start {
 public static class RouteConfig {
  public static void RegisterRoutes(RouteCollection routes) {
   routes.MapPageRoute("Inicio","Default","~/Default.aspx");
   routes.MapPageRoute("Canciones","Song","~/Song.aspx");
   routes.MapPageRoute("Ingreso","Login","~/Login.aspx");
   routes.MapPageRoute("Registro","SignUp","~/SignUp.aspx");
   routes.MapPageRoute("Salida","Logout","~/Logout.aspx");
  }
 }
}
