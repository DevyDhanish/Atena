using atena.ServiceType;
using atenaGrpc;
using Grpc.Net.Client;
using System.ComponentModel;
using System.Diagnostics;
using atena.RpcHandlers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace atena
{
    public class Services
    {
        private static AtenServices.AtenServicesClient? _client;
        private static GrpcChannel? _channel;

        public static Services? Instance { get; private set; }

        public class ServiceFilePath
        {
            public string? serviceName;
            public string? serviceFilePath;
        }
        public Services()
        {
            if (Instance == null) Instance = this;
            else
            {
                Log.Err("Services.Instance already exists");
                return;
            }

            _client = null;
            if (!StartMainService())
            {
                Log.Err("Something went wrong while starting the main service");
            }

            ConnectToMainService();
        }

        // change this file if you change proto/servicecmd.proto
        // make sure all the serviceid correspond to their name
        // this function is not needed by the program but. i use them here and there
        // not needed for actuall functionality.
        public static string GetServiceNameById(atenaGrpc.ServiceId serviceId)
        {
            switch(serviceId)
            {
                case atenaGrpc.ServiceId.Ping:
                    return "Ping";

                case atenaGrpc.ServiceId.StartService:
                    return "StartService";

                case atenaGrpc.ServiceId.SeeScreen:
                    return "SeeScreen";

                case atenaGrpc.ServiceId.DefaultOkReturn:
                    return "DefaultOkReturn";

                case atenaGrpc.ServiceId.ListenToDesktopAudio:
                    return "ListenToDesktopAudio";

                case atenaGrpc.ServiceId.DefaultBadReturn:
                    return "DefaultBadReturn";

                default:
                    return "Not a service";
            }
        }

        public static string GetRandomData()
        {
            return "Osama bin laden my bro";
        }

        private void ConnectToMainService()
        {
            if (_client != null) return;

            Config? configInst = Config.Instance;
            Config.ConfigData confData = configInst.Data;

            string serverUrl = "";

            if (confData.tls)
            {
                Log.Err("Cannot use tls, not implemented yet");
            }

            serverUrl += "http://" + confData.serverAddr + ":" + confData.port;

            Log.Info("Connecting to {0}", serverUrl);

            _channel = GrpcChannel.ForAddress(serverUrl);

            _client = new AtenServices.AtenServicesClient(_channel);
        }

        private bool StartMainService()
        {
            Config? configInst = Config.Instance;

            if (configInst == null)
            {
                Log.Warn("Config instance is null. Please ensure it is initialized before starting services.");
                return false;
            }

            try
            {
                if (configInst.Data.pythonPath == null || configInst.Data.mainService == null)
                {
                    Log.Err("Python path or main service command is not set in the configuration.");
                    return false;
                }

                // currently start main_service from one dir above the file which causing the imports in python to fail.
                ProcessStartInfo mainServiceProcess = new ProcessStartInfo
                {
                    FileName = configInst.Data.pythonPath,
                    Arguments = configInst.Data.mainService,
                    UseShellExecute = false,
                };

                Process? res = Process.Start(mainServiceProcess);

                if (res != null) return true;
                  
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Log.Err("No file specified: {0}", ex.Message);
                return false;
            }
            catch (Win32Exception ex)
            {
                Log.Err("Win32 Error starting main service: {0}", ex.Message);
                return false;
            }
        }

        public Cmd CreateCmd(atenaGrpc.ServiceId serviceId, string serviceName, byte[]? data)
        {
            Cmd serviceCmd = new Cmd();
            serviceCmd.ServiceId = serviceId;
            serviceCmd.ServiceName = serviceName;
            serviceCmd.Data = Google.Protobuf.ByteString.CopyFrom(data);

            return serviceCmd;
        }

        //private atenaGrpc.CmdResponse? StartListenToDesktopAudioService(ListenDeskAudio service)
        //{

        //    atenaGrpc.CmdResponse? res = _client?.ExeService(CreateCmd(service._serviceId,
        //        service._serviceName,
        //        service._data));

        //    if(res == null)
        //    {
        //        Log.Err("No CmdResponse recieved for {0}", service._serviceName);
        //        return null;
        //    }

        //    return res; 
        //}

        //private atenaGrpc.CmdResponse? StartPingService(Ping service)
        //{

        //    atenaGrpc.CmdResponse? res = _client?.ExeService(CreateCmd(service._serviceId,
        //        service._serviceName,
        //        service._data));

        //    if (res == null)
        //    {
        //        Log.Err("No CmdResponse recieved for {0}", service._serviceName);
        //        return null;
        //    }

        //    return res;
        //}

        /// <summary>
        /// Do not use this. this will not fire the event on Response return use <see cref="StartService(Service)"/>
        /// that will fire the correct handler automatically that handles the return data.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>

        public atenaGrpc.CmdResponse? SendService(Service service)
        {
            atenaGrpc.CmdResponse? res = _client?.ExeService(
                    CreateCmd(service.GetServiceId(), service.GetServiceName(), service.GetData())
                );


            if (res == null)
            {
                Log.Err("No CmdResponse recieved for {0}", service.GetServiceName());
                return null;
            }

            return res;
        }

        public byte[]? handleRes(atenaGrpc.CmdResponse? cmdRes)
        {
            atenaGrpc.Status res = cmdRes?.Status ?? 0;

            if (res == Status.Fail) return null;

            return cmdRes?.Data.ToArray();
        }

        /// <summary>
        /// Non Blocking, Start RPCs, returned data will be called using <see cref="AtenaEvent.OnRecvGrpcResonse"/>
        /// if you want to recv incoming rpc data then add a listen too <see cref="AtenaEvent.OnRecvGrpcResonse"/>
        /// else, the class <see cref="ServiceResponseHandler"/> handles that for you just make sure you create one in main.
        /// </summary>
        /// <param name="service"></param>
        public void DispatchService(Service service)
        {
            if(service == null)
            {
                Log.Err("Service {} object is null", service);
                return;
            }

            Task.Run(() => StartService(service));
        }

        /// <summary>
        /// Blocking, if you want Non-Blocking use <see cref="DispatchService"/>
        /// </summary>
        /// <param name="service"></param>
        public void StartService(Service service)
        {
            // notify that we are going to start a service, bismillah.
            AtenaEvent.instance?.FireOnServiceStarted(service.GetServiceId());

            atenaGrpc.CmdResponse? response = SendService(service);

            byte[]? data = handleRes(response);

            if(data == null)
            {
                Log.Err("Service {0} return with status FAIL", service.GetServiceId());
                return;
            }

            // notify that the service has returned
            AtenaEvent.instance?.FireOnServiceStopped(service.GetServiceId());
            AtenaEvent.instance?.FireOnRecvGrpcResponseEvent(data);
        }
    }
}
