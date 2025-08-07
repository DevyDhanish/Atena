using atena;
using atena.ServiceType;
using atena.RpcHandlers;
using AtenaAI.EventHandlers;

namespace atena
{
    class Atena
    {
        public void InitAtena()
        {
            Log.InitLogger();

            AtenaEvent atenaEvent = new();
            ServiceResponseHandler serviceResponseHandler = new();

            // load the config it will create the singleton instance
            Config.LoadFromFile("config.json");

            Services services = new();
        }
    }
}
