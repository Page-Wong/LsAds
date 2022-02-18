using System;
using System.Net;

namespace ActiveEquipment.Application.DataModel {
    public class ActiveEquipmentLog {
        public enum ActiveEquipmentLogType {
            Connecting,
            Connected,
            ConnecteFail,
            Disconnected,
            Sended,
            Received
        }

        public ActiveEquipmentLogType Type { get; set; }
        public Guid? EquipmentId { get; set; }
        public string Token { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Message { get; set; }

    }

}