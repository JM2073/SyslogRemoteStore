using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace SyslogRemoteStore.Web.Models;

public interface IT6S3
{
    string Ip { get; }
    int Port { get; }
    bool AlertFlag { get; }
    
    Socket Socket { get; }
    ObservableCollection<Log> Logs { get; }

}