using LsAdmin.Domain.IRepositories;
using LsAdmin.Domain.Entities;
using LsAdmin.Application.Imp;
using LsAdmin.Application.ProgramApp.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using AutoMapper;
using LsAdmin.Application.MaterialApp;
using System.Linq;
using System.Threading.Tasks;
using LsAdmin.Application.FilesApp;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using LsAdmin.Application.FilesApp.Dtos;
using LsAdmin.Utility.Security;
using System.Text;
using System.Runtime.InteropServices;

namespace LsAdmin.Application.ProgramApp {
    public class ProgramAppService : BaseAppService<Program, ProgramDto>, IProgramAppService {

        private readonly IProgramRepository _repository;
        private readonly IMaterialAppService _materialService;
        private readonly IFilesAppService _filesService;
        public ProgramAppService(IMaterialAppService materialService, IProgramRepository repository, IHttpContextAccessor httpContextAccessor, IFilesAppService filesService) : base(repository, httpContextAccessor) {
            _repository = repository;
            _materialService = materialService;
            _filesService = filesService;
        }

        public bool AddMaterial(Guid id, Guid materialId) {
            var item = Get(id);
            if (item == null) {
                return false;
            }
            var resource = _materialService.Get(materialId);
            if (resource == null) {
                return false;
            }
            var resources = item.ProgramMaterials.Select(it => it.Material).ToList();
            resources.Add(resource);
            return _repository.UpdateMaterials(id, Mapper.Map<List<Material>>(resources));
        }

