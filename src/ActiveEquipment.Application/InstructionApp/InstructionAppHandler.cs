using System;
using LsAdmin.Application.EquipmentApp;
using System.Threading.Tasks;
using LsAdmin.Domain.Entities;
using ActiveEquipment.Application.Common;
using ActiveEquipment.Application.DataModel;
using ActiveEquipment.Application.ActiveEquipmentApp;
using LsAdmin.Application.InstructionLogApp;
using LsAdmin.Application.InstructionApp;
using LsAdmin.Application.InstructionApp.Dto;
using LsAdmin.Application.InstructionLogApp.Dto;
using LsAdmin.Application.EquipmentApp.Dtos;

namespace ActiveEquipment.Application.InstructionApp {
    public class InstructionAppHandler : IInstructionAppHandler {

        IActiveEquipmentAppService _activeEquipmentAppService;
        IEquipmentAppService _equipmentAppService;
        IInstructionLogAppService _instructionLogAppService;
        IInstructionAppService _instructionAppService;
        IActiveEquipmentAppHandler _activeEquipmentAppHandler;
        IInstructionMethodAppService _instructionMethodAppService;
        public InstructionAppHandler(IInstructionMethodAppService instructionMethodAppService, IActiveEquipmentAppHandler activeEquipmentAppHandler, IActiveEquipmentAppService activeEquipmentAppService, IEquipmentAppService equipmentAppService, IInstructionLogAppService instructionLogAppService, IInstructionAppService instructionAppService) : base() {
            _activeEquipmentAppService = activeEquipmentAppService;
            _equipmentAppService = equipmentAppService;
            _instructionLogAppService = instructionLogAppService;
            _instructionAppService = instructionAppService;
            _activeEquipmentAppHandler = activeEquipmentAppHandler;
            _instructionMethodAppService = instructionMethodAppService;
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="instruction">待发送的指令</param>
        /// <param name="isEnforce">是否强制执行，如果不强制执行并且设备在离线状态，则放弃此指令，如果强制执行，则设备离线会加入待发送列表</param>
        /// <returns>发送操作结果</returns>
        public async Task<Result> SendInstructionAsync(InstructionDto instruction, bool isEnforce = false) {
            var activeEquipment = _activeEquipmentAppService.Get(instruction.EquipmentId);
            var result = new Result();
            //设备离线时判断是否强制执行
            if (activeEquipment == null && !isEnforce) {
                result.Code = Result.ResultCode.EQUIPMENT_NONE;
                return result;
            }

            //校验指令信息
            result = CheckInstruction(instruction);
            if (!result.IsSuccess()) {
                return result;
            }
            //InstructionDto instruction;
            try {
                #region 生成并保存指令数据
                instruction.Token = activeEquipment.Token;
                instruction.Status = InstructionStatus.Waiting;
                instruction.Timestamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
                instruction.CreateTime = DateTime.Now;
                _instructionAppService.Insert(ref instruction);
                #endregion
            }
            catch (Exception e) {
                result.Code = Result.ResultCode.SAVE_INSTRUCTION_ERROR;
                result.Exception = e;
                return result;
            }

            return await _activeEquipmentAppHandler.SendInstructionAsync(activeEquipment, instruction);
        }

        /// <summary>
        /// 接收指令执行结果
        /// </summary>
        /// <param name="instructionResult">指令执行结果</param>
        /// <returns>接收指令执行结果的处理结果</returns>
        public Result ReceiveInstructionResult(InstructionResultDto instructionResult) {
            var result = CheckInstructionResult(instructionResult, out var instruction, out var activeEquipment);
            if (!result.IsSuccess()) {
                return result;
            }
            #region 记录信息接收日志
            LogHelper.Log(ActiveEquipmentLog.ActiveEquipmentLogType.Received, activeEquipment, instructionResult);
            #endregion
            try {
                #region 更新指令状态
                instruction.Status = instructionResult.EquipmentResult.IsProcessing() ? InstructionStatus.Processing : InstructionStatus.Done;
                _instructionAppService.Update(instruction);
                #endregion

                #region 记录指令日志
                var log = new InstructionLogDto {
                    InstructionId = instruction.Id,
                    EquipmentId = instruction.EquipmentId,
                    Type = InstructionLogType.Receive,
                    InstructionStatus = instruction.Status,
                    Remarks = instructionResult.Result
                };
                _instructionLogAppService.Insert(ref log);
                #endregion
                result.Code = Result.ResultCode.SUCCESS;
            }
            catch (Exception e) {
                result.Code = Result.ResultCode.SAVE_INSTRUCTION_ERROR;
                result.Exception = e;
                return result;
            }
            return result;
        }

        /// <summary>
        /// 接收设备原始指令的接收结果通知（设备在接收到之后会发送这个通知到服务器提醒已收到指令）
        /// </summary>
        /// <param name="dto">原始指令的接收结果通知</param>
        /// <returns>处理结果</returns>
        public Result ReceiveOriginalInstructionNotify(OriginalInstructionNotifyDto dto) {
            var result = CheckOriginalInstructionNotify(dto, out var instruction, out var activeEquipment);
            if (!result.IsSuccess()) {
                return result;
            }
            #region 记录信息接收日志
            LogHelper.Log(ActiveEquipmentLog.ActiveEquipmentLogType.Received, activeEquipment, dto);
            #endregion
            try {
                #region 更新指令状态
                instruction.Status = instruction.Status == InstructionStatus.Send ? InstructionStatus.Receive : instruction.Status;
                _instructionAppService.Update(instruction);
                #endregion

                #region 记录指令日志
                var log = new InstructionLogDto {
                    InstructionId = instruction.Id,
                    EquipmentId = instruction.EquipmentId,
                    Type = InstructionLogType.Receive,
                    InstructionStatus = instruction.Status,
                    Remarks = dto.Result
                };
                _instructionLogAppService.Insert(ref log);
                #endregion
                result.Code = Result.ResultCode.SUCCESS;
            }
            catch (Exception e) {
                result.Code = Result.ResultCode.SAVE_INSTRUCTION_ERROR;
                result.Exception = e;
                return result;
            }
            return result;
        }

        /// <summary>
        /// 检查发送的指令是否合法
        /// </summary>
        /// <param name="dto">指令</param>
        /// <returns>操作结果</returns>
        private Result CheckInstruction(InstructionDto dto) {
            var result = new Result(Result.ResultCode.SUCCESS);
            var method = _instructionMethodAppService.Get(dto.MethodId);
            if (method == null) {
                result.Code = Result.ResultCode.METHOD_NONE;
                return result;
            }
            //TODO G 检测该设备是否有此方法

            //检查参数是否合法
            if (method.ParamRoleDtos != null) {
                method.ParamRoleDtos.ForEach(it => {
                    if (!dto.ParamsMap.TryGetValue(it.Name, out var value)) {
                        result.Code = Result.ResultCode.PARAM_INVAILD;
                        return;
                    }
                });
            }
            
            return result;
        }

        /// <summary>
        /// 检查设备的指令执行结果是否合法
        /// </summary>
        /// <param name="dto">设备的指令结果</param>
        /// <param name="instruction">对应的指令</param>
        /// <returns>操作结果</returns>
        private Result CheckInstructionResult(InstructionResultDto dto, out InstructionDto instruction, out ActiveEquipmentDto activeEquipment) {
            var result = new Result();
            activeEquipment = _activeEquipmentAppService.GetByToken(dto.Token);
            instruction = null;
            if (activeEquipment == null) {
                result.Code = Result.ResultCode.ACTIVE_EQUIPMENT_NONE;
                return result;
            }
            if (dto.Sign != SignHelper.Sign(MapperHelper.Mapper.Map<InstructionResult>(dto), activeEquipment.Token, activeEquipment.DeviceId)) {
                result.Code = Result.ResultCode.SIGN_MISMATCH;
                return result;
            }
            instruction = _instructionAppService.Get(Guid.Parse(dto.InstructionId));
            if (instruction == null) {
                result.Code = Result.ResultCode.INSTRUCTION_NONE;
                return result;
            }
            if (instruction.EquipmentId != activeEquipment.EquipmentId) {
                result.Code = Result.ResultCode.INSTRUCTION_EQUIPMENT_MISMATCH;
                return result;
            }

            return new Result(Result.ResultCode.SUCCESS);
        }

        /// <summary>
        /// 检查设备原始指令的接收结果通知是否合法
        /// </summary>
        /// <param name="dto">设备的接收结果通知</param>
        /// <param name="instruction">对应的指令</param>
        /// <returns>操作结果</returns>
        private Result CheckOriginalInstructionNotify(OriginalInstructionNotifyDto dto, out InstructionDto instruction, out ActiveEquipmentDto activeEquipment) {
            var result = new Result();
            instruction = null;
            activeEquipment = null;
            if (dto == null) {
                result.Code = Result.ResultCode.ORIGINAL_INSTRUCTION_NOTIFY_NONE;
                return result;
            }
            activeEquipment = _activeEquipmentAppService.GetByToken(dto.Token);
            if (activeEquipment == null) {
                result.Code = Result.ResultCode.ACTIVE_EQUIPMENT_NONE;
                return result;
            }

            if (dto.Sign != SignHelper.Sign(MapperHelper.Mapper.Map<OriginalInstructionNotify>(dto), activeEquipment.Token, activeEquipment.DeviceId)) {
                result.Code = Result.ResultCode.SIGN_MISMATCH;
                return result;
            }
            instruction = _instructionAppService.Get(Guid.Parse(dto.InstructionMessage?.InstructionId));
            if (instruction == null) {
                result.Code = Result.ResultCode.INSTRUCTION_NONE;
                return result;
            }
            if (instruction.EquipmentId != activeEquipment.EquipmentId) {
                result.Code = Result.ResultCode.INSTRUCTION_EQUIPMENT_MISMATCH;
                return result;
            }

            return new Result(Result.ResultCode.SUCCESS);
        }
    }

}