using System.ComponentModel;
using SyslogRemoteStore.Web.Data;
using SyslogRemoteStore.Web.Enums;
using SyslogRemoteStore.Web.Pages;

namespace SyslogRemoteStore.Web.Stores;
public class ConfigurationStore : BaseStore, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;
    
    private ProtocolType _listeningProtocolType = ProtocolType.Both;

    private string _warningHex = "#FFFF00";
    private string _errorHex = "#FF0000";
    private string _debugHex = "#00008B";
    private string _infoHex = "#000000";
    private int _port = 514;
    private string _ip = "127.0.0.1";

    public int Port
    {
        get => _port;
        set => SetValue(ref _port, value);
    }

    public string Ip
    {
        get => _ip;
        set => SetValue(ref _ip, value);
    }

    public ProtocolType ListeningProtocolType
    {
        get => _listeningProtocolType;
        set
        {
            ProtocolType oldVal = _listeningProtocolType;
            _listeningProtocolType = value;
            if (!oldVal.Equals(_listeningProtocolType))
            {
                RadioService.Instance.handlePropertyChanged(this, new PropertyChangedEventArgs(nameof(ListeningProtocolType)));
            }
        }
    }
    
    public string WarningHex
    {
        get => _warningHex;
        set
        {
            SetValue(ref _warningHex, value);
        }
    }

    public string ErrorHex
    {
        get => _errorHex;
        set
        {
            SetValue(ref _errorHex, value);
        }
    }

    public string DebugHex
    {
        get => _debugHex;
        set
        {
            SetValue(ref _debugHex, value);
        }
    }

    public string InfoHex
    {
        get => _infoHex;
        set
        {
            SetValue(ref _infoHex, value);
        }
    }
}