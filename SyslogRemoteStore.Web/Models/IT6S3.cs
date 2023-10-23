using System.Net.Sockets;

namespace SyslogRemoteStore.Web.Models;

public interface IT6S3
{
    string Ip { get; }
    int Port { get; }
    
    Socket Socket { get; }
    List<Log> Logs { get; }

    /// <summary>
    /// can be used to connect or reconnect with the socket.
    /// </summary>
    /// <returns>if the connection was successfull</returns>
    bool Connect();
}