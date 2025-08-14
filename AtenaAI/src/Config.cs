using Newtonsoft.Json;
using System;

namespace atena
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;

    public class Config
    {
        private static Config? _instance = null;
        public static Config Instance => _instance ??= new Config();

        public static string Version { get; } = "1.1.0";
        public static string Name { get; } = "Atena";

        public ConfigData Data { get; private set; }

        private Config()
        {
            Data = new ConfigData();
        }

        public static void LoadFromFile(string pathToFile)
        {
            if (_instance != null)
            {
                Log.Warn("Config already initialized. Ignoring reload.");
                return;
            }

            var config = new Config();
            var storedConfig = config.readConfig(pathToFile);

            if (storedConfig != null)
            {
                config.Data = storedConfig;
            }

            _instance = config;
        }

        private ConfigData? readConfig(string pathToFile)
        {
            try
            {
                string json = File.ReadAllText(pathToFile);
                if (string.IsNullOrEmpty(json)) return null;

                return JsonConvert.DeserializeObject<ConfigData>(json);
            }
            catch (JsonException)
            {
                Log.Err("Error reading configuration file. Please check the format.");
                return null;
            }
            catch (FileNotFoundException)
            {
                Log.Err("Configuration file not found. Please ensure the path is correct.");
                return null;
            }
        }

        public string GetServiceFilePathFromName(string name)
        {
            if (Data.servicesFilePathWithName == null) return "";

            foreach(Services.ServiceFilePath? sfp in Data.servicesFilePathWithName)
            {
                if(sfp.serviceName == name)
                {
                    if (sfp.serviceFilePath == null) return "";

                    return sfp.serviceFilePath;
                }
            }

            return "";
        }

        public class ConfigData
        {

            public int screenWidth { get; set; }
            public int screenHeight { get; set; }
            public int mainWindowWidth { get; set; }
            public int mainWindowHeight { get; set; }
            public int mainWindowPosX { get; set; }
            public int mainWindowPosY { get; set; }
            public int chatWindowWidth { get; set; }
            public int chatWindowHeight { get; set; }
            public int chatWindowPosX { get; set; }
            public int chatWindowPosY { get; set; }
            public string userTextColor { get; set; } = "#505050";
            public string atenaTextColor { get; set; } = "#ffffff";
            public bool listenDesktopAudio { get; set; } = false;
            public bool seeScreen { get; set; } = false;
            public string pythonPath { get; set; } = "null";
            public string mainService { get; set; } = "null";
            public bool tls { get; set; } = false;
            public string serverAddr { get; set; } = "null";
            public string port { get; set; } = "null";
            public string tcpPort { get; set; } = "null";
            public string chatWindowBackgroundColor { get; set; } = "Transparent";
            public int notificationBorderWidth { get; set; }
	        public int notificationBorderHeight {get; set;} 
	        public string notificationBorderBackground {get; set;} = "null";
	        public int notificatonBorderCornerRadius {get; set;} 
	        public string notificationStackPanelOrientation {get; set;} = "null";
	        public string notificationStackPanelVertialAlignment {get; set;} = "null";
	        public int notificationTextBlockPadding { get; set; }
	        public string notificationTextBlockVerticialAlignment {get; set;} = "null";
	        public string notificationTextBlockForeground {get; set;} = "null";
	        public string notificationTextBlockText {get; set;} = "null";
	        public int notificationTextBlockBorderWidth {get; set;} 
	        public int notificationTextBlockBorderHeight {get; set;}
	        public string notificationTextBlockBorderBackground {get; set;} = "null";
	        public int notificationTextBlockBorderCornerRadius {get; set;} 
            public List<Services.ServiceFilePath>? servicesFilePathWithName { get; set; }
        }
    }

}
