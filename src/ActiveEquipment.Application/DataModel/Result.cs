using System;

namespace ActiveEquipment.Application.DataModel {
    public class Result {
        public enum ResultCode {
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
            SIGN_MISMATCH,
            INSTRUCTION_EQUIPMENT_MISMATCH,
            ORIGINAL_INSTRUCTION_NOTIFY_NONE,
            RECEIVE_ORIGINAL_INSTRUCTION_NOTIFY_ERROR,
            METHOD_NONE,
            PARAM_INVAILD,
            APP_NONE
        }

        public Result(ResultCode Code = ResultCode.NONE, Exception Exception = null) {
            this.Code = Code;
            this.Exception = Exception;
        }


        public ResultCode Code { get; set; }
        public Exception Exception { get; set; }


        public string Msg {
            get {
                switch (Code) {
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
                    case ResultCode.SIGN_MISMATCH: return "签名不匹配";
                    case ResultCode.INSTRUCTION_EQUIPMENT_MISMATCH: return "指令与设备不匹配";
                    case ResultCode.ORIGINAL_INSTRUCTION_NOTIFY_NONE: return "原始指令通知为空";
                    case ResultCode.RECEIVE_ORIGINAL_INSTRUCTION_NOTIFY_ERROR: return "接收原始指令通知出错";
                    case ResultCode.METHOD_NONE: return "操作方法为空";
                    case ResultCode.PARAM_INVAILD: return "参数有误";
                    case ResultCode.APP_NONE: return "APP不存在";

                    default: return "未知结果";
                }
            }
        }

        public bool IsSuccess() {
            return Code == ResultCode.SUCCESS;
        }
    }

}