using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Model
{
    public class NotificationSendStatus
    {
        public bool IsSuccessful { get; set; }
        public string FailureMessage { get; set; }
        public string MessageId { get; set; }
        public string Name { get; set; }
    }
}
