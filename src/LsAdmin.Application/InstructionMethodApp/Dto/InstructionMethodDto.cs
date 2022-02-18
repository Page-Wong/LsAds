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

public class InstructionMethodDto {
    public string Name { get; set; }
    public string Method { get; set; }
    public string ParamRole { get; set; }
    public string ResultRole { get; set; }
    public List<InstructionMethodRoleDto> ParamRoleDtos {
        get {
            try {
                return JsonConvert.DeserializeObject<List<InstructionMethodRoleDto>>(ParamRole, new LsJsonSerializerSettings());

            }
            catch (Exception) {
                return null;
            }
        }
        set {
            ParamRole = JsonConvert.SerializeObject(value, new LsJsonSerializerSettings());
        }
    }
    public List<InstructionMethodRoleDto> ResultRoleDtos {
        get {
            try {
                return JsonConvert.DeserializeObject<List<InstructionMethodRoleDto>>(ResultRole, new LsJsonSerializerSettings());

            }
            catch (Exception) {
                return null;
            }
        }
        set {
            ResultRole = JsonConvert.SerializeObject(value, new LsJsonSerializerSettings());
        }
    }

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
