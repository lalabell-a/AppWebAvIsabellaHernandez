using System;
using System.Web;
namespace WebAppPoliMusicV3 { public class Global : HttpApplication { protected void Application_Start(object sender,EventArgs e) { App_Start.RouteConfig.RegisterRoutes(System.Web.Routing.RouteTable.Routes); } } }
