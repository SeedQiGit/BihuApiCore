using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace BihuApiCore.Infrastructure.Helper
{
    public class LogHelper
    {
        private static readonly ILogger Logger;
        private static readonly ILoggerFactory LoggerFactory;

        public static ILoggerFactory LoggerFactorySingleton
        {
            get { return LoggerFactory; }
        }


        static LogHelper()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            //这里会输出到Nlog
            loggerFactory.AddNLog();
            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(nameof(LogHelper));
        }

        public static void Info(string msg)
        {
            Logger.LogInformation(msg);
        }

        public static void Error(string msg)
        {
            Logger.LogError(msg);
        }

        public static void Warning(string msg)
        {
            Logger.LogWarning(msg);
        }
        public static void Trace(string msg)
        {
            Logger.LogTrace(msg);
        }

        public static void Error(Exception ex)
        {
            Error("发生异常:" + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
        }
       
        public static void Error(string msg,Exception ex)
        {
            Error(msg + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
        }
    }
}
