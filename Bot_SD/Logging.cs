using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ants
{
    public static class Logger
    {
        public static string filePath = string.Format("{0}{1}text-{2:yyyy-MM-dd_hh-mm-ss-tt}.txt",
                AppDomain.CurrentDomain.BaseDirectory,
               Path.DirectorySeparatorChar,
               DateTime.Now);

        public static NLog.Logger Log;

        static Logger()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = filePath };
            //var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            //config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;


            Log = LogManager.GetCurrentClassLogger();
        }


    }



}
