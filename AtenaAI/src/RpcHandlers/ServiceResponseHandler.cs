using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atena.RpcHandlers
{
    public class ServiceResponseHandler
    {
        public ServiceResponseHandler() 
        {
            if (AtenaEvent.instance == null)
            {
                Log.Err("AtenaEvent.instance does not exists cannot attach methods");
                return;
            }

            AtenaEvent.instance.OnRecvGrpcResonse += OnGrpcData;
        }

        private void OnGrpcData(byte[]? data)
        {
            if (data == null) return;

            Log.Info(Encoding.UTF8.GetString(data));
        }
    }
}
