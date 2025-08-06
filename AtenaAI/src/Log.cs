using Serilog;
using Serilog.Sinks;

namespace atena
{
    class Log
    {
        public static void InitLogger()
        {
#if DEBUG
            Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
#else
            Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
#endif
        }
        public static void Info(string msgTemplate, params object?[]? propVal)
        {
            Serilog.Log.Information(msgTemplate, propVal); 
        }
        public static void Warn(string msgTemplate, params object?[]? propVal)
        {
            Serilog.Log.Warning(msgTemplate, propVal);
        }
        public static void Err(string msgTemplate, params object?[]? propVal)
        {
            Serilog.Log.Error(msgTemplate, propVal);
        }

        public static void Info(string msgTemplate)
        {
            Serilog.Log.Information(msgTemplate);
        }
        public static void Warn(string msgTemplate)
        {
            Serilog.Log.Warning(msgTemplate);
        }
        public static void Err(string msgTemplate)
        {
            Serilog.Log.Error(msgTemplate);
        }
    }
}
