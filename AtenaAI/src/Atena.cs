using atena;
using atena.ServiceType;
using atena.RpcHandlers;

namespace atena
{
    class Atena
    {
        public void InitAtena()
        {
            Log.InitLogger();

            // since we don't really use these vars both
            AtenaEvent _ = new();
            ServiceResponseHandler __ = new();

            // load the config it will create the singleton instance
            Config.LoadFromFile("config.json");

            Services services = new();

            services.DispatchService(new Ping());
        }
    }
}
