using LsAdmin.Application.CollectionsBlacklistsApp.Dtos;
using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;
using System.Text;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using System.Linq;

namespace LsAdmin.Application.CollectionsBlacklistsApp
{
    public class CollectionsBlacklistsAppService: BaseAppService<CollectionsBlacklists, CollectionsBlacklistsDto>, ICollectionsBlacklistsAppService
    {
        private readonly ICollectionsBlacklistsRepository _repository;

        public CollectionsBlacklistsAppService(ICollectionsBlacklistsRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor) {
            _repository = repository;
        }

        ///<summary>
        ///获取当前用户的黑名单
        ///</summary>
        ///<returns></returns>
        public List<CollectionsBlacklistsDto> GetBlacklists()
        {
            return Mapper.Map<List<CollectionsBlacklistsDto>>(_repository.GetEntities().Where(o => o.UserId == CurrentUser.Id && o.FavoriteType==2)).ToList();
        }

        ///<summary>
        ///获取当前用户的黑名单列表
        ///</summary>
        ///<returns></returns>
        public List<CollectionsBlacklistsDto> GetBlacklistsPageList(int startPage,int pageSize,out int rowCount)
        {
            return Mapper.Map<List<CollectionsBlacklistsDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, o => o.UserId == CurrentUser.Id && o.FavoriteType == 2));
        }

        ///<summary>
        ///获取当前用户的收藏
        ///</summary>
        ///<returns></returns>
        public List<CollectionsBlacklistsDto> GetCollections()
        {
            return Mapper.Map<List<CollectionsBlacklistsDto>>(_repository.GetEntities().Where(o => o.UserId == CurrentUser.Id && o.FavoriteType == 1)).ToList();
        }

        ///<summary>
        ///获取当前用户的收藏列表
        ///</summary>
        ///<returns></returns>
        public List<CollectionsBlacklistsDto> GetCollectionsPageList(int startPage,int pageSize,out int rowCount)
        {
            return Mapper.Map<List<CollectionsBlacklistsDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, o => o.UserId == CurrentUser.Id && o.FavoriteType == 1));
        }

        ///<summary>
        ///返回该用户该设备是否是收藏或者黑名单
        ///</summary>
        ///<returns></returns>
        public ushort FavoriteType(Guid equipmentId)
        {
            var result = _repository.GetEntities().Where(o => o.UserId == CurrentUser.Id && o.EquipmentId == equipmentId);
            if (result != null)
            {
                return result.Select(s => s.FavoriteType).FirstOrDefault();
            }
            else
            {
                return 0;
            }
        }

        ///<summary>
        ///获取收藏/黑名单状态
        ///</summary>
        ///<returns></returns>
        public CollectionsBlacklistsDto GetFavoriteInfo(Guid equipmentId)
        {
            var result = Mapper.Map<List<CollectionsBlacklistsDto>>(_repository.GetEntities().Where(o => o.UserId == CurrentUser.Id && o.EquipmentId == equipmentId));
            if (result != null)
            {
                return result.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}
