using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.JSInterop;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Services;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public class LogsViewModel : BaseViewModel, ILogsViewModel 
{
    private readonly CollectionStore _collectionStore;
    private readonly IJSRuntime _js;
    private readonly ExsportService _exsportService = new ExsportService();
    public LogsViewModel(ConfigurationStore configurationStore, CollectionStore collectionStore, IJSRuntime js)
    {
        _configurationStore = configurationStore;
        _collectionStore = collectionStore;
        _js = js;
        Radios = _collectionStore.Radios;
        
        
    }
    public ConfigurationStore _configurationStore { get; }
    
    public ObservableCollection<IT6S3> Radios { get; set; }
    
    public async void Export()
    {
        try
        {
            Stream fileStream = _exsportService.ZipFiles(_collectionStore.Radios.ToList());
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
        throw new NotImplementedException();
    }

}