using Newtonsoft.Json;
using System;

namespace atena
{
    using Newtonsoft.Json;
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

        public class ConfigData
        {
            public bool ListenDesktopAudio { get; set; } = false;
            public bool SeeScreen { get; set; } = false;
            public string pythonPath { get; set; } = "null";
            public string mainService { get; set; } = "null";
            public bool tls { get; set; } = false;
            public string serverAddr { get; set; } = "null";
            public string port { get; set; } = "null";

            public string serviceFilePath { get; set; } = "null";
        }
    }

}
