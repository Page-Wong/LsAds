using Microsoft.Extensions.Configuration;

namespace EquipmentService.WebAPI.Common {
    public class WebServiceIpWhitelist {



        public static string KEY = "ApiIpWhitelist";
        private static string[] _ips = null;

        public static string[] Ips => _ips;


        public static void SetIps(IConfigurationSection section) {
            if (section.Value != null) {
                var str = section.Value;
                _ips = str.Split(",");
            }
        }
    }
}
