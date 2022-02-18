using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LsAdmin.Utility.Security {
    public class Md5Helper {
        /// <summary>  
        /// MD5 加密字符串  
        /// </summary>  
        /// <param name="rawPass">源字符串</param>  
        /// <returns>加密后字符串</returns>  
        public static string MD5Encrypt(string rawPass) {
            // 创建MD5类的默认实例：MD5CryptoServiceProvider  
            MD5 md5 = MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(rawPass);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs) {
                // 以十六进制格式格式化  
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>  
        /// MD5盐值加密  
        /// </summary>  
        /// <param name="rawPass">源字符串</param>  
        /// <param name="salt">盐值</param>  
        /// <returns>加密后字符串</returns>  
        public static string MD5Encrypt(string rawPass, object salt) {
            if (salt == null) return rawPass;
            return MD5Encrypt(rawPass + salt.ToString());
        }

        /// <summary>
        /// Md5加密文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns>加密后字符串</returns>
        public static string MD5Encrypt(byte[] file) {

            try
            {
                MD5 md5 = MD5.Create();
                byte[] hs = md5.ComputeHash(file);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hs)
                {
                    // 以十六进制格式格式化  
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MD5Encrypt() fail, error:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取文件MD5
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件MD5</returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail, error:" + ex.Message);
            }
        }


        /// <summary>
        /// MD5盐值加密文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="salt">盐值</param>
        /// <returns>加密后字符串</returns>
        public static string MD5Encrypt(byte[] file, object salt) {
            if (salt == null) return null;
            return MD5Encrypt(MD5Encrypt(file) + salt.ToString());
        }
    }

}