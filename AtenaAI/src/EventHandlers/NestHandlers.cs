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
            AtenaEvent.instance.OnPigeonDataRecieved += OnDataRecieved;
        }

        private void PigeonConnected(Socket pigeonSocket) {
            Log.Info("Pigeon Connected");

            Nest.ListenForData(pigeonSocket);
        }

        private void OnDataRecieved(Socket clientSocket, atenaNest.StreamData? data)
        {
            if (data == null) return;
        }
    }
}
