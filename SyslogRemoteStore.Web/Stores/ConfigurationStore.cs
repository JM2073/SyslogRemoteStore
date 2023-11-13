using System.ComponentModel;
using SyslogRemoteStore.Web.Enums;

namespace SyslogRemoteStore.Web.Stores;
public class ConfigurationStore : BaseStore
{
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private ProtocolType _listeningProtocolType = ProtocolType.Both;

    private string _warningHex = "#FFFF00";
    private string _errorHex = "#FF0000";
    private string _debugHex = "#00008B";
    private string _infoHex = "#000000";
    public int Port { get; set; } = 514;
    public string Ip { get; set; } = "127.0.0.1";

    public ProtocolType ListeningProtocolType
    {
        get => _listeningProtocolType;
        set
        {
            SetValue(ref _listeningProtocolType, value);
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