using SyslogRemoteStore.Web.Data;
using System.ComponentModel;

namespace SyslogRemoteStore.Web.ViewModels;

public interface IWeatherViewModel
{
    bool IsLoading { get; }
    WeatherForecast[] Forecasts { get; }
    void InitializeViewModel();
    Task LoadDataAsync();

    event PropertyChangedEventHandler PropertyChanged;
}