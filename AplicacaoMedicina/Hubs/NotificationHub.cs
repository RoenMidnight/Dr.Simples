using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;

namespace AplicacaoMedicina.Hubs
{
    [HubName("notificationHub")]
    public class NotificationHub: Hub
    {
        
    }
}