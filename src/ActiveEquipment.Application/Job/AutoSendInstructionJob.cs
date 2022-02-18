using ActiveEquipment.Application.ActiveEquipmentApp;
using LsAdmin.Application.InstructionApp;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveEquipment.Application.Job
{
    class AutoSendInstructionJob : Pomelo.AspNetCore.TimedJob.Job {

        IInstructionAppService _instructionAppService;
        IActiveEquipmentAppHandler _activeEquipmentAppHandler;
        IActiveEquipmentAppService _activeEquipmentAppService;
        public AutoSendInstructionJob(IInstructionAppService instructionAppService, IActiveEquipmentAppHandler activeEquipmentAppHandler, IActiveEquipmentAppService activeEquipmentAppService) : base() {
            _instructionAppService = instructionAppService;
            _activeEquipmentAppService = activeEquipmentAppService;
            _activeEquipmentAppHandler = activeEquipmentAppHandler;
        }

        [Invoke(Interval = 1000 * 10, SkipWhileExecuting = true)]
        public void Run() {
            var instructions = _instructionAppService.GetAllReadyList();
            instructions.ForEach(it => {
                var equipment = _activeEquipmentAppService.Get(it.EquipmentId);
                if (equipment != null) {
                    _activeEquipmentAppHandler.SendInstructionAsync(equipment, it,"定时器发送");
                }
            });
        }
    }
}
