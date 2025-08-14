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

            // will init all types of handlers
            AtenaEvent atenaEvent = new();
            //ServiceResponseHandler serviceResponseHandler = new();

            // load the config it will create the singleton instance
            Config.LoadFromFile("config.json");



            Nest nest = new Nest();
            nest.Accept(false);


            // service should be the last thing getting init cuz this runs the main_service.py 
            // so putting it last to init make sure that everything else is up and running before we start main_service
            Services services = new();


            // auto start the listening service
            if (Config.Instance.Data.listenDesktopAudio)
            {
                services.RegisterAutoRunService(new ListenDeskAudio());
            }

            // after everything been setup start the main service
            services.Main();
        }
    }
}
