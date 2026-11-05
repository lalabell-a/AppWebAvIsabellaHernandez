using System;
using System.Configuration;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.SessionState;
namespace WebAppPoliMusicV3
{
    // Sirve MP3 locales, con sesion y soporte Range para avanzar y retroceder.
    public class Media : IHttpHandler,IReadOnlySessionState
    {
        public bool IsReusable { get { return false; } }
        public void ProcessRequest(HttpContext context)
        {
            if(!context.Request.IsAuthenticated||context.Session[EntityLayer.PoliMusicV3.Util.Constants.USER]==null) { context.Response.StatusCode=403; return; }
            if(context.Request.HttpMethod!="GET"&&context.Request.HttpMethod!="HEAD") { context.Response.StatusCode=405; return; }
            int id;
            if(!Int32.TryParse(context.Request.QueryString["id"],out id)||id<1) { context.Response.StatusCode=400; return; }
            try
            {
                string stored=new BLL.PoliMusicV3.Song(ConfigurationManager.ConnectionStrings["BDD_PoliMusicConnectionString"].ConnectionString).ReadPath(id);
                if(String.IsNullOrEmpty(stored)) { context.Response.StatusCode=404; return; }
                string root=context.Server.MapPath(ConfigurationManager.AppSettings["songFolderPath"]);
                string relative=stored.Replace('\\','/');
                string[] prefixes=new string[]{"~/SongFiles/","../SongFiles/","SongFiles/","/SongFiles/"};
                foreach(string prefix in prefixes) if(relative.StartsWith(prefix,StringComparison.OrdinalIgnoreCase)) { relative=relative.Substring(prefix.Length); break; }
                string path=Path.GetFullPath(Path.Combine(root,relative));
                if(!path.StartsWith(Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)||Path.GetExtension(path).ToLowerInvariant()!=".mp3"||!File.Exists(path)) { context.Response.StatusCode=404; return; }
                long length=new FileInfo(path).Length,start=0,end=length-1;
                string range=context.Request.Headers["Range"];
                if(!String.IsNullOrEmpty(range)&&range.StartsWith("bytes=",StringComparison.Ordinal))
                {
                    string[] pieces=range.Substring(6).Split('-'); long parsed;
                    bool valid=pieces.Length==2&&range.IndexOf(',')<0;
                    if(valid&&pieces[0].Length==0) { valid=Int64.TryParse(pieces[1],out parsed)&&parsed>0; if(valid) start=Math.Max(0,length-parsed); }
                    else if(valid) { valid=Int64.TryParse(pieces[0],out start); if(valid&&pieces[1].Length>0) { valid=Int64.TryParse(pieces[1],out end); end=Math.Min(end,length-1); } }
                    if(!valid||start<0||start>=length||end<start) { context.Response.StatusCode=416; context.Response.AddHeader("Content-Range","bytes */"+length); return; }
                    context.Response.StatusCode=206;
                    context.Response.AddHeader("Content-Range",String.Format(CultureInfo.InvariantCulture,"bytes {0}-{1}/{2}",start,end,length));
                }
                context.Response.ContentType="audio/mpeg"; context.Response.BufferOutput=false;
                context.Response.AddHeader("Accept-Ranges","bytes");
                context.Response.AddHeader("Content-Length",(end-start+1).ToString(CultureInfo.InvariantCulture));
                context.Response.Cache.SetCacheability(HttpCacheability.Private);
                if(context.Request.HttpMethod=="HEAD") return;
                using(FileStream stream=File.OpenRead(path))
                {
                    stream.Seek(start,SeekOrigin.Begin); byte[] buffer=new byte[65536]; long remaining=end-start+1;
                    while(remaining>0&&context.Response.IsClientConnected)
                    { int read=stream.Read(buffer,0,(int)Math.Min(buffer.Length,remaining)); if(read==0) break; context.Response.OutputStream.Write(buffer,0,read); remaining-=read; }
                }
            }
            catch(DbException) { context.Response.StatusCode=503; }
            catch(IOException) { context.Response.StatusCode=404; }
            catch(ArgumentException) { context.Response.StatusCode=404; }
        }
    }
}
