using System.Net;
using System.Net.Sockets;
using System.Text;
using SyslogRemoteStore.Web.Enums;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;
using ProtocolType = System.Net.Sockets.ProtocolType;

namespace SyslogRemoteStore.Web.Data;

public class RadioService
{
    private Socket _asyncSockettcp;
    private Socket _asyncSocketudp;
    private readonly CollectionStore _collectionStore;
    private readonly ConfigurationStore _configStore;
    private EndPoint _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

    public RadioService(ConfigurationStore configStore, CollectionStore collectionStore)
    {
        _configStore = configStore;
        _collectionStore = collectionStore;
    }


    public void BeginListening()
    {
        ListenUdp(_configStore.Ip, _configStore.Port);
        ListenTcp(_configStore.Ip, _configStore.Port);
    }

    public async Task ListenUdp(string ip, int port)
    {
        _asyncSocketudp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        _asyncSocketudp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        _asyncSocketudp.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

        byte[] buffer = new byte[250];
        _asyncSocketudp.BeginReceiveFrom(buffer, 0, buffer.Length, 0, ref _remoteEndPoint,
            ar => HandleMessageCallback(ar, _asyncSocketudp), buffer);
    }

    private void HandleMessageCallback(IAsyncResult ar, Socket asyncSocket)
    {
        try
        {
            string senderIpAddress = (_remoteEndPoint as IPEndPoint)?.Address.ToString();
            int senderPort = (_remoteEndPoint as IPEndPoint)?.Port ?? 0;

            T6S3? _t6S3 = _collectionStore.Radios.SingleOrDefault(x => x.Ip == senderIpAddress && x.Port == senderPort);

            if (_t6S3 is null)
            {
                _t6S3 = new T6S3(asyncSocket);
                _t6S3.Ip = senderIpAddress;
                _t6S3.Port = senderPort;
                _collectionStore.Radios.Add(_t6S3);
            }

            int bytesRead = _asyncSocketudp.EndReceiveFrom(ar, ref _remoteEndPoint);
            byte[] buffer = (byte[])ar.AsyncState;
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            Log log = new Log { Message = message , SourceIp = $"{senderIpAddress}:{senderPort}"};
            log.ParseMessage();
            _t6S3.Logs.Add(log);
            
            // Start the next asynchronous receive operation
            buffer = new byte[1024];
            _asyncSocketudp.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref _remoteEndPoint,
                ar1 => HandleMessageCallback(ar1, asyncSocket), buffer);
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

    public async Task ListenTcp(string ip, int port)
    {
        _asyncSockettcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        _asyncSockettcp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _asyncSockettcp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        _asyncSockettcp.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

        _asyncSockettcp.Listen((int)SocketOptionName.MaxConnections);

        _asyncSockettcp.BeginAccept(OnClientConnect, null);
    }

    private void OnClientConnect(IAsyncResult _async)
    {
        Socket clientSocket = _asyncSockettcp.EndAccept(_async);

        T6S3 t6S3 = new(clientSocket);
        _collectionStore.Radios.Add(t6S3);

        byte[] rxBuffer = new byte[250];
        t6S3.Socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, 0, ar => DequeueRequests(ar, t6S3), rxBuffer);

        _asyncSockettcp.BeginAccept(OnClientConnect, null);
    }

    private void DequeueRequests(IAsyncResult _async, T6S3 t6S3)
    {
        try
        {
            int bytesRead = t6S3.Socket.EndReceive(_async);

            byte[] rxBuffer = (byte[])_async.AsyncState;
            string request = Encoding.ASCII.GetString(rxBuffer, 0, bytesRead);

            t6S3.Logs.Add(new Log { Message = request });

            t6S3.Socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, 0, async => DequeueRequests(async, t6S3), rxBuffer);
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

}