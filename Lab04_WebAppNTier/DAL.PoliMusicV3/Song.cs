using System.Data;
using System.Data.SqlClient;
namespace DAL.PoliMusicV3
{
    public class Song
    {
        private readonly string connectionString;
        public Song(string strConnString) { connectionString=strConnString; }
        public DataTable Read() { return Read(string.Empty); }
        public DataTable Read(string search)
        {
            using(SqlConnection con=new SqlConnection(connectionString))
            using(SqlCommand cmd=new SqlCommand("SELECT ID_SONG,SONG_NAME,SONG_PATH,ISNULL(PLAYS,0) AS PLAYS FROM dbo.TBL_SONG WHERE @SEARCH='' OR SONG_NAME LIKE @PATTERN ESCAPE '~' ORDER BY SONG_NAME,ID_SONG",con))
            {
                cmd.Parameters.Add("@SEARCH",SqlDbType.NVarChar,100).Value=search;
                cmd.Parameters.Add("@PATTERN",SqlDbType.NVarChar,210).Value="%"+search.Replace("~","~~").Replace("%","~%").Replace("_","~_").Replace("[","~[")+"%";
                using(SqlDataAdapter adapter=new SqlDataAdapter(cmd)) { DataTable table=new DataTable(); adapter.Fill(table); return table; }
            }
        }
        public string ReadPath(int id)
        {
            using(SqlConnection con=new SqlConnection(connectionString))
            using(SqlCommand cmd=new SqlCommand("SELECT SONG_PATH FROM dbo.TBL_SONG WHERE ID_SONG=@ID",con))
            { cmd.Parameters.Add("@ID",SqlDbType.Int).Value=id; con.Open(); object path=cmd.ExecuteScalar(); return path==null||path==System.DBNull.Value?null:path.ToString(); }
        }
    }
}
