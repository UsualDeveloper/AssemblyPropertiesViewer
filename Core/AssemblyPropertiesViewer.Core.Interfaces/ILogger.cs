using System;

namespace AssemblyPropertiesViewer.Core.Interfaces
{
    public interface ILogger
    {
        void Debug(string message);

        void Info(string message);

        void Warning(string message);

        void Error(string message);

        void Error(Exception ex, string message);
    }
}
