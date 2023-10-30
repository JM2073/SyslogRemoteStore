using SyslogRemoteStore.Web.Models;
using System.ComponentModel;

namespace SyslogRemoteStore.Web.ViewModels;

public interface IWeatherViewModel
{
    bool IsLoading { get; }
    List<T6S3> Radios { get; set; }
    void InitializeViewModel();
    Task LoadDataAsync();

    event PropertyChangedEventHandler PropertyChanged;
}