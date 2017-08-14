using NLog;
using System;
using System.Collections.Concurrent;
using AssemblyPropertiesViewerLogging = AssemblyPropertiesViewer.Core.Interfaces;

namespace AssemblyPropertiesViewer.Core.Logger
{
    public class BasicLogger : AssemblyPropertiesViewerLogging.ILogger
    {
        private static ConcurrentDictionary<Type, NLog.Logger> loggers = new ConcurrentDictionary<Type, NLog.Logger>();

        private NLog.Logger logger = null;

        // TODO: improve
        public BasicLogger()
        {
            Type callingClassType = typeof(BasicLogger);

            if (!loggers.TryGetValue(callingClassType, out logger))
            {
                logger = LogManager.GetLogger(callingClassType.FullName);

                if (!loggers.TryAdd(callingClassType, logger))
                {
                    //TODO: handle scenario when the logger cannot be added to the dictionary
                }
            }
        }

        public void Debug(string message)
        {
            this.logger.Debug(message);
        }

        public void Info(string message)
        {
            this.logger.Info(message);
        }

        public void Warning(string message)
        {
            this.logger.Warn(message);
        }

        public void Error(string message)
        {
            this.logger.Error(message);
        }

        public void Error(Exception ex, string message)
        {
            this.logger.Error(ex, message);
        }
    }
}
