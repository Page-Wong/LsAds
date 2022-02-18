using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LsAdmin.WebAPI.Models
{
    public class PlayHistoryModel {
        public string id { get; set; }
        public string deviceId { get; set; }
        public string playId { get; set; }
        public string orderTimeId { get; set; }
        public string serialVersionUID { get; set; }

        public string startTime { get; set; }
        public string endTime { get; set; }

        public long frontalfaceCount { get; set; }
        public long profilefaceCount { get; set; }
        public int clickCount { get; set; }
        public bool isUpload { get; set; }

    }
}
