using System;
using System.Security.Cryptography;
using System.Text;
namespace EntityLayer.PoliMusicV3.Util
{
    public static class Hash
    {
        // Compatibilidad con los datos SHA1 de la guia academica. No usar para sistemas nuevos de produccion.
        public static string GeneratePasswordHash(string input)
        {
            using (SHA1Managed sha1=new SHA1Managed())
            {
                byte[] hash=sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb=new StringBuilder(hash.Length*2);
                foreach(byte b in hash) sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
        }
        public static string HashPassword(string password)
        {
            byte[] salt=new byte[16];
            RNGCryptoServiceProvider rng=new RNGCryptoServiceProvider();
            rng.GetBytes(salt);
            Rfc2898DeriveBytes pbkdf2=new Rfc2898DeriveBytes(password,salt,100000);
            byte[] hash=pbkdf2.GetBytes(20), result=new byte[36];
            Array.Copy(salt,0,result,0,16); Array.Copy(hash,0,result,16,20);
            return Convert.ToBase64String(result);
        }
    }
}
