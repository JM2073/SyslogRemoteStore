using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public class RadioViewModel : BaseViewModel, IRadioViewModel
{
    private readonly CollectionStore _collectionStore;

    public RadioViewModel(ConfigurationStore configurationStore, CollectionStore collectionStore)
    {
        _configurationStore = configurationStore;
        _collectionStore = collectionStore;
        
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
}