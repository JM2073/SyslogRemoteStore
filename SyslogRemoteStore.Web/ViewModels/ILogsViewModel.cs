using System.Collections.ObjectModel;
using SyslogRemoteStore.Web.Models;

namespace SyslogRemoteStore.Web.ViewModels;

public interface ILogsViewModel
{
    ObservableCollection<T6S3> Radios { get; set; }
    void Export();
    void Delete();
}