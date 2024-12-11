using System.Text;
using System.Security.Cryptography;

namespace BTL_ASPNETCORE.Models
{
    public class Cipher
    {
        public static string GenerateMD5(string data)
        {
            //tạo mới đối tượng lưu chuỗi kết quả 
            StringBuilder hash = new StringBuilder();
            //tạo mới đối tượng mã hóa md5
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            //mã hóa
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(data));
            //duyệt từng byte chuyển sang hệ 16
            for (int i = 0; i < bytes.Length; i++)
            {
                //chuyển về hệ hexa (16) chữ thường nếu muốn chữ hoa thì X2 nhé
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
