using SyslogRemoteStore.Web.Enums;

namespace SyslogRemoteStore.Web.ViewModels;

public interface ISettingsViewModel
{
    List<string> AvailableIpAddress { get; }
    string IpAddress { get; set; }
    int Port { get; set; }
    ProtocolType ListeningProtocolType { get; set; }
    string WarningHex { get; set; }
    string ErrorHex { get; set; }
    string DebugHex { get; set; }
    string InfoHex { get; set; }
    void Submit();
}