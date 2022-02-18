using LsAdmin.Application.Imp;
using LsAdmin.Application.OrderApp;
using LsAdmin.Application.OrderApp.Dtos;
using LsAdmin.Application.PlaceMaterialApp.Dtos;
using LsAdmin.Domain.Entities;
using LsAdmin.Domain.IRepositories;
using LsAdmin.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LsAdmin.Application.PlaceMaterialApp
{
    public class PlaceMaterialAppService: BaseAppService<PlaceMaterial,PlaceMaterialDto>, IPlaceMaterialAppService
    {
        private readonly IPlaceMaterialRepository _repository;
        private readonly LsAdminDbContext _dbcontext;
        private readonly IOrderAppService _orderService;

        // private readonly IMaterialRepository _materialrepository;

        public PlaceMaterialAppService(IPlaceMaterialRepository repository, LsAdminDbContext dbcontext, IHttpContextAccessor httpContextAccessor, IOrderAppService orderService) : base(repository, httpContextAccessor) {
            _repository = repository;
            _orderby = x => x.CreateTime;
            _dbcontext = dbcontext;
            _orderService = orderService;
        }

        public List<PlaceMaterialDto> GetPlaceMaterials(Guid placeid, ushort materialType=0)
        {
            try
            {
               var PlaceMaterias = GetAllList().Where(w => w.PlaceId == placeid).OrderBy(o => o.Orderby)?.ToList() ?? new List<PlaceMaterialDto>();
                if (materialType != 0)
                    PlaceMaterias = PlaceMaterias.Where(w => w.MaterialType == materialType)?.ToList();

                return PlaceMaterias;
            }
            catch (Exception e)
            {
                return new List<PlaceMaterialDto>();
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="startPage">页码</param>
        /// <param name="pageSize">单页数据数</param>
        /// <param name="rowCount">行数</param>
        /// <param name="placeid">场合编码</param>
        /// <param name="materialType">素材类型{图片=1 ,视频=2，全部=0} </param>
        /// <returns></returns>
        public List<PlaceMaterialDto> GetPageListByType(int startPage, int pageSize,  out int rowCount, Guid placeid, ushort materialType)
        {
            var result = GetPlaceMaterials(placeid, materialType);                      
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public bool SaveToOrder(Guid placeId, List<PlaceMaterialDto> dtos)
        {
            using (var transactionSaveToOrder = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    //1.删除PlaceMaterial原记录
                    DeleteBatch(GetAllList().Where(w => w.PlaceId == placeId).Select(s =>s.Id).ToList());

                    //2.插入PlaceMaterial记录
                    for (int i = 0; i < dtos.Count(); i++)
                    {
                        var item = dtos.ElementAt(i);
                        InsertOrUpdate(ref item);
                    }

                    //3.将场地的其他场所order 的状态改为取消
                    foreach(var orderid in _orderService.GetePlaceMaterialOrde(placeId))
                    {
                        var order = _orderService.Get(orderid);
                        if (order != null)
                        {
                            order.Status = 6;
                            _orderService.Update(order);
                        }
                    } 

                    //4.生成场所播放order
                    /*if (dtos.Count > 0) {
                        if (_orderService.SavePlaceMaterialOrder(placeId, dtos) == false) {
                            transactionSaveToOrder.Rollback();
                            return false;
                        }
                    }*/
                    transactionSaveToOrder.Commit();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
