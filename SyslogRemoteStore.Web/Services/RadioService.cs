using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SyslogRemoteStore.Web.Enums;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.Data;

public class RadioService
{
    private ConfigurationStore _configStore;
    private CollectionStore _collectionStore;
    private Socket _asyncSocket = null;
    private EndPoint _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

    public RadioService(ConfigurationStore configStore, CollectionStore collectionStore)
    {
        _configStore = configStore;
        _collectionStore = collectionStore;
    }


    public void BeginListening()
    {
        
        ListenTcp(_configStore.Ip,_configStore.Port);
        ListenUdp(_configStore.Ip,_configStore.Port);
    }

    public void ListenUdp(string ip, int port)
    {
        _asyncSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        _asyncSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
        
        byte[] buffer = new byte[250];
        _asyncSocket.BeginReceiveFrom(buffer,0,buffer.Length,0,ref _remoteEndPoint, HandleMessageCallback,buffer);
    }

    private void HandleMessageCallback(IAsyncResult ar)
    {
        try
        {
            int bytesRead = _asyncSocket.EndReceiveFrom(ar, ref _remoteEndPoint);

            byte[] buffer = (byte[])ar.AsyncState;
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            string senderIpAddress = (_remoteEndPoint as IPEndPoint)?.Address.ToString();
            int senderPort = (_remoteEndPoint as IPEndPoint)?.Port ?? 0;

            Console.WriteLine($"Received from {senderIpAddress}:{senderPort}: {message}");

            // Start the next asynchronous receive operation
            buffer = new byte[1024];
            _asyncSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref _remoteEndPoint, HandleMessageCallback, buffer);
        }
        catch (SocketException se)
        {
            Console.WriteLine("Socket error: " + se.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Receive error: " + ex.Message);
        }
    }

    public void ListenTcp(string ip,int port)
    {
        _asyncSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        _asyncSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _asyncSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        _asyncSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
        
        _asyncSocket.Listen((int)SocketOptionName.MaxConnections);

        _asyncSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);

    }

    private void OnClientConnect(IAsyncResult _async)
    {
        Socket clientSocket = _asyncSocket.EndAccept(_async);

        T6S3 t6S3 = new T6S3(clientSocket);
        
        _collectionStore.Radios.Add(t6S3);

        byte[] rxBuffer = new byte[250];
        t6S3.Socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, 0, new AsyncCallback(ar => DequeueRequests(ar, t6S3)), rxBuffer);

        _asyncSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);
    }

    private void DequeueRequests(IAsyncResult _async, T6S3 t6S3)
    {
        int bytesRead = t6S3.Socket.EndReceive(_async);

        byte[] rxBuffer = (byte[])_async.AsyncState;
        string request = Encoding.ASCII.GetString(rxBuffer, 0, bytesRead);

        t6S3.Logs.Add(new Log { Message = request, SeverityLevel = SeverityLevel.Info });

        string message = string.Empty;
        message += "---------------Socket----------";
        foreach (Log log in t6S3.Logs)
        {
            message += log.Message;
        }
        message += "------------END-------------";
        Console.WriteLine(message);
        
        t6S3.Socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, 0, new AsyncCallback(async => DequeueRequests(async,t6S3)), rxBuffer);
    }
    
    
}