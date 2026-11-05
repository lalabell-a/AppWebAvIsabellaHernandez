using System;
namespace EntityLayer.PoliMusicV3
{
    [Serializable]
    public class User
    {
        private int id, userType;
        private string userName, password, email, birthday, photo;
        public User() { }
        public User(int id, string username, string password, string email, string birthday, int userType, string photo)
        { ID=id; UserName=username; Password=password; Email=email; Birthday=birthday; UserType=userType; Photo=photo; }
        public int ID { get { return id; } set { id=value; } }
        public int UserType { get { return userType; } set { userType=value; } }
        public string UserName { get { return userName; } set { userName=value; } }
        public string Password { get { return password; } set { password=value; } }
        public string Email { get { return email; } set { email=value; } }
        public string Birthday { get { return birthday; } set { birthday=value; } }
        public string Photo { get { return photo; } set { photo=value; } }
    }
}
