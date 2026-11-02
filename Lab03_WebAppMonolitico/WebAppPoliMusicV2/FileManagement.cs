using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace WebAppPoliMusicV2
{
    public class FileManagement : Page
    {
        private string Folder(string path)
        { return System.Web.HttpContext.Current.Server.MapPath(path); }
        private static string SafeName(string name)
        { return Path.GetFileName(name); }
        public string SaveImageOnServer(FileUpload upload,string folder,string newFileName)
        {
            if(!upload.HasFile) return "Selecciona una imagen.";
            string ext=Path.GetExtension(upload.FileName).ToLowerInvariant();
            if((ext!=".jpg"&&ext!=".jpeg"&&ext!=".png") || (upload.PostedFile.ContentType!="image/jpeg"&&upload.PostedFile.ContentType!="image/png"&&upload.PostedFile.ContentType!="image/jpg")) return "La foto debe ser JPG o PNG.";
            if(upload.PostedFile.ContentLength>10485760) return "La foto no puede superar 10 MB.";
            try
            {
                using(System.Drawing.Image image=System.Drawing.Image.FromStream(upload.PostedFile.InputStream))
                { if(image.Width<1||image.Height<1) return "La imagen no es válida."; }
                upload.PostedFile.InputStream.Position=0;
                Directory.CreateDirectory(Folder(folder));
                string name=String.IsNullOrEmpty(newFileName)?Path.GetFileNameWithoutExtension(upload.FileName):SafeName(newFileName);
                upload.SaveAs(Path.Combine(Folder(folder),name+ext)); return string.Empty;
            }
            catch(ArgumentException) { return "El archivo no contiene una imagen válida."; }
            catch(IOException) { return "No se pudo guardar la foto. Revisa los permisos de la carpeta."; }
        }
        public string SaveDefaultImageOnServer(string folder,string newFileName)
        {
            try { File.Copy(Path.Combine(Folder(folder),"default.jpg"),Path.Combine(Folder(folder),SafeName(newFileName)+".jpg"),true); return string.Empty; }
            catch(IOException) { return "No se pudo guardar la imagen predeterminada."; }
        }
        public string SaveSongOnServer(FileUpload upload,string folder,string newFileName)
        {
            if(!upload.HasFile) return "Selecciona una canción.";
            if(Path.GetExtension(upload.FileName).ToLowerInvariant()!=".mp3") return "Solo se permiten archivos MP3.";
            int max=Convert.ToInt32(ConfigurationManager.AppSettings["maxSongLength"]);
            if(upload.PostedFile.ContentLength>max) return "La canción supera el tamaño permitido.";
            try { Directory.CreateDirectory(Folder(folder)); upload.SaveAs(Path.Combine(Folder(folder),SafeName(newFileName)+".mp3")); return string.Empty; }
            catch(IOException) { return "No se pudo guardar la canción."; }
        }
    }
}
