using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using SyslogRemoteStore.Web.Models;

namespace SyslogRemoteStore.Web.Stores;

public class CollectionStore : BaseStore
{
    private ObservableCollection<T6S3> _radios = new ObservableCollection<T6S3>();
    public ObservableCollection<T6S3> Radios 
    {
        get => _radios;
        set => SetValue(ref _radios, value);
    }
    public List<string> AvailableIpAddress { get; set; }
    
}

