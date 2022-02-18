using System;
using System.Linq;

namespace LsAdmin.Application.MaterialApp.Dtos {
    public class MaterialInfoDto :MaterialDto{
        public virtual byte[] File
        {
            get
            {
                try { return Utility.FTP.FtpHelper.DownloadFtpFile(Id.ToString() + "." + FilenameExtension); }
                catch (Exception ex)
                { return Thumbnail; }
            }
        }

    }
}
