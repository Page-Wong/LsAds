using LsAdmin.Application.MaterialApp;
using LsAdmin.Utility.Service;
using System;
using System.Linq;

namespace LsAdmin.Application.PlaceMaterialApp.Dtos
{
    public class PlaceMaterialDto
    {
        public Guid Id { get; set; }
        public string Remarks { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? CreateTime { get; set; }
  
        /// 使用素材
        public Guid MaterialId { get; set; }

        /// <summary>
        /// 场所编码
        /// </summary>
        public Guid PlaceId { get; set; }

        ///<sumary>
        ///素材顺序
        ///</sumary>
        public int Orderby { get; set; }

        /// <summary>
        /// 播放时间（单位：s)
        /// </summary>
        public int Seconds { get; set; }

        public virtual ushort MaterialType
        {
            get
            {
                var service = (IMaterialAppService)HttpHelper.ServiceProvider.GetService(typeof(IMaterialAppService));   
                return service.GetInfo(MaterialId)?.MaterialType ?? 0;
            }
        }

        /// 素材名称
        public string MaterialName
        {
            get
            {
                if (MaterialId == null)
                {
                    return "";
                }
                var service = (IMaterialAppService)HttpHelper.ServiceProvider.GetService(typeof(IMaterialAppService));          
                return service.GetInfo(MaterialId)?.Name;
            }
        }

    }
}
