using Newtonsoft.Json;
using NLog;
using ActiveEquipment.Application.DataModel;
using LsAdmin.Utility.Json;
using LsAdmin.Application.EquipmentApp.Dtos;

namespace ActiveEquipment.Application.Common {

    public class LogHelper {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Log(ActiveEquipmentLog log) {

            LogEventInfo ei = new LogEventInfo(LogLevel.Info, "", log.Message);
            ei.Properties["type"] = log.Type;
            ei.Properties["equipmentId"] = log.EquipmentId?.ToString();
            ei.Properties["token"] = log.Token;
            ei.Properties["ip"] = log.Ip.ToString();
            ei.Properties["port"] = log.Port;
            Logger.Log(ei);
        }

        public static void Error(ActiveEquipmentLog log) {

            LogEventInfo ei = new LogEventInfo(LogLevel.Info, "", log.Message);
            ei.Properties["type"] = log.Type;
            ei.Properties["equipmentId"] = log.EquipmentId?.ToString();
            ei.Properties["token"] = log.Token;
            ei.Properties["ip"] = log.Ip.ToString();
            ei.Properties["port"] = log.Port;
            Logger.Error(ei);
        }

        public static void Log(ActiveEquipmentLog.ActiveEquipmentLogType type, ActiveEquipmentDto activeEquipment, object obj) {
            Log(new ActiveEquipmentLog {
                Token = activeEquipment.Token,
                Type = type,
                Ip = activeEquipment.Ip,
                Port = activeEquipment.Port,
                EquipmentId = activeEquipment.EquipmentId,
                Message = JsonConvert.SerializeObject(obj, new LsJsonSerializerSettings())
            });
        }
    }
}