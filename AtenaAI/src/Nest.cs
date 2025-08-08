using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace atena
{
    public class Nest
    {
        private IPEndPoint? _nestAddress;
        private readonly Socket _listenSocket;
    
        public Nest()
        {
            string serverAddr = Config.Instance.Data.serverAddr;
            string serverPort = Config.Instance.Data.tcpPort;

            IPAddress ipAddress = Dns.GetHostAddresses(serverAddr)
                                     .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);


            int srvPort = 0;

            try
            {
                srvPort = int.Parse(serverPort);
            }
            catch(ArgumentNullException)
            {
                Log.Err("Provided string {0}, cannot be converted to int", serverPort);
                return;
            }
            catch(FormatException)
            {
                Log.Err("Provided string {0}, cannot be converted to int", serverPort);
                return;
            }
            catch (OverflowException)
            {
                Log.Err("Provided string {0}, cannot be converted to int", serverPort);
                return;
            }

            _nestAddress = new IPEndPoint(ipAddress, srvPort);

            _listenSocket = new Socket(_nestAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _listenSocket.Bind(_nestAddress);

            _listenSocket.Listen(1);
        }


        public void Accept(bool blocking = false)
        {
            if(blocking)
            {
                AtenaEvent.instance?.FireOnPigeonConnected(_listenSocket.Accept());
                return;
            }

            Task.Run(() => {
                AtenaEvent.instance?.FireOnPigeonConnected(_listenSocket.Accept());
            });

            return;
        }
    }
}
