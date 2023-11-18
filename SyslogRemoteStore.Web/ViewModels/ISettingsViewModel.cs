using System.Collections.ObjectModel;
using SyslogRemoteStore.Web.Enums;
using SyslogRemoteStore.Web.Models;

namespace SyslogRemoteStore.Web.ViewModels;

public interface ISettingsViewModel
{
    List<string> AvailableIpAddress { get; }
    ObservableCollection<T6S3> Radios { get; }
    string IpAddress { get; set; }
    int Port { get; set; }
    ProtocolType ListeningProtocolType { get; set; }
    string WarningHex { get; set; }
    string ErrorHex { get; set; }
    string DebugHex { get; set; }
    string InfoHex { get; set; }
    void Submit();
    void ShowRadio(Guid radioId);
}