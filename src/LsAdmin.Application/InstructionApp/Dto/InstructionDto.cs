using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Domain.Entities;
using LsAdmin.Utility.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.WebSockets;


namespace LsAdmin.Application.InstructionApp.Dto {
    public class InstructionDto {
        public Guid EquipmentId { get; set; }
        public Guid MethodId { get; set; }
        public InstructionStatus Status { get; set; }
        public string NotifyUrl { get; set; }
        public string Sign { get; set; }
        public long Timestamp { get; set; }
        public string Params { get; set; }
        public SortedDictionary<string, string> ParamsMap {
            get {
                try {
                    return JsonConvert.DeserializeObject<SortedDictionary<string, string>>(Params, new LsJsonSerializerSettings());
                }
                catch (Exception) {
                    return null;
                }
            }
            set {
                Params = JsonConvert.SerializeObject(value, new LsJsonSerializerSettings());
            }
        }
        public string Content { get; set; }
        public SortedDictionary<string, string> ContentMap {
            get {
                try {
                    return JsonConvert.DeserializeObject<SortedDictionary<string, string>>(Content, new LsJsonSerializerSettings());

                }
                catch (Exception) {
                    return null;
                }
            }
            set {
                Content = JsonConvert.SerializeObject(value, new LsJsonSerializerSettings());
            }
        }
        public string Token { get; set; }
        public Guid Id { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

    }
}