using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.WebSockets;

namespace ActiveEquipment.Application.DataModel {
    public class InstructionMessage {
        public string InstructionId { get; set; }
        public string NotifyUrl { get; set; }
        public string Key { get; set; }
        public string Sign { get; set; }
        public long Timestamp { get; set; }
        public SortedDictionary<string, string> Params { get; set; }
        public SortedDictionary<string, string> Content { get; set; }
    }
}