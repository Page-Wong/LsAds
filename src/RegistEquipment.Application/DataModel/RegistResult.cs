using System;
using System.Collections.Generic;
using System.Text;

namespace RegistEquipment.Application.DataModel
{
     public class RegistResult
    {
        public enum ResultCode
        {
            NONE,
            SUCCESS,
            EQUIPMENT_NONE,
            EQUIPMENT_TOKEN_NONE,
            EQUIPMENT_ALREADY_EXISTS,
            SEND_INSTRUCTION_ERROR,
            SAVE_INSTRUCTION_ERROR,
            ACTIVE_EQUIPMENT_NONE,
            ACTIVE_EQUIPMENT_WEBSOCKET_CLOSE,
            INSTRUCTION_NONE,
            RECEIVE_INSTRUCTION_RESULT_ERROR,

            REGIST_NONE,
            REGIST_OVERTIME,
            REGIST_OWNERUSERID_NONE,
            REGIST_EQUIPMENTNAME_NONE,
            EQUIPMENT_SAMENAME_EXISTS,
            EQUIPMENT_REGISTERED,   
        }

        public RegistResult(ResultCode Code = ResultCode.NONE, Exception Exception = null)
        {
            this.Code = Code;
            this.Exception = Exception;
        }


        public ResultCode Code { get; set; }
        public Exception Exception { get; set; }


        public string Msg
        {
            get
            {
                switch (Code)
                {
                    case ResultCode.NONE: return "未知结果";
                    case ResultCode.SUCCESS: return "成功";
                    case ResultCode.EQUIPMENT_NONE: return "设备信息为空";
                    case ResultCode.EQUIPMENT_TOKEN_NONE: return "设备令牌信息为空";
                    case ResultCode.EQUIPMENT_ALREADY_EXISTS: return "设备已经存在";
                    case ResultCode.SEND_INSTRUCTION_ERROR: return "指令发送错误";
                    case ResultCode.SAVE_INSTRUCTION_ERROR: return "保存指令出错";
                    case ResultCode.ACTIVE_EQUIPMENT_NONE: return "活动设备为空";
                    case ResultCode.ACTIVE_EQUIPMENT_WEBSOCKET_CLOSE: return "活动设备WebSocket关闭";
                    case ResultCode.INSTRUCTION_NONE: return "指令为空";
                    case ResultCode.RECEIVE_INSTRUCTION_RESULT_ERROR: return "接收指令执行结果出错";

                    case ResultCode.REGIST_NONE: return "注册申请信息失效";
                    case ResultCode.REGIST_OVERTIME: return "注册申请超时";
                    case ResultCode.REGIST_OWNERUSERID_NONE:  return "所有者信息为空";
                    case ResultCode.REGIST_EQUIPMENTNAME_NONE: return "设备名称为空";
                    case ResultCode.EQUIPMENT_SAMENAME_EXISTS: return "存在同设备名称记录";
                    case ResultCode.EQUIPMENT_REGISTERED:      return "设备已注册";
                        
                    default: return "未知结果";
                }
            }
        }

        public bool IsSuccess()
        {
            return Code == ResultCode.SUCCESS;
        }
    }
}
