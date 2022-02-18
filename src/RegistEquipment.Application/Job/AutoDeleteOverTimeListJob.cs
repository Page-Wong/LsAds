using System;
using Pomelo.AspNetCore.TimedJob;
using RegistEquipment.Application.RegistEquipmentApp;

namespace RegistEquipment.Application.Job
{
     class AutoDeleteOverTimeListJob : Pomelo.AspNetCore.TimedJob.Job
    {
        IRegistEquipmentAppHandler _registEquipmentAppHandler;
        IRegistEquipmentAppService _registEquipmentAppService;
        public AutoDeleteOverTimeListJob( IRegistEquipmentAppService registEquipmentAppServic, IRegistEquipmentAppHandler registEquipmentAppHandler) : base()
        {
            _registEquipmentAppHandler = registEquipmentAppHandler;
            _registEquipmentAppService = registEquipmentAppServic;
        }

        // Begin 起始时间；Interval执行时间间隔，单位是毫秒，建议使用以下格式，此处为10分钟；
        //SkipWhileExecuting是否等待上一个执行完成，true为等待；
        [Invoke(Interval = 1000 * 600, SkipWhileExecuting = true)]
        public void Run()
        {
            _registEquipmentAppHandler.DeleteOverTimeWebSocketsAsync(DateTime.Now);
          //  _registEquipmentAppService.DeleteOverTimeList(DateTime.Now);
        }
    }
}
