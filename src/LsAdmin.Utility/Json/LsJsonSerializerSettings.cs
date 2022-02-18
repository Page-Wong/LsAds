using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace LsAdmin.Utility.Json {
    public class LsJsonSerializerSettings : JsonSerializerSettings {
        public LsJsonSerializerSettings() {
            DateFormatString = "yyyy-MM-dd HH:mm:ss";
            ContractResolver = new CamelCasePropertyNamesContractResolver();
            NullValueHandling = NullValueHandling.Ignore;
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore; //设置不处理循环引用
        }
    }

}