using System;
using System.Threading.Tasks;

namespace LiveSharp.Interfaces
{
    public interface ILiveSharpTransport
    {
        object ConnectionObject { get; }

        Task Connect(string host, int port, Action<Exception, object> onTransportException);
        void Send(byte[] buffer, Action onComplete);
        void StartReceiving(Action<object, byte[], int> onBufferReceived);
        void CloseConnection();
    }
}