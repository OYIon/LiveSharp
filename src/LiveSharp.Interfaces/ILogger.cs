using System;
namespace LiveSharp
{
    public interface ILogger
    {
        void LogError(string errorMessage);
        void LogError(string errorMessage, Exception e);
        void LogMessage(string message);
        void LogWarning(string warning);
    }
}
