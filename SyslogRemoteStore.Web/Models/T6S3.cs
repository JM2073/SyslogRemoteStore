using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SyslogRemoteStore.Web.Models;

public class T6S3 : INotifyPropertyChanged
{
    private readonly Timer logTimer;

    private bool _udpConnected;

    private bool _tcpConnected;

    private bool _isHidden;
    
    private bool _pendingDisconnect;

    private string _ipvType;

    private ObservableCollection<Log> _logs = new();


    public T6S3(Socket socket, string ip, int port)
    {
        Id = Guid.NewGuid();
        Socket = socket;
        Ip = ip;
        Port = port;
        TcpConnected = socket.ProtocolType == ProtocolType.Tcp;
        PendingDisconnect = false;
        UdpConnected = socket.ProtocolType == ProtocolType.Udp;
        IsHidden = false;
        _logs.CollectionChanged += Logs_CollectionChanged;

        // Initialize and configure the timer
        logTimer = new Timer();
        logTimer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
        logTimer.Elapsed += LogTimerElapsed;
        logTimer.AutoReset = false;
    }

    public bool IsHidden
    {
        get => _isHidden;
        set => SetField(ref _isHidden, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool TcpConnected
    {
        get => _tcpConnected;
        set => SetField(ref _tcpConnected, value);
    }

    public bool PendingDisconnect
    {
        get => _pendingDisconnect;
        set => SetField(ref _pendingDisconnect, value);
    }

    public bool UdpConnected
    {
        get => _udpConnected;
        set => SetField(ref _udpConnected, value);
    }

    public Guid Id { get; set; }

    public string Ip { get; set; }

    public int Port { get; set; }

    public Socket Socket { get; set; }

    public ObservableCollection<Log> Logs
    {
        get => _logs;
        set => SetField(ref _logs, value);
    }

    public string GetSocketStatus()
    {
        string result = this.Socket.ProtocolType switch
        {
            ProtocolType.Udp => this.UdpConnected ? "Connected" : "Disconnected",
            ProtocolType.Tcp => this.PendingDisconnect ? "PendingDisconnect" : this.TcpConnected ? "Connected" : "Disconnected",
            _ => "Unknown"
        };
        return result;
    }
    
    public string GetIpvType()
    {
        return IPAddress.Parse(this.Ip).IsIPv4MappedToIPv6 ? "IPv4" : "IPv6";
    }

    public string GetFormatedIp()
    {
        IPAddress ip = IPAddress.Parse(this.Ip);
        return IPAddress.Parse(this.Ip).IsIPv4MappedToIPv6 ? ip.MapToIPv4().ToString() : ip.MapToIPv6().ToString();
    }
    
    // Event handler for changes in the Logs collection
    private void Logs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Logs));
        ResetLogTimer(); // Reset the timer when a log is added
    }

    // Event handler for the log timer elapsed
    private void LogTimerElapsed(object sender, ElapsedEventArgs e)
    {
        switch (Socket.ProtocolType)
        {
            case ProtocolType.Udp:
                this.UdpConnected = false;
                break;
            case ProtocolType.Tcp:
                this.TcpConnected = false;
                break;
        }
    }

    // Reset the log timer
    private void ResetLogTimer()
    {
        logTimer.Stop();
        logTimer.Start();
        
        switch (Socket.ProtocolType)
        {
            case ProtocolType.Udp:
                this.UdpConnected = true;
                break;
            case ProtocolType.Tcp:
                this.TcpConnected = true;
                break;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}