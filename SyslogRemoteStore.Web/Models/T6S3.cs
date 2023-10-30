using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace SyslogRemoteStore.Web.Models;

public class T6S3 : IT6S3
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public T6S3(Socket socket)
    {
        this.Socket = socket;
        _logs.CollectionChanged += Logs_CollectionChanged;
    }
    
    public string Ip { get; set; }
    public int Port { get; set; }

    public Socket Socket { get; set; }
    private ObservableCollection<Log> _logs { get; set; } = new ObservableCollection<Log>();
    public ObservableCollection<Log> Logs
    {
        get { return _logs; }
    }
    
    public bool Connect()
    {
        throw new NotImplementedException();
    }

    // Event handler for changes in the Logs collection
    private void Logs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged("Logs");
    }
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}