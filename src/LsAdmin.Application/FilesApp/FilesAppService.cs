using System;
using System.Collections.Generic;
using System.Linq;
using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using AutoMapper;
using LsAdmin.Application.Imp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using LsAdmin.Application.FilesApp.Dtos;

namespace LsAdmin.Application.FilesApp
{
    class FilesAppService : BaseAppService<Files, FilesDto>, IFilesAppService
    {
        private readonly IFilesRepository _repository;
        public FilesAppService(IFilesRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }


        public override void Delete(Guid id)
        {
            Utility.FTP.FtpHelper.DeleteFtpFile(id.ToString() + "." + Get(id).FilenameExtension);
            base.Delete(id);
        }
        public override void DeleteBatch(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                Utility.FTP.FtpHelper.DeleteFtpFile(id.ToString() + "." + Get(id).FilenameExtension);
            }
            base.DeleteBatch(ids);
        }

        public void DeletetOwnerObj(Guid Ownerid)
        {
            List<Guid> ids = GetAllList().Where(w => w.OwnerObjId == Ownerid)?.Select(s => s.Id)?.ToList();
            if (ids != null)
                DeleteBatch(ids);
        }

        public FilesInfoDto GetInfo(Guid id)
        {
            FilesDto files = Get(id);
            if (files == null)
            {
                return null;
            }
            return (FilesInfoDto)files;
        }

        public List<FilesDto> GetOwnerObjPageList(Guid OwnerObjId, int startPage, int pageSize, out int rowCount)
        {
            var result = from p in GetAllList().Where(w => w.OwnerObjId == OwnerObjId)
                         select p;
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<FilesDto> GetPageListByType(Guid OwnerObjId, int startPage, int pageSize, out int rowCount, ushort type)
        {

            var result = from p in GetAllList().Where(w => w.Type == type & w.OwnerObjId == OwnerObjId)
                         select p;
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<FilesDto> GetPageListByEquipment(Guid equipmentId,Guid placeId,int startPage,int pageSize,out int rowCount)
        {
            var result = GetAllList().Where(w => w.OwnerObjId == equipmentId);
            var placeresult= from p in GetAllList().Where(w => w.OwnerObjId == placeId)
                             select p;
            result = result.Union(placeresult);
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public bool InsertInfo(ref FilesInfoDto infodto)
        {
            FilesDto dto = infodto;
            if(Insert(ref dto)==false)
                return false;
            
            if(Utility.FTP.FtpHelper.UploadFtpFile(dto.Id.ToString() + "." + dto.FilenameExtension, infodto.File) == false)
            {
                Delete(dto.Id);
                return false;
            }
            infodto.Id = dto.Id;
            return true;      
        }

        public List<FilesDto> GetOwnerObj(Guid OwnerObjId)
        {
            var result = GetAllList().Where(w => w.OwnerObjId == OwnerObjId);
            if (result != null){
                return result.ToList();
            }
            else{
                return null;
            }         
        }

        public FilesDto GetOwnerOneObj(Guid OwnerObjId)
        {
            return GetOwnerObj(OwnerObjId).FirstOrDefault();
        }
    }
}
