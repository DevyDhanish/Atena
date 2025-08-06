using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atena
{
    public class AtenaEvent
    {
        public static AtenaEvent? instance { get; private set; }

        public AtenaEvent()
        {
            if (instance == null) instance = this;
            else Log.Err("AtenaEvent instance already exists");
        }

        public Action<byte[]?>? OnRecvGrpcResonse;

        public void FireOnRecvGrpcResponseEvent(byte[]? res)
        {
            OnRecvGrpcResonse?.Invoke(res);
        }
    }
}
