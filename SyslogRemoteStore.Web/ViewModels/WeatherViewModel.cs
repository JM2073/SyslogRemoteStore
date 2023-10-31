using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SyslogRemoteStore.Web.Data;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public class WeatherViewModel : BaseViewModel, IWeatherViewModel
{
    private readonly WeatherForecastService _weatherService;
    private ConfigurationStore _configurationStore;
    private readonly CollectionStore _collectionStore;

    public WeatherViewModel(WeatherForecastService weatherService, ConfigurationStore configurationStore, CollectionStore collectionStore)
    {
        _weatherService = weatherService;
        _configurationStore = configurationStore;
        _collectionStore = collectionStore;
        InitializeViewModel();
        
    }

    public ObservableCollection<T6S3> Radios { get; set; } = new ObservableCollection<T6S3>();

    public async void InitializeViewModel()
    {
        await LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        IsLoading = true;

        try
        {
            Radios = _collectionStore.Radios;
        }
        finally
        {
            IsLoading = false;
        }
    }
}