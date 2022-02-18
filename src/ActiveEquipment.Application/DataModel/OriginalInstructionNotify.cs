using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.WebSockets;

namespace ActiveEquipment.Application.DataModel {
    public class OriginalInstructionNotify {
        public string Original { get; set; }
        public string Result { get; set; }
        public string Token { get; set; }
        public long Timestamp { get; set; }
        public string Sign { get; set; }

    }
}