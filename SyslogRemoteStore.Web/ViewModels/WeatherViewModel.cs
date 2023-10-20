using System.ComponentModel;
using System.Runtime.CompilerServices;
using SyslogRemoteStore.Web.Data;

namespace SyslogRemoteStore.Web.ViewModels;

public class WeatherViewModel : BaseViewModel, IWeatherViewModel
{
    private readonly WeatherForecastService _weatherService;

    public WeatherViewModel(WeatherForecastService weatherService)
    {
        _weatherService = weatherService;
        InitializeViewModel();
    }


    public WeatherForecast[] Forecasts { get; set; } = Array.Empty<WeatherForecast>();

    public async void InitializeViewModel()
    {
        await LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        IsLoading = true;

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2)); // simulate loading

            Forecasts = await _weatherService.GetForecastAsync(DateTime.Now);
        }
        finally
        {
            IsLoading = false;
        }
    }
}