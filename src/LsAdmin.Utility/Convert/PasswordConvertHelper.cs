using System.Security.Cryptography;
using System.Text;

namespace LsAdmin.Utility.Convert {
    public class PasswordConvertHelper {
        
        public static string Create(string password)
        {
            return System.Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
        
    }
}
