using SyslogRemoteStore.Web.Enums;

namespace SyslogRemoteStore.Web.Stores;
public class ConfigurationStore : BaseStore
{
    public int Port { get; set; } = 514;
    public string Ip { get; set; } = "127.0.0.1";
    public ProtocolType ListeningProtocolType { get; set; } = ProtocolType.UdpAndTcp;
    public string WarningHex { get; set; }
    public string ErrorHex { get; set; }
    public string DebugHex { get; set; }
    public string InfoHex { get; set; }
}