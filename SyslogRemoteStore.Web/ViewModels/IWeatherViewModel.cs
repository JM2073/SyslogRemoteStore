using System.Collections.ObjectModel;
using SyslogRemoteStore.Web.Models;
using System.ComponentModel;

namespace SyslogRemoteStore.Web.ViewModels;

public interface IWeatherViewModel
{
    bool IsLoading { get; }
    ObservableCollection<T6S3> Radios { get; set; }
    void InitializeViewModel();
    Task LoadDataAsync();

    event PropertyChangedEventHandler PropertyChanged;
}