        public List<ProgramDto> GetByOwnerUser(Guid userId, int startPage, int pageSize, out int rowCount) {
            return Mapper.Map<List<ProgramDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.OwnerUserId == userId, _orderbyDesc));
        }

        public bool PackageMaterialsZipById(Guid id)
        {
            var item = _repository.GetWithMaterials(id);

            if (item == null)  return false;

            List<FilesInfoDto> files = new List<FilesInfoDto>();
            #region  将素材文件装载到压缩包含文件里
            foreach (var material in item.ProgramMaterials)
            {
                var materialIfo = _materialService.GetInfo(material.MaterialId);
                if (materialIfo.File == null) continue;

                files.Add(new FilesInfoDto {
                    MD5 = string.IsNullOrEmpty(materialIfo.MD5) ?  Md5Helper.MD5Encrypt(materialIfo.File) : materialIfo.MD5,
                    File= materialIfo.File,
                    Name= materialIfo.Name,
                    FilenameExtension = materialIfo.FilenameExtension,
                    Id = materialIfo.Id
                });
            }
            #endregion

            #region  将网页装载到压缩包含文件里
            if (item.Type== ProgramType.Web)
            {
                FilesInfoDto htmlfile = new FilesInfoDto{
                    Id = new Guid(),
                    FilenameExtension=".html",
                    Name=id.ToString()+".html"
                };
   
                try
                {
                    string tempFileName = Path.GetTempFileName();
                    string ls_fileNeme = Path.ChangeExtension(tempFileName, htmlfile.FilenameExtension);
                    File.Move(tempFileName, ls_fileNeme);

                    using (FileStream htmlfs = new FileStream(ls_fileNeme, FileMode.Open))
                    {
                        StreamWriter sw = new StreamWriter(htmlfs, Encoding.Unicode);
                        sw.WriteLine(item.Content);
                        sw.Flush();
                        sw.Close();
                    }

                    using (FileStream fs = new FileStream(ls_fileNeme, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffur = new byte[fs.Length];
                        fs.Read(buffur, 0, (int)fs.Length);
                        htmlfile.File = buffur;
                        fs.Close();
                    }

                    htmlfile.MD5 = Md5Helper.MD5Encrypt(htmlfile.File);

                    files.Add(htmlfile);
                    File.Delete(ls_fileNeme);
                }
                catch(Exception ex)
                {
                    return false;
                }
            }

            #endregion

            if (files==null || files.Count==0) return false;

            string FilesMD5 ="";// 节目中各文件的md5 拼接字符串用作压缩文件的MD5

            #region 打包压缩节目所有文件 800M
            byte[] buffer = new byte[819200];
            MemoryStream returnStream = new MemoryStream();
            var zipMs = new MemoryStream();
            using (ZipOutputStream zipStream = new ZipOutputStream(zipMs))
            {
                zipStream.SetLevel(9);
                foreach(var file in files.OrderBy(o => o.Name))
                {
                    if (file.File == null) continue;
                    
                    FilesMD5  += file.MD5;

                    string fileName = file.Name;
                    using (var streamInput = new MemoryStream(file.File) )
                    {
                        ZipEntry entry = new ZipEntry(fileName);
                        //设置wei unicode编码 解决原文件名称为中文但压缩后文件名为乱码问题
                        entry.IsUnicodeText = true;

                        zipStream.PutNextEntry(entry);
                        while (true)
                        {
                            var readCount = streamInput.Read(buffer, 0, buffer.Length);
                            if (readCount > 0)
                            {
                                zipStream.Write(buffer, 0, readCount);
                            }
                            else
                            {
                                break;
                            }
                        }
                        zipStream.Flush();
                    }
                }
                zipStream.Finish();
                zipMs.Position = 0;
                zipMs.CopyTo(returnStream, 819200);
            }
            returnStream.Position = 0;

            byte[] zipBytes = new byte[returnStream.Length];
            returnStream.Read(zipBytes, 0, zipBytes.Length);
            // 设置当前流的位置为流的开始
            returnStream.Seek(0, SeekOrigin.Begin);
            #endregion

            #region 保存压缩文件
            string zipFileMD5        = Md5Helper.MD5Encrypt(zipBytes);
            string zipFileMD5MD5 = Md5Helper.MD5Encrypt(FilesMD5);

            FilesDto zipFileSalt = new FilesDto {
                MD5 = zipFileMD5MD5,
                OwnerObjId = id,
                Name = FilesMD5,
                FilenameExtension="MD5s2MD5",
                //CreateUserId=new Guid("1c16d695-0065-4820-9a5c-821ce8d7d2d8"),
            };

            if (_filesService.Insert(ref zipFileSalt)==false) return false ;
            var fileInfo = new FilesInfoDto {
                File = zipBytes,
                FilenameExtension = "zip",
                MD5 = Md5Helper.MD5Encrypt(zipBytes, id),
                OwnerObjId = zipFileSalt.Id,
                Name = zipFileSalt.Id.ToString() + ".zip",
                // CreateUserId = new Guid("1c16d695-0065-4820-9a5c-821ce8d7d2d8"),
            };
            if (
            _filesService.InsertInfo(ref fileInfo) == false)
            {
                _filesService.Delete(zipFileSalt.Id);
                return false; } 

            #endregion

            return true; 
        }

        public bool RemoveMaterial(Guid id, Guid materialId) {
            var item = Get(id);
            if (item == null) {
                return false;
            }
            var resource = _materialService.Get(materialId);
            if (resource == null) {
                return false;
            }
            item.ProgramMaterials.RemoveAll(it => it.ProgramId == item.Id && it.MaterialId == resource.Id);
            return Update(item);
        }

       public FilesInfoDto GetResourcesById(Guid id)
       {
            try { 
                var file = _filesService.GetOwnerOneObj(id);
                //找不到对应文件记录返回null
                if (file == null) return null;
                var zipfile = _filesService.GetOwnerOneObj(file.Id);
                //找不到对应zip文件返回null
                if (zipfile == null) return null;

                //var asdf = _filesService.GetInfo(zipfile.Id);

                FilesInfoDto zipfileinfo = new FilesInfoDto {
                    Id= zipfile.Id,
                    FilenameExtension=zipfile.FilenameExtension,
                    MD5= zipfile.MD5,
                    Name= zipfile.Name,
                    OwnerObjId= zipfile.OwnerObjId,
                    Thumbnail= zipfile.Thumbnail,
                    Remarks= zipfile.Remarks,
                    File=Utility.FTP.FtpHelper.DownloadFtpFile(zipfile.Id.ToString() + "." + zipfile.FilenameExtension),
                };
        
                return zipfileinfo;
                //return ((FilesInfoDto)zipfile);
            }catch(Exception ex)
            {
                return null;
            } 
       }

        public void PackageMaterialsZipFromNoPackage()
        {
           var zipFileIds = _filesService.GetAllList().Where(w => w.FilenameExtension == "MD5s2MD5").Select(s => s.OwnerObjId).ToList();

           var ids = _repository.GetEntities().Where(w => !zipFileIds.Contains(w.Id)).Select(s => s.Id).ToList();

           foreach(var id in ids)
            {
                PackageMaterialsZipById(id);
            }
        }

        public string GetLauncher(Guid id)
        {
            try { 
            var item = _repository.GetWithMaterials(id);

            if (item == null ) return "";

            //网页类型
            if (item.Type == ProgramType.Web)
                return item.Id.ToString() + ".html";

            //视频类型
            if(item.ProgramMaterials == null) return "";
  
             return  _materialService.Get(item.ProgramMaterials.First().MaterialId)?.Name;
            }catch(Exception ex)
            {
                return "";
            }
        }
    }
}
