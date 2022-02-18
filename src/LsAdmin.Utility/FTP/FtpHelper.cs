using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Globalization;

namespace LsAdmin.Utility.FTP
{
    public static class FtpHelper
    {

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="File">byte文件内容</param>
        /// <returns></returns>
        public static Boolean UploadFtpFile(string filename, byte[] File)
        {
            return UploadeFtpFile("172.16.39.1", "", "FtpAds", "lsftp+9893-", filename, File);
        }

        /// <summary>
        /// 上传(可自定义指定ftp服务器)
        /// </summary>
        /// <param name="FtpServerIP">ftp服务器ip</param>
        /// <param name="FtpRemotePath">文件上传后存放路径</param>
        /// <param name="FtpUserID">用户</param>
        /// <param name="FtpPassword">密码</param>
        /// <param name="filename">文件名</param>
        /// <param name="File">文件内容</param>
        /// <returns></returns>
        public static Boolean UploadeFtpFile(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword,string filename, byte[] File)
        {
            string ftpURI = "ftp://" + FtpServerIP + "/" + FtpRemotePath + "/";
            string uri = ftpURI + filename;
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            reqFTP.Credentials = new NetworkCredential(FtpUserID, FtpPassword);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = File.Length;

            try
            {
                Stream strm = reqFTP.GetRequestStream();
                strm.Write(File, 0, File.Length);
                strm.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static byte[] DownloadFtpFile( string filename)
        {
            return DownloadFtpFile("172.16.39.1", "", "FtpAds", "lsftp+9893-", filename);
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="FtpServerIP"></param>
        /// <param name="FtpRemotePath"></param>
        /// <param name="FtpUserID"></param>
        /// <param name="FtpPassword"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static byte[] DownloadFtpFile(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword, string filename)
        {     
            try
            {
                string ftpURI = "ftp://" + FtpServerIP + "/" + FtpRemotePath + "/";
                string uri = ftpURI + filename;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(FtpUserID, FtpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                MemoryStream mem = new MemoryStream(1024*1024*200);//200mb
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                int TotalByteRead = 0;
                while (true)
                {
                    bytesRead = ftpStream.Read(buffer, 0, buffer.Length);
                    TotalByteRead += bytesRead;
                    if (bytesRead == 0)
                        break;
                    mem.Write(buffer, 0, bytesRead);
                }
                if (mem.Length > 0)
                {
                    return mem.ToArray();
                }
                else
                {
                    return null;
                }
    
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Boolean DeleteFtpFile(string filename)
        {
            return DeleteFtpFile("172.16.39.1", "", "FtpAds", "lsftp+9893-", filename);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FtpServerIP"></param>
        /// <param name="FtpRemotePath"></param>
        /// <param name="FtpUserID"></param>
        /// <param name="FtpPassword"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Boolean DeleteFtpFile(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword, string filename)
        {
            try
            {
                string ftpURI = "ftp://" + FtpServerIP + "/" + FtpRemotePath + "/";
                string uri = ftpURI + filename;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(FtpUserID, FtpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }




    }
}
