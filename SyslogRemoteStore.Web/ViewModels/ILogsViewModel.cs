using System.Collections.ObjectModel;
using SyslogRemoteStore.Web.Models;

namespace SyslogRemoteStore.Web.ViewModels;

public interface ILogsViewModel
{
    ObservableCollection<IT6S3> Radios { get; set; }
    void Export();
    void Delete();
}