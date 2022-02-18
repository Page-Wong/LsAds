using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LsAdmin.Application.CollectionsBlacklistsApp.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LsAdmin.MVC.Controllers
{
    public class CollectionsBlacklistsController : LsAdminControllerBase
    {
        // GET: /<controller>/

        private readonly ICollectionsBlacklistsAppService _service;

        public CollectionsBlacklistsController(ICollectionsBlacklistsAppService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult favoriteAD(Guid equipmentId,ushort favoriteType,ushort scType)
        {
            CollectionsBlacklistsDto favorite = _service.GetFavoriteInfo(equipmentId);
            if (scType == 1)
            {
                if (favorite == null)
                {
                    CollectionsBlacklistsDto newfavorite = new CollectionsBlacklistsDto();
                    newfavorite.UserId = CurrentUser.Id;
                    newfavorite.EquipmentId = equipmentId;
                    newfavorite.CreateTime = DateTime.Now;
                    newfavorite.FavoriteType = favoriteType;
                    _service.InsertOrUpdate(ref newfavorite);
                    return Json(new
                    {
                        Result = "Success",

                    });
                }
                else
                {
                    _service.Delete(favorite.Id);
                    CollectionsBlacklistsDto newfavorite = new CollectionsBlacklistsDto();
                    newfavorite.UserId = CurrentUser.Id;
                    newfavorite.EquipmentId = equipmentId;
                    newfavorite.CreateTime = DateTime.Now;
                    newfavorite.FavoriteType = favoriteType;
                    _service.InsertOrUpdate(ref newfavorite);
                    return Json(new
                    {
                        Result = "Success",
                    });
                }
            }
            else
            {
                _service.Delete(favorite.Id);
                return Json(new
                {
                    Result = "Success",
                });
            }
        }
    }
}
