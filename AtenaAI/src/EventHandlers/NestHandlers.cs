using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using atena;

namespace AtenaAI.EventHandlers
{
    public class NestHandlers
    {
        public NestHandlers() 
        {
            AtenaEvent.instance.OnPigeonConnected += PigeonConnected;
        }

        private void PigeonConnected(Socket pigeonSocket) {
            Log.Info("Pigeon Connected");
        }
    }
}
