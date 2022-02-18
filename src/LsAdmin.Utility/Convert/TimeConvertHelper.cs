using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LsAdmin.Utility.Convert {
    public class TimeConvertHelper {
        /// <summary>
        /// 将对象转换为byte数组
        /// </summary>
        /// <param name="obj">被转换对象</param>
        /// <returns>转换后byte数组</returns>
        public static string TimeDiffString(DateTime newDate, DateTime oldDate) {
            if (newDate == null || oldDate == null) {
                return "";
            }
            var stime = newDate.Subtract(oldDate).TotalSeconds;
            var rtime = string.Format("{0:M-d H:m}", oldDate);
            var htime = string.Format("{0:H:m}", oldDate);
            if (newDate.Subtract(oldDate).TotalSeconds < 60 * 30) {
                return "刚刚";
            }
            if (newDate.Subtract(oldDate).TotalMinutes < 60) {
                var min = Math.Floor(stime / 60);
                return min + "分钟前";
            }
            if (newDate.Subtract(oldDate).TotalHours < 24) {
                var h = Math.Floor(stime / (60 * 60));
                return h + "小时前 ";
            }
            var aaa = newDate.Date.Subtract(oldDate.Date).TotalDays;
            if (newDate.Date.Subtract(oldDate.Date).TotalDays < 2) {
                return "昨天 " + htime;
            }
            if (newDate.Date.Subtract(oldDate.Date).TotalDays < 3) {
                return "前天 " + htime;
            }
            return string.Format("{0:yyyy-M-d H:m}", oldDate);
        }

        public static long GetTimeStamp() {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

    }
}
