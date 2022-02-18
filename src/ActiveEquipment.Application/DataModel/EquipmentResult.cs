using System;

namespace ActiveEquipment.Application.DataModel {
    public class EquipmentResult {

        public string Code { get; set; }
        public object Exception { get; set; }
        public string Msg { get; set; }

        public bool IsSuccess() {
            return Code == "1";
        }

        public bool IsProcessing() {
            return Code == "2";
        }
    }

}