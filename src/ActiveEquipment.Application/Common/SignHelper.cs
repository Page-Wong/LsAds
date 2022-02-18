using LsAdmin.Application.EquipmentApp.Dtos;
using LsAdmin.Application.UserApp.Dtos;
using LsAdmin.Domain.Entities;
using LsAdmin.Utility.Json;
using LsAdmin.Utility.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Web;

namespace ActiveEquipment.Application.Common {
    public class SignHelper {
        public static string Sign(Object obj, string token, string deviceId) {
            var json = JsonConvert.SerializeObject(obj, new LsJsonSerializerSettings());
            var objMap = JsonConvert.DeserializeObject<SortedDictionary<string, Object>>(json, new LsJsonSerializerSettings());
            if (objMap.ContainsKey("sign")) {
                objMap.Remove("sign");
            }
            if (objMap.ContainsKey("token")) {
                objMap.Remove("token");
            }
            objMap.Add("token", token);

            json = JsonConvert.SerializeObject(objMap, new LsJsonSerializerSettings()).Replace("\\\"", "").Replace("\"", "");
            var md5 = Md5Helper.MD5Encrypt(json, deviceId);

            return HttpUtility.UrlEncode(md5, Encoding.UTF8);
        }
    }
}