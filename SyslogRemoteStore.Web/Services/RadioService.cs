using System.Net;
using System.Net.Sockets;
using System.Text;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;
using ProtocolType = SyslogRemoteStore.Web.Enums.ProtocolType;

namespace SyslogRemoteStore.Web.Data;

public class RadioService
{
    private readonly CollectionStore _collectionStore;
    private readonly ConfigurationStore _configStore;
    private Socket _asyncSockettcp;
    private Socket _asyncSocketudp;
    private EndPoint _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

    public RadioService(ConfigurationStore configStore, CollectionStore collectionStore)
    {
        _configStore = configStore;
        _collectionStore = collectionStore;
        Instance = this;
    }


    public static RadioService Instance { get; set; }


    public void BeginListening()
    {
        switch (_configStore.ListeningProtocolType)
        {
            case ProtocolType.Udp:
                ListenUdp(_configStore.Ip, _configStore.Port);
                break;
            case ProtocolType.Tcp:
                ListenTcp(_configStore.Ip, _configStore.Port);
                break;
            case ProtocolType.Both:
                ListenTcp(_configStore.Ip, _configStore.Port);
                ListenUdp(_configStore.Ip, _configStore.Port);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public void CloseOpenConnections()
    {
        if (_configStore.ListeningProtocolType == ProtocolType.Tcp)
            foreach (T6S3 radio in _collectionStore.Radios.Where(x =>
                         x.Socket.ProtocolType == System.Net.Sockets.ProtocolType.Udp))
            {
                radio.Socket.Close();
                radio.UdpConnected = false;
            }
        else if (_configStore.ListeningProtocolType == ProtocolType.Udp)
            foreach (T6S3 radio in _collectionStore.Radios.Where(x =>
                         x.Socket.ProtocolType == System.Net.Sockets.ProtocolType.Tcp))
            {
                radio.Socket.Close();
                radio.TcpConnected = false;
            }

        BeginListening();
    }


    public async Task ListenUdp(string ip, int port)
    {
        try
        {
            while (_configStore.ListeningProtocolType is ProtocolType.Both or ProtocolType.Udp)
            {
                _asyncSocketudp = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram,
                    System.Net.Sockets.ProtocolType.Udp);

                _asyncSocketudp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _asyncSocketudp.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                _asyncSocketudp.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

                byte[] buffer = new byte[250];
                _asyncSocketudp.BeginReceiveFrom(buffer, 0, buffer.Length, 0, ref _remoteEndPoint,
                    ar => HandleMessageCallback(ar, _asyncSocketudp), buffer);
            }

            Console.WriteLine("Exit UDP while loop.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void HandleMessageCallback(IAsyncResult ar, Socket asyncSocket)
    {
        try
        {
            while (_configStore.ListeningProtocolType is ProtocolType.Both or ProtocolType.Udp)
            {
                int bytesRead = _asyncSocketudp.EndReceiveFrom(ar, ref _remoteEndPoint);

                string senderIpAddress = (_remoteEndPoint as IPEndPoint)?.Address.ToString() ?? string.Empty;
                int senderPort = (_remoteEndPoint as IPEndPoint)?.Port ?? 0;

                T6S3? _t6S3 =
                    _collectionStore.Radios.SingleOrDefault(x => x.Ip == senderIpAddress && x.Port == senderPort);

                if (_t6S3 is null)
                {
                    if (_configStore.ListeningProtocolType == ProtocolType.Tcp)
                        return;

                    _t6S3 = new T6S3(asyncSocket, senderIpAddress, senderPort);
                    _collectionStore.Radios.Add(_t6S3);
                }

                byte[] buffer = (byte[])ar.AsyncState;
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                _t6S3.Logs.Add(new Log(message, senderIpAddress, senderPort));

                // Start the next asynchronous receive operation
                buffer = new byte[1024];
                _asyncSocketudp.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref _remoteEndPoint,
                    ar1 => HandleMessageCallback(ar1, asyncSocket), buffer);
            }
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
        try
        {
            while (_configStore.ListeningProtocolType is ProtocolType.Both or ProtocolType.Tcp)
            {
                _asyncSockettcp = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream,
                    System.Net.Sockets.ProtocolType.Tcp);

                _asyncSockettcp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                _asyncSockettcp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _asyncSockettcp.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                _asyncSockettcp.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

                _asyncSockettcp.Listen((int)SocketOptionName.MaxConnections);

                _asyncSockettcp.BeginAccept(OnClientConnect, null);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void OnClientConnect(IAsyncResult _async)
    {
        try
        {
            while (_configStore.ListeningProtocolType is ProtocolType.Both or ProtocolType.Tcp)
            {
                Socket clientSocket = _asyncSockettcp.EndAccept(_async);
                IPEndPoint remoteEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;

                T6S3 t6S3 = new(clientSocket, remoteEndPoint.Address.ToString(), remoteEndPoint.Port);
                _collectionStore.Radios.Add(t6S3);

                byte[] rxBuffer = new byte[250];
                t6S3.Socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, 0, ar => DequeueRequests(ar, t6S3), rxBuffer);

                _asyncSockettcp.BeginAccept(OnClientConnect, null);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void DequeueRequests(IAsyncResult _async, T6S3 t6S3)
    {
        try
        {
            while (_configStore.ListeningProtocolType is ProtocolType.Both or ProtocolType.Tcp)
            {
                if (t6S3.Socket.Connected)
                {
                    int bytesRead = t6S3.Socket.EndReceive(_async);
                    byte[] rxBuffer = (byte[])_async.AsyncState;
                    string request = Encoding.ASCII.GetString(rxBuffer, 0, bytesRead);

                    t6S3.Logs.Add(new Log(request, t6S3.Ip, t6S3.Port));

                    t6S3.Socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, 0, async => DequeueRequests(async, t6S3),
                        rxBuffer);
                }
                else
                {
                    t6S3.TcpConnected = false;
                }
            }
        }
        catch (SocketException se)
        {
            Console.WriteLine("Socket error: " + se.Message);
            t6S3.TcpConnected = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Receive error: " + ex.Message);
        }
    }
}