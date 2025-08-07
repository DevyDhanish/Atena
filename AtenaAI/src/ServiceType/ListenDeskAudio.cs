using atena;
using atenaGrpc;
using Grpc.Core;
using System;
using System.Text;

namespace atena.ServiceType
{
    public class ListenDeskAudio : Service
    {
        private string _serviceName;
        private byte[]? _data;
        private atenaGrpc.ServiceId _serviceId;
        public ListenDeskAudio()
        {
            SetServiceId(atenaGrpc.ServiceId.ListenToDesktopAudio);
            SetServiceName(Services.GetServiceNameById(_serviceId));
            SetData(null);
        }

        public void SetData(byte[]? data)
        {
            if (data == null)
            {
                Log.Warn("No data provided, using default");
                _data = Encoding.UTF8.GetBytes(Services.GetRandomData());
                return;
            }

            _data = data;
        }

        public byte[]? GetData()
        {
            return _data;
        }

        public string GetServiceName()
        {
            return _serviceName;
        }

        public void SetServiceName(string serviceName)
        {
            _serviceName = serviceName;
        }

        public void SetServiceId(ServiceId id)
        {
            _serviceId = id;
        }

        public ServiceId GetServiceId()
        {
            return _serviceId;
        }
    }
}
