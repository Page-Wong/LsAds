using LsAdmin.Application.FilesApp;
using LsAdmin.Application.FilesApp.Dtos;
using LsAdmin.Utility.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LsAdmin.Application.EquipmentModelApp.Dtos
{
    public class EquipmentModelInfoDto: EquipmentModelDto{
       public virtual byte[] Thumbnail{
            get{
                if (Inthumbnail == null){
                    try{
                        var service = (IFilesAppService)HttpHelper.ServiceProvider.GetService(typeof(IFilesAppService));
                        var file = service.GetOwnerObj(Id)?.FirstOrDefault();
                        if (file != null){
                            Inthumbnail = ((FilesInfoDto)file).File;
                        }else
                            Thumbnail = Utility.FTP.FtpHelper.DownloadFtpFile("zbddx.jpg");
                 

                       // Inthumbnail = null;
                       //Inthumbnail = Utility.FTP.FtpHelper.DownloadFtpFile(Id.ToString() + ".jpg" );
                    }
                    catch (Exception){
                        return null;
                    }
                }
                return Inthumbnail;
            }
            set{
                Inthumbnail = value;
            }
        }

        private byte[] Inthumbnail;

    }
}
