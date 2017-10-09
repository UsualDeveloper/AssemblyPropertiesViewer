using NLog;
using System;
using System.Collections.Concurrent;
using AssemblyPropertiesViewerLogging = AssemblyPropertiesViewer.Core.Interfaces;

namespace AssemblyPropertiesViewer.Core.Logger
{
    public class BasicLogger : AssemblyPropertiesViewerLogging.ILogger
    {
        private static readonly Type DefaultLoggerAssociatedType = typeof(BasicLogger);

        private static ConcurrentDictionary<Type, NLog.Logger> loggers = new ConcurrentDictionary<Type, NLog.Logger>();
        
        private NLog.Logger logger = null;

        private bool isInitialized = false;
        
        public BasicLogger()
        {
            
        }

        public void InitializeLogger(Type type = null)
        {
            if (isInitialized)
                throw new InvalidOperationException("Logger was already initialized.");

            SetLoggerType(type ?? DefaultLoggerAssociatedType);

            isInitialized = true;
        }

        public void Debug(string message)
        {
            EnsureLoggerInitialized();

            this.logger.Debug(message);
        }

        public void Info(string message)
        {
            EnsureLoggerInitialized();

            this.logger.Info(message);
        }

        public void Warning(string message)
        {
            EnsureLoggerInitialized();

            this.logger.Warn(message);
        }

        public void Error(string message)
        {
            EnsureLoggerInitialized();

            this.logger.Error(message);
        }

        public void Error(Exception ex, string message)
        {
            EnsureLoggerInitialized();

            this.logger.Error(ex, message);
        }

        private void EnsureLoggerInitialized()
        {
            if (!isInitialized)
            {
                InitializeLogger();
            }
        }

        private void SetLoggerType(Type loggerAssociatedType)
        {
            if (!loggers.TryGetValue(loggerAssociatedType, out logger))
            {
                logger = LogManager.GetLogger(loggerAssociatedType.FullName);

                if (!loggers.TryAdd(loggerAssociatedType, logger))
                {
                    //TODO: handle scenario when the logger cannot be added to the dictionary
                }
            }
        }
    }
}
