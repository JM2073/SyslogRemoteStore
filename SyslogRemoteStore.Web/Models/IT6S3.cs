using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;

namespace SyslogRemoteStore.Web.Models;

public interface IT6S3
{
    event PropertyChangedEventHandler? PropertyChanged;

    bool IsHidden { get; }
    bool TcpConnected { get; }
    bool AlertFlag { get; }
    Guid Id { get; }
    string Ip { get; }
    int Port { get; }
    Socket Socket { get; }
    ObservableCollection<Log> Logs { get; }

    string GetIpvType();
    string GetFormatedIp();
    string GetSocketStatus();
}