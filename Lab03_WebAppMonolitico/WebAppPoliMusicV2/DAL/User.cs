using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
namespace WebAppPoliMusicV2.DAL
{
    public class User
    {
        private readonly string connectionString;
        public User(string strConnString) { connectionString=strConnString; }
        private SqlCommand Command(string sql) { return new SqlCommand(sql,new SqlConnection(connectionString)); }
        private static DataTable Table(SqlCommand cmd)
        {
            using(cmd) using(cmd.Connection) using(SqlDataAdapter adapter=new SqlDataAdapter(cmd))
            { DataTable dt=new DataTable(); adapter.Fill(dt); return dt; }
        }
        private static int Execute(SqlCommand cmd)
        { using(cmd) using(cmd.Connection) { cmd.Connection.Open(); return cmd.ExecuteNonQuery(); } }
        private static void Fields(SqlCommand cmd,Entities.User user)
        {
            cmd.Parameters.Add("@USERNAME",SqlDbType.VarChar,50).Value=user.UserName;
            cmd.Parameters.Add("@EMAIL",SqlDbType.VarChar,50).Value=user.Email;
            cmd.Parameters.Add("@USER_TYPE",SqlDbType.Int).Value=user.UserType;
            cmd.Parameters.Add("@BIRTHDAY",SqlDbType.DateTime).Value=String.IsNullOrEmpty(user.Birthday)?(object)DBNull.Value:DateTime.ParseExact(user.Birthday,"yyyy-MM-dd",CultureInfo.InvariantCulture);
            cmd.Parameters.Add("@PHOTO",SqlDbType.VarChar,255).Value=String.IsNullOrEmpty(user.Photo)?(object)DBNull.Value:user.Photo;
        }
        public int Save(Entities.User user)
        {
            SqlCommand cmd=Command("INSERT INTO dbo.TBL_USER(USERNAME,PASSWORD,EMAIL,BIRTHDAY,USER_PHOTO,USER_TYPE) VALUES(@USERNAME,@PASSWORD,@EMAIL,@BIRTHDAY,@PHOTO,@USER_TYPE)");
            Fields(cmd,user); cmd.Parameters.Add("@PASSWORD",SqlDbType.VarChar,50).Value=user.Password;
            return Execute(cmd);
        }
        public DataTable Read() { return Table(Command("SELECT ID_USER,USERNAME,EMAIL,BIRTHDAY,USER_PHOTO,USER_TYPE FROM dbo.TBL_USER ORDER BY ID_USER")); }
        private int Check(string column,string value)
        {
            using(SqlCommand cmd=Command("SELECT TOP 1 ID_USER FROM dbo.TBL_USER WHERE "+column+"=@VALUE"))
            using(cmd.Connection)
            { cmd.Parameters.Add("@VALUE",SqlDbType.VarChar,50).Value=value; cmd.Connection.Open(); object id=cmd.ExecuteScalar(); return id==null?0:Convert.ToInt32(id); }
        }
        public int CheckExistUser(string value) { return Check("USERNAME",value); }
        public int CheckExistEmail(string value) { return Check("EMAIL",value); }
        public int Update(Entities.User user)
        {
            SqlCommand cmd=Command("UPDATE dbo.TBL_USER SET USERNAME=@USERNAME,EMAIL=@EMAIL,USER_TYPE=@USER_TYPE,BIRTHDAY=@BIRTHDAY,USER_PHOTO=@PHOTO"+(String.IsNullOrEmpty(user.Password)?"":",PASSWORD=@PASSWORD")+" WHERE ID_USER=@ID");
            Fields(cmd,user); cmd.Parameters.Add("@ID",SqlDbType.Int).Value=user.ID;
            if(!String.IsNullOrEmpty(user.Password)) cmd.Parameters.Add("@PASSWORD",SqlDbType.VarChar,50).Value=user.Password;
            return Execute(cmd);
        }
        public int Delete(int id)
        { SqlCommand cmd=Command("DELETE FROM dbo.TBL_USER WHERE ID_USER=@ID"); cmd.Parameters.Add("@ID",SqlDbType.Int).Value=id; return Execute(cmd); }
        public DataTable AuthenticateUser(string login,string password)
        {
            SqlCommand cmd=Command("SELECT ID_USER,USERNAME,EMAIL,BIRTHDAY,USER_PHOTO,USER_TYPE FROM dbo.TBL_USER WHERE USERNAME=@USERNAME AND PASSWORD=@PASSWORD");
            cmd.Parameters.Add("@USERNAME",SqlDbType.VarChar,50).Value=login;
            cmd.Parameters.Add("@PASSWORD",SqlDbType.VarChar,50).Value=password;
            return Table(cmd);
        }
    }
}
