using atena.RpcHandlers;
using AtenaAI.EventHandlers;
using Avalonia.Threading;
using System;
using System.Net.Sockets;

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
