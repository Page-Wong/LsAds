using LsAdmin.Application.Imp;
using System;
using System.Collections.Generic;
using System.Text;

namespace LsAdmin.Application.CollectionsBlacklistsApp.Dtos
{
    public  interface ICollectionsBlacklistsAppService:IBaseAppService<CollectionsBlacklistsDto>
    {

        ///<summary>
        ///获取当前用户的黑名单
        ///</summary>
        ///<returns></returns>
        List<CollectionsBlacklistsDto> GetBlacklists();

        ///<summary>
        ///获取当前用户的黑名单列表
        ///</summary>
        ///<returns></returns>
        List<CollectionsBlacklistsDto> GetBlacklistsPageList(int startPage, int pageSize, out int rowCount);

        ///<summary>
        ///获取当前用户的收藏
        ///</summary>
        ///<returns></returns>
        List<CollectionsBlacklistsDto> GetCollections();

        ///<summary>
        ///获取当前用户的收藏列表
        ///</summary>
        ///<returns></returns>
        List<CollectionsBlacklistsDto> GetCollectionsPageList(int startPage, int pageSize, out int rowCount);

        ///<summary>
        ///返回该用户该设备是否是收藏或者黑名单
        ///</summary>
        ///<returns></returns>
        ushort FavoriteType(Guid equipmentId);


        ///<summary>
        ///返回该用户该设备的收藏/黑名单信息
        ///</summary>
        ///<returns></returns>
        CollectionsBlacklistsDto GetFavoriteInfo(Guid equipmentId);
    }
}
