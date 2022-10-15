namespace maxwell_dmm
{
    using System;
    using System.IO;
    using NLog;

    /// <summary>
    /// Set the Nlog logger class.
    /// </summary>
    internal static class Logger
    {
        private static string logPath = string.Empty;

        /// <summary>
        /// Sets up the Nlog logger class.
        /// </summary>
        public static void SetLogger()
        {
            // private static readonly NLog.Logger NLogger = NLog.LogManager.GetCurrentClassLogger();
            /* var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: Console
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);

            // Apply config
            NLog.LogManager.Configuration = config;*/
            var config = new NLog.Config.LoggingConfiguration();
            Directory.CreateDirectory("logs");
            string path = "logs/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_log.txt";
            logPath = AppDomain.CurrentDomain.BaseDirectory + "/" + path;
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = path };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole, "Quartz*", true);

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;
        }
    }
}
