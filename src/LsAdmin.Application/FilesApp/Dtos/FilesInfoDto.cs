using System;
using System.Linq;

namespace LsAdmin.Application.FilesApp.Dtos {
    public class FilesInfoDto : FilesDto
    {
         
        public virtual byte[] File
        {
            get
            {
                if (Infile == null) {
                    try {
                        Infile = Utility.FTP.FtpHelper.DownloadFtpFile(Id.ToString() + "." + FilenameExtension);
                    }
                    catch (Exception) {
                        return Thumbnail;
                    }
                }

                return Infile;
            }
            set {
                Infile = value;
            }
        }

        private byte[] Infile;

    }
}
