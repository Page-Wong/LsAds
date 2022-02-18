using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Domain.Entities;
using LsAdmin.Utility.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.WebSockets;

namespace ActiveEquipment.Application.DataModel {
    public class InstructionResultDto : InstructionResult {

        public InstructionResult InstructionResult {
            get {
                return new InstructionResult {
                    InstructionId = InstructionId,
                    Original = Original,
                    Result = Result,
                    Token = Token,
                    Timestamp = Timestamp,
                    Sign = Sign
                };
            }
        }

        public InstructionMessage InstructionMessage {
            get {
                try {
                    return JsonConvert.DeserializeObject<InstructionMessage>(Original, new LsJsonSerializerSettings());
                }
                catch (Exception) {
                    return null;
                }
            }
        }
        public EquipmentResult EquipmentResult {
            get {
                try {
                    return JsonConvert.DeserializeObject<EquipmentResult>(Result, new LsJsonSerializerSettings());
                }
                catch (Exception) {
                    return null;
                }
            }
        }
        public ConcurrentDictionary<string, string> ContentMap {
            get {
                try {
                    return JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(Content, new LsJsonSerializerSettings());

                }
                catch (Exception) {
                    return null;
                }
            }
            set {
                Content = JsonConvert.SerializeObject(value, new LsJsonSerializerSettings());
            }
        }

    }
}