using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RegistEquipment.Application.DataModel
{
    public class RegistMessage
    {
        public String methodName;
        public ConcurrentDictionary<string, string> arguments { get; set; }
    }
}
