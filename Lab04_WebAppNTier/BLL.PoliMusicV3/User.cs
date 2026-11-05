using System.Data;
namespace BLL.PoliMusicV3
{
    public class User
    {
        private readonly DAL.PoliMusicV3.User data;
        public User(string connectionString) { data=new DAL.PoliMusicV3.User(connectionString); }
        public int Save(EntityLayer.PoliMusicV3.User user) { return data.Save(user); }
        public DataTable Read() { return data.Read(); }
        public int Update(EntityLayer.PoliMusicV3.User user) { return data.Update(user); }
        public int Delete(int id) { return data.Delete(id); }
        public int CheckExistUser(string value) { return data.CheckExistUser(value); }
        public int CheckExistEmail(string value) { return data.CheckExistEmail(value); }
        public DataTable AuthenticateUser(string login,string password) { return data.AuthenticateUser(login,password); }
        public string ValidationsDuplicated(EntityLayer.PoliMusicV3.User user)
        {
            if(CheckExistUser(user.UserName)!=0) return "El nombre de usuario ya está registrado.";
            if(CheckExistEmail(user.Email)!=0) return "El correo electrónico ya está registrado.";
            return string.Empty;
        }
    }
}
