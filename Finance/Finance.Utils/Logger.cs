using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Finance.Utils
{
    public class Logger
    {
        static ILogger logger= null;
        public static ILogger GetLogger(Type t)
        {
            if (logger == null)
                logger = new DefaultLogger(t);

            return logger;
        }

    }

    public interface ILogger
    {
        void Error(Exception ex, string traceId = "");
        void Error(string message, params object[] args);
        void Fatal(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Info(string message, params object[] args);
        void Debug(string message, params object[] args);
    }

    public class DefaultLogger : ILogger
    {
        log4net.ILog log = null;
        public DefaultLogger(Type t)
        {
            //配置文件
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Config/log4net.config");
            //加载配置
            XmlConfigurator.ConfigureAndWatch(logCfg);
            if (t == null)
                log = log4net.LogManager.GetLogger("");
            else
                log = log4net.LogManager.GetLogger(t);
        }

        public void Debug(string message, params object[] args)
        {
            if (args.Length > 0)
                log.Debug(string.Format(message, args));
            else
                log.Debug(message);
        }

        public void Error(string message, params object[] args)
        {
            if (args.Length > 0)
                log.Error(string.Format(message, args));
            else
                log.Error(message);
        }

        public void Fatal(string message, params object[] args)
        {
            if (args.Length > 0)
                log.Fatal(string.Format(message, args));
            else
                log.Fatal(message);
        }

        public void Info(string message, params object[] args)
        {
            if (args.Length > 0)
                log.Info(string.Format(message, args));
            else
                log.Info(message);
        }

        public void Warn(string message, params object[] args)
        {
            if (args.Length > 0)
                log.Warn(string.Format(message, args));
            else
                log.Warn(message);
        }
        public void Error(Exception ex,string traceId ="")
        {
            if(traceId!="")
                log.Error("TraceId:" + traceId);
            log.Error(ex);            
        }
    }
}
