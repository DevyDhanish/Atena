using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atena
{
    public interface Service
    {
        public abstract void SetData(byte[]? data);
        public abstract byte[]? GetData();
        public abstract string GetServiceName();
        public abstract void SetServiceName(string serviceName);
        public abstract void SetServiceId(atenaGrpc.ServiceId id);
        public abstract atenaGrpc.ServiceId GetServiceId();
    }
}
