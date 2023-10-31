using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SyslogRemoteStore.Web.Models;

public class T6S3 : IT6S3, INotifyPropertyChanged
{
    private readonly Timer logTimer;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public T6S3(Socket socket, string ip, int port)
    {
        Socket = socket;
        Ip = ip;
        Port = port;
        TcpConnected = socket.ProtocolType == ProtocolType.Tcp;
        _logs.CollectionChanged += Logs_CollectionChanged;

        // Initialize and configure the timer
        logTimer = new Timer();
        logTimer.Interval = 5 * 60 * 1000; 
        logTimer.Elapsed += LogTimerElapsed;
        logTimer.AutoReset = false; 
    }

    private bool _tcpConnected;

    public bool TcpConnected
    {
        get => _tcpConnected;
        set => SetField(ref _tcpConnected, value);
    }

    private bool _alertFlag;
    
    public bool AlertFlag
    {
        get => _alertFlag;
        set => SetField(ref _alertFlag, value);
    }

    public string Ip { get; set; }

    public int Port { get; set; }
    
    public Socket Socket { get; set; }

    private ObservableCollection<Log> _logs = new();

    public ObservableCollection<Log> Logs
    {
        get => _logs;
        set => SetField(ref _logs, value);
    }

    // Event handler for changes in the Logs collection
    private void Logs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged("Logs");
        ResetLogTimer(); // Reset the timer when a log is added
    }

    // Event handler for the log timer elapsed
    private void LogTimerElapsed(object sender, ElapsedEventArgs e)
    {
        _alertFlag = true;
    }

    // Reset the log timer
    private void ResetLogTimer()
    {
        logTimer.Stop();
        logTimer.Start();
        _alertFlag = false;
        OnPropertyChanged("AlertFlag"); // Notify about the alert flag change
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