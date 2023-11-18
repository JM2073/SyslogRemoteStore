using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using SyslogRemoteStore.Web.Data;
using SyslogRemoteStore.Web.Enums;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public class SettingsViewModel : BaseViewModel, ISettingsViewModel
{
    private CollectionStore _collectionStore;
    private ConfigurationStore _configurationStore;

    public SettingsViewModel(CollectionStore collectionStore, ConfigurationStore configurationStore)
    {
        _collectionStore = collectionStore;
        _configurationStore = configurationStore;

        IpAddress = _configurationStore.Ip;
        Port = _configurationStore.Port;
        ListeningProtocolType = _configurationStore.ListeningProtocolType;
        WarningHex = _configurationStore.WarningHex;
        ErrorHex = _configurationStore.ErrorHex;
        DebugHex = _configurationStore.DebugHex;
        InfoHex = _configurationStore.InfoHex;

        Radios = _collectionStore.Radios;
        AvailableIpAddress = _collectionStore.AvailableIpAddress;
    }
    public ObservableCollection<T6S3> Radios { get; set; }
    public List<string> AvailableIpAddress { get; }

    [Required(ErrorMessage = "is Required")]
    public string IpAddress { get; set; }
    [Required(ErrorMessage = "is Required")]
    public int Port { get; set; }
    [Required(ErrorMessage = "is Required")]
    public ProtocolType ListeningProtocolType { get; set; }
    [Required(ErrorMessage = "is Required")]
    public string WarningHex { get; set; }
    [Required(ErrorMessage = "is Required")]
    public string ErrorHex { get; set; }
    [Required(ErrorMessage = "is Required")]
    public string DebugHex { get; set; }
    [Required(ErrorMessage = "is Required")]
    public string InfoHex { get; set; }


    public void Submit()
    {
        bool restartListeners = _configurationStore.Ip != IpAddress || _configurationStore.Port != Port || _configurationStore.ListeningProtocolType != ListeningProtocolType;
        
        _configurationStore.Ip = IpAddress;
        _configurationStore.Port = Port;
        _configurationStore.ListeningProtocolType = ListeningProtocolType;
        _configurationStore.WarningHex = WarningHex;
        _configurationStore.ErrorHex = ErrorHex;
        _configurationStore.DebugHex = DebugHex;
        _configurationStore.InfoHex = InfoHex;

        if (restartListeners)
        {
            RadioService.Instance.CloseOpenConnections();
        }
        
    }

    public void ToggleRadioVisibility (Guid radioId, bool value)
    {
        _collectionStore.Radios.Single(x => x.Id == radioId).IsHidden = value;
    }
    
}