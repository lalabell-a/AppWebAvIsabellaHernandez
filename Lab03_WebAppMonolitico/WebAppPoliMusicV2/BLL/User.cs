using System.Data;
namespace WebAppPoliMusicV2.BLL
{
    public class User
    {
        private readonly DAL.User data;
        public User(string connectionString) { data=new DAL.User(connectionString); }
        public int Save(Entities.User user) { return data.Save(user); }
        public DataTable Read() { return data.Read(); }
        public int Update(Entities.User user) { return data.Update(user); }
        public int Delete(int id) { return data.Delete(id); }
        public int CheckExistUser(string value) { return data.CheckExistUser(value); }
        public int CheckExistEmail(string value) { return data.CheckExistEmail(value); }
        public DataTable AuthenticateUser(string login,string password) { return data.AuthenticateUser(login,password); }
        public string ValidationsDuplicated(Entities.User user)
        {
            if(CheckExistUser(user.UserName)!=0) return "El nombre de usuario ya está registrado.";
            if(CheckExistEmail(user.Email)!=0) return "El correo electrónico ya está registrado.";
            return string.Empty;
        }
    }
}
