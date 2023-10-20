using System.Net.Sockets;

namespace SyslogRemoteStore.Web.Models;

public class T6S3 : IT6S3
{
    public string Ip { get; set; }
    public int Port { get; set; }

    public Socket Socket { get; set; }
    public List<Log> Logs { get; set; }
    
    public bool Connect()
    {
        throw new NotImplementedException();
    }
    
}