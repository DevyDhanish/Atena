using atena;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtenaAI.EventHandlers
{
    public class UIEventRouter
    {
        public static UIEventRouter? instance { get; private set; }

        public UIEventRouter()
        {
            if (instance == null) instance = this;
            else Log.Err("UIEventRouter instance already exists");
        }

        // these events also follow the same style as AtenaEvent
        // but these will only be called when a event happens that should do some change in UI
        // so all the method that change UI should hook to these events rather than AtenaEvent
        public Action<string>? UI_OnServiceStarted { get; set; }
        public Action<string>? UI_OnServiceStopped { get; set; }

        public void FireOnServiceStarted(string serviceName)
        {
            UI_OnServiceStarted?.Invoke(serviceName);
        }
        public void FireOnServiceStopped(string serviceName)
        {
            UI_OnServiceStopped?.Invoke(serviceName);
        }
    }
}
