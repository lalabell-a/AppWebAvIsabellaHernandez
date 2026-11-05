using System.Data;
namespace BLL.PoliMusicV3
{
    public class Song
    {
        private readonly DAL.PoliMusicV3.Song data;
        public Song(string connectionString) { data=new DAL.PoliMusicV3.Song(connectionString); }
        public DataTable Read() { return data.Read(); }
        public DataTable Read(string search) { return data.Read(search==null?string.Empty:search.Trim()); }
        public string ReadPath(int id) { return id>0?data.ReadPath(id):null; }
    }
}
