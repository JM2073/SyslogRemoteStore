using System.Net.Sockets;

namespace SyslogRemoteStore.Web.Models;

public class T6S3 : IT6S3
{
    public T6S3(Socket socket)
    {
        this.Socket = socket;
    }
    
    public string Ip { get; set; }
    public int Port { get; set; }

    public Socket Socket { get; set; }
    public List<Log> Logs { get; set; } = new List<Log>();
    
    public bool Connect()
    {
        throw new NotImplementedException();
    }
    
}