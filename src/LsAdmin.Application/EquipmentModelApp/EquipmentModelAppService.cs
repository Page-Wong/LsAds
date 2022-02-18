using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;
using LsAdmin.Application.EquipmentModelApp.Dtos;
using System;
using System.Collections.Generic;
using LsAdmin.Application.FilesApp;
using LsAdmin.Application.FilesApp.Dtos;

namespace LsAdmin.Application.EquipmentModelApp
{
    class EquipmentModelAppService : BaseAppService<EquipmentModel, EquipmentModelDto>, IEquipmentModelAppService
    {
        private readonly IEquipmentModelRepository _repository;
        private readonly IFilesAppService _serviceFiles;
        
        public EquipmentModelAppService(IEquipmentModelRepository repository, IFilesAppService serviceFiles, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
            _serviceFiles = serviceFiles;
        }

        public void DeleteInfo(Guid id)
        {

            // Utility.FTP.FtpHelper.DeleteFtpFile(id.ToString() + ".jpg");
            _serviceFiles.DeletetOwnerObj(id);
            Delete(id);
        }

        public void DeleteBatcheInfo(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                _serviceFiles.DeletetOwnerObj(id);
               // Utility.FTP.FtpHelper.DeleteFtpFile(id.ToString() + ".jpg");
            }
            base.DeleteBatch(ids);
        }

        public EquipmentModelInfoDto GetInfo(Guid id)
        {
            EquipmentModelDto files = Get(id);
            if (files == null)
            {
                return null;
            }
            return (EquipmentModelInfoDto)files;
        }



        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public bool InsertInfo(EquipmentModelInfoDto infodto)
        {
            EquipmentModelDto dto = infodto;
            if (Insert(ref dto) == false)
                return false;

            if (infodto.Thumbnail != null) {
                FilesInfoDto file = new FilesInfoDto {
                    File = infodto.Thumbnail,
                    Name = infodto.Model+"_图片",
                    FilenameExtension = "jpg",
                    OwnerObjId = dto.Id
                };
          
                if (_serviceFiles.InsertInfo(ref file)==false)
                {
                    Delete(dto.Id);
                    return false;
                }

                /*
                if (Utility.FTP.FtpHelper.UploadFtpFile(dto.Id.ToString() + ".jpg" , infodto.Thumbnail) == false){
                    Delete(dto.Id);
                    return false;
                }*/
            }
            return true;
        }

        public bool UpdateInfo(EquipmentModelInfoDto infodto)
        {
            try {    
                if (infodto.Thumbnail != null)
                {
                    _serviceFiles.DeletetOwnerObj(infodto.Id);

                    FilesInfoDto file = new FilesInfoDto{
                        File = infodto.Thumbnail,
                        Name = infodto.Model + "_图片",
                        FilenameExtension = "jpg",
                        OwnerObjId = infodto.Id
                    };

                    if (_serviceFiles.InsertInfo(ref file) == false){       
                        return false;
                    }

                    /* Utility.FTP.FtpHelper.DeleteFtpFile(infodto.Id.ToString() + ".jpg");
                    if (Utility.FTP.FtpHelper.UploadFtpFile(infodto.Id.ToString() + ".jpg", infodto.Thumbnail) == false)
                    {
                        return false;
                    }*/
                }
                return base.Update(infodto);
                    
            }catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 检测是否存在同型号记录，true 存在 false 不存在
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manufacturer"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistSameModel(string model, string manufacturer, Guid id)
        {
            try { 
            if( _repository.FirstOrDefault(f => f.Model == model && f.Manufacturer == manufacturer && f.Id != id) != null){
                    return true;
                }
            else{
                    return false;
                }
            }catch(Exception ex) {
                return true;
            }
        }
    }
}
