using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Services;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public class RadioViewModel : BaseViewModel, IRadioViewModel
{
    private readonly CollectionStore _collectionStore;
    private readonly IJSRuntime _js;
    private readonly ExsportService _exsportService = new ExsportService();
    public RadioViewModel(ConfigurationStore configurationStore, CollectionStore collectionStore, IJSRuntime js)
    {
        _configurationStore = configurationStore;
        _collectionStore = collectionStore;
        _js = js;
    }
    public ConfigurationStore _configurationStore { get; }

    public T6S3 Radio { get; set; }

    private string _radioId;
    public string RadioId
    {
        get => _radioId;
        set
        {
            SetValue(ref _radioId, value);
            Radio = _collectionStore.Radios.Single(x => x.Id == Guid.Parse(_radioId));
        }
    }

    public void ToggleRadioVisibility (bool value)
    {
        _collectionStore.Radios.Single(x => x.Id == Guid.Parse(RadioId)).IsHidden = value;
    }

    public void HideRadio()
    {
        _collectionStore.Radios.Single(x => x.Id == Guid.Parse(_radioId)).IsHidden = true;
    }
    
    public async void Export()
    {
        try
        {
            Stream fileStream = _exsportService.ProcessZipFiles(Radio);
            using DotNetStreamReference streamRef = new(fileStream);
            await _js.InvokeVoidAsync("downloadFileFromStream", "SyslogFiles.zip", streamRef);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Cannot Download file");
        }
    }

    public void Delete()
    {
        int count = Radio.Logs.Count;
        for (int i = 0; i < count; i++)
        {
            Radio.Logs.RemoveAt(0);
        }
    }
}