using atena.RpcHandlers;
using AtenaAI.EventHandlers;
using Avalonia.Threading;
using System;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace atena
{
    public class AtenaEvent
    {
        private UIEventRouter uIEventRouter;
        private NestHandlers _nestEventHandler;
        private ServiceResponseHandler _serviceResponseHandler;
        public static AtenaEvent? instance { get; private set; }

        public AtenaEvent()
        {
            if (instance == null) instance = this;
            else Log.Err("AtenaEvent instance already exists");

            uIEventRouter = new UIEventRouter();
            _nestEventHandler = new NestHandlers();
            _serviceResponseHandler = new ServiceResponseHandler();
        }

        // some event don't need to return anything and some don't even take parameters
        // so keep this consistency
        // event -> no param, no return = Action
        // event -> x param, no return = Action<x>?
        // event -> x param, y return = Func<x, y>?
        public Action<byte[]?>? OnRecvGrpcResonse;
        public Action<atenaGrpc.ServiceId>? OnServiceStarted;
        public Action<atenaGrpc.ServiceId>? OnServiceStopped;
        public Action<Socket>? OnPigeonConnected;
        public Action<Socket, atenaNest.StreamData?>? OnPigeonDataRecieved;
        
        public void FireOnPegionDataRecieved(Socket clientSocket, atenaNest.StreamData? data)
        {
            OnPigeonDataRecieved?.Invoke(clientSocket, data);

            string? defaultData = null;
            atenaNest.DataType defaultDataType;

            if (data == null)
            {
                defaultData = "";
                defaultDataType = atenaNest.DataType.NormalText;
            }
            else
            {
                defaultData = Encoding.UTF8.GetString(data.Data.ToArray());
                defaultDataType = data.DataType;
            }

            Dispatcher.UIThread.Post(() =>
            {
                UIEventRouter.instance?.FireOnChatTextRecieved(defaultData, defaultDataType);
            });
        }

        public void FireOnPigeonConnected(Socket pigeonSocket)
        {
            OnPigeonConnected?.Invoke(pigeonSocket);
        }

        public void FireOnRecvGrpcResponseEvent(byte[]? res)
        {
            OnRecvGrpcResonse?.Invoke(res);
        }
        public void FireOnServiceStarted(atenaGrpc.ServiceId id)
        {
            OnServiceStarted?.Invoke(id);

            Dispatcher.UIThread.Post(() =>
            {
                uIEventRouter.FireOnServiceStarted(
                    Services.GetServiceNameById(id)
                );
            });
        }
        public void FireOnServiceStopped(atenaGrpc.ServiceId id)
        {
            OnServiceStopped?.Invoke(id);

            Dispatcher.UIThread.Post(() =>
            {
                uIEventRouter.FireOnServiceStopped(
                    Services.GetServiceNameById(id)
                );
            });
        }
    }
}
