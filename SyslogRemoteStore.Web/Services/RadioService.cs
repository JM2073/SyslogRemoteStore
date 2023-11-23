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
    private Socket? _asyncSockettcp;
    private Socket? _asyncSocketudp;
    private EndPoint _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

    public RadioService(ConfigurationStore configStore, CollectionStore collectionStore)
    {
        _configStore = configStore;
        _collectionStore = collectionStore;
        Instance = this;
    }


    public static RadioService Instance { get; set; }


    public async Task BeginListeningAsync()
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


    public void ChangeProtocoleType()
    {
        if (_configStore.ListeningProtocolType == ProtocolType.Tcp)
        {
            foreach (T6S3 radio in _collectionStore.Radios.Where(x =>
                         x.Socket.ProtocolType == System.Net.Sockets.ProtocolType.Udp))
            {
                radio.Socket.Dispose();
                radio.UdpConnected = false;
            }
            _asyncSocketudp.Dispose();
            _asyncSocketudp = null;
        }

        else if (_configStore.ListeningProtocolType == ProtocolType.Udp)
        {
            foreach (T6S3 radio in _collectionStore.Radios.Where(x =>
                         x.Socket.ProtocolType == System.Net.Sockets.ProtocolType.Tcp))
            {
                radio.Socket.Dispose();
                radio.TcpConnected = false;
            }
            _asyncSockettcp.Dispose();
        }

        BeginListeningAsync();
    }

    public void CloseAllOpenConnections()
    {
        foreach (T6S3 _radio in _collectionStore.Radios)
        {
            _radio.Socket.Dispose();
            _radio.TcpConnected = false;
            _radio.UdpConnected = false;
        }

        _asyncSocketudp.Dispose();
        _asyncSocketudp = null;    
        _asyncSockettcp.Dispose();
        _asyncSockettcp = null;


        
        BeginListeningAsync();
    }
    
    private async Task ListenUdp(string ip, int port)
    {
        try
        {
            if (_configStore.ListeningProtocolType is ProtocolType.Udp or ProtocolType.Both)
            {
                if (_asyncSocketudp?.IsBound is null or false)
                {
                    _asyncSocketudp = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram,
                        System.Net.Sockets.ProtocolType.Udp);

                    _asyncSocketudp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _asyncSocketudp.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                    _asyncSocketudp.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                }

                byte[] buffer = new byte[250];
                _asyncSocketudp.BeginReceiveFrom(buffer, 0, buffer.Length, 0, ref _remoteEndPoint,
                    ar => HandleMessageCallback(ar, _asyncSocketudp), buffer);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void HandleMessageCallback(IAsyncResult ar, Socket asyncSocket)
    {
        try
        {
            if (_configStore.ListeningProtocolType is ProtocolType.Udp or ProtocolType.Both && _asyncSocketudp.IsBound)
            {
                if (_asyncSocketudp == null)
                {
                    return;
                }
                
                int bytesRead = _asyncSocketudp.EndReceiveFrom(ar, ref _remoteEndPoint);

                string senderIpAddress = (_remoteEndPoint as IPEndPoint)?.Address.ToString() ?? string.Empty;
                int senderPort = (_remoteEndPoint as IPEndPoint)?.Port ?? 0;

                T6S3? _t6S3 =
                    _collectionStore.Radios.SingleOrDefault(x => x.Ip == senderIpAddress && x.Port == senderPort);

                if (_t6S3 is null)
                {
                    _t6S3 = new T6S3(asyncSocket, senderIpAddress, senderPort);
                    _collectionStore.Radios.Add(_t6S3);
                }

                byte[] buffer = (byte[])ar.AsyncState;
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                _t6S3.Logs.Add(new Log(message, senderIpAddress, senderPort));

                buffer = new byte[1024];
                _asyncSocketudp.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref _remoteEndPoint,
                    ar1 => HandleMessageCallback(ar1, asyncSocket), buffer);
            }
            else
            {
                Console.WriteLine($"{DateTime.Now} UDP is no longer receiving, rejected connection.");
            }
        }
        catch (SocketException se)
        {
            Console.WriteLine($"{DateTime.Now} Socket error: " + se.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now} Receive error: " + ex.Message);
        }
    }

    private async Task ListenTcp(string ip, int port)
    {
        try
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
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void OnClientConnect(IAsyncResult _async)
    {
        try
        {
            if (_configStore.ListeningProtocolType is ProtocolType.Tcp or ProtocolType.Both)
            {
                if (_asyncSockettcp == null)
                {
                    return;
                }
                Socket clientSocket = _asyncSockettcp.EndAccept(_async);
                IPEndPoint remoteEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;

                T6S3 t6S3 = new(clientSocket, remoteEndPoint.Address.ToString(), remoteEndPoint.Port);
                _collectionStore.Radios.Add(t6S3);

                byte[] rxBuffer = new byte[250];
                t6S3.Socket.BeginReceive(rxBuffer, 0, rxBuffer.Length, 0, ar => DequeueRequests(ar, t6S3), rxBuffer);
                
                if (_asyncSockettcp == null)
                {
                    return;
                }
                _asyncSockettcp.BeginAccept(OnClientConnect, null);
            }
            else
            {
                Console.WriteLine($"{DateTime.Now} TCP is no longer receiving, rejected connection.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void DequeueRequests(IAsyncResult _async, T6S3 t6S3)
    {
        try
        {
            if (t6S3.Socket.Connected && t6S3.TcpConnected)
            {
                if (_configStore.ListeningProtocolType is ProtocolType.Tcp or ProtocolType.Both)
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
                    Console.WriteLine($"{DateTime.Now} Not taking TCP connections.{t6S3.Id} has been Disconnected");
                    t6S3.TcpConnected = false;
                }
            }
        }
        catch (SocketException se)
        {
            Console.WriteLine($"{DateTime.Now} Socket error: " + se.Message);
            t6S3.TcpConnected = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.Now} Receive error: " + ex.Message);
        }
    }


    public void Dispose()
    {
        _asyncSockettcp.Dispose();
        _asyncSocketudp.Dispose();
    }
}