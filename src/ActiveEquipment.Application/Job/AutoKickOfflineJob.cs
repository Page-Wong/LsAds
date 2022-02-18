using ActiveEquipment.Application.ActiveEquipmentApp;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActiveEquipment.Application.Job
{
    class AutoKickOfflineJob : Pomelo.AspNetCore.TimedJob.Job {
        
        IActiveEquipmentAppService _activeEquipmentAppService;
        IActiveEquipmentAppHandler _activeEquipmentAppHandler;
        public AutoKickOfflineJob(IActiveEquipmentAppService activeEquipmentAppService, IActiveEquipmentAppHandler activeEquipmentAppHandler) : base() {
            _activeEquipmentAppService = activeEquipmentAppService;
            _activeEquipmentAppHandler = activeEquipmentAppHandler;
        }
        
        [Invoke(Interval = 1000 * 15, SkipWhileExecuting = true)]
        public void Run() {
            var nowTimeSpan = new TimeSpan(DateTime.Now.Ticks);
            var timeoutAE = _activeEquipmentAppService.GetAllList().Where(it => nowTimeSpan.Subtract(new TimeSpan(it.LastConnectTime.Ticks)).Duration().Seconds>15).ToList();
            timeoutAE.ForEach(it => {
                _activeEquipmentAppHandler.OnDisconnected(it.WebSocket);
            });
        }
    }
}
