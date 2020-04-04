using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeopardToolKit.EventBus
{
    public class EventMessage
    {
        public string Name { get; set; }

        public object Data { get; set; }
    }
}
