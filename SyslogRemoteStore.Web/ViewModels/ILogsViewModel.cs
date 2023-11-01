using System.Collections.ObjectModel;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public interface ILogsViewModel
{
    ConfigurationStore _configurationStore { get; }
    ObservableCollection<IT6S3> Radios { get; }
    void Export();
    void Delete();
}