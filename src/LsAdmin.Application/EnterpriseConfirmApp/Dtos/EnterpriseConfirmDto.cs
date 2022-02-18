using LsAdmin.Application.OrderEquipmentApp.Dtos;
using LsAdmin.Application.OrderMaterialApp.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LsAdmin.Application.EnterpriseConfirmApp.Dtos {
    public class EnterpriseConfirmDto
    {
        public Guid Id { get; set; }
        public string Remarks { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? CreateTime { get; set; }

        //企业名称

        public string Name { get; set; }

        //营业执照注册号
        [Required]
        public string RegisteredNumber { get; set; }

        //营业执照所在地
        [Required]
        public string LicenseAddress { get; set; }

        //营业期限
        [Required]
        public string Period { get; set; }

        //常用地址
        [Required]
        public string Location { get; set; }

        //联系电话
        public string Phone { get; set; }

        //营业执照副本扫描件
        public byte[] DuplicateLicenseScan { get; set; }

        //加盖公章的副本
        public byte[] DuplicateLicenseSeal { get; set; }

        //组织机构代码
        public string OrganizationCode { get; set; }

        //营业范围
        public string ManagementScope { get; set; }

        //注册资金
        public string RegisteredCapital { get; set; }

        //传真
        public string Fax { get; set; }

        //校验码
        public string VerificationCode { get; set; }
    }
}