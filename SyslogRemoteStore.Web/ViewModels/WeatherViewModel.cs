﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using SyslogRemoteStore.Web.Data;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public class WeatherViewModel : BaseViewModel, IWeatherViewModel
{
    private readonly WeatherForecastService _weatherService;
    private ConfigurationStore _configurationStore;
    private CollectionStore _collectionStore;

    public WeatherViewModel(WeatherForecastService weatherService, ConfigurationStore configurationStore, CollectionStore collectionStore)
    {
        _weatherService = weatherService;
        _configurationStore = configurationStore;
        _collectionStore = collectionStore;
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