using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.WebSockets;

public class InstructionMethodRoleDto {
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsRequired { get; set; }

}
