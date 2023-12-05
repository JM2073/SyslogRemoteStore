using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.JSInterop;
using SyslogRemoteStore.Web.Data;
using SyslogRemoteStore.Web.Enums;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Services;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public class SettingsViewModel : BaseViewModel, ISettingsViewModel
{
    private CollectionStore _collectionStore;
    private ConfigurationStore _configurationStore;
    private readonly ExsportService _exsportService = new ExsportService();
    private IJSRuntime _js;

    public SettingsViewModel(CollectionStore collectionStore, ConfigurationStore configurationStore, IJSRuntime js)
    {
        _collectionStore = collectionStore;
        _configurationStore = configurationStore;
        _js = js;

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
        bool killListeners = _configurationStore.Ip != IpAddress || _configurationStore.Port != Port;
        bool restartListeners =  _configurationStore.ListeningProtocolType != ListeningProtocolType;
        
        _configurationStore.Ip = IpAddress;
        _configurationStore.Port = Port;
        _configurationStore.ListeningProtocolType = ListeningProtocolType;
        _configurationStore.WarningHex = WarningHex;
        _configurationStore.ErrorHex = ErrorHex;
        _configurationStore.DebugHex = DebugHex;
        _configurationStore.InfoHex = InfoHex;

        if (killListeners)
        {
            RadioService.Instance.CloseAllOpenConnections();
        }
        else if(restartListeners)
        {
            RadioService.Instance.ChangeProtocoleType();
        }
        
    }

    public void ToggleRadioVisibility (Guid radioId, bool value)
    {
        _collectionStore.Radios.Single(x => x.Id == radioId).IsHidden = value;
    }
    
    public async void ExportLogs(Guid radioId)
    {
        try
        {
            Stream fileStream = _exsportService.ProcessZipFiles(_collectionStore.Radios.Single(x=>x.Id == radioId));
            using DotNetStreamReference streamRef = new(fileStream);
            await _js.InvokeVoidAsync("downloadFileFromStream", "SyslogFile.zip", streamRef);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Cannot Download file");
        }
    }

    public void DeleteLocalLogs(Guid radioId)
    {
        T6S3 radio = _collectionStore.Radios.Single(x => x.Id == radioId);
        int count = radio.Logs.Count;
        for (int i = 0; i < count; i++)
        {
            radio.Logs.RemoveAt(0);
        }
    }
}