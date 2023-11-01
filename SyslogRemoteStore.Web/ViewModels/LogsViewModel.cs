using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public class LogsViewModel : BaseViewModel, ILogsViewModel 
{
    private readonly ConfigurationStore _configurationStore;
    private readonly CollectionStore _collectionStore;

    public LogsViewModel(ConfigurationStore configurationStore, CollectionStore collectionStore)
    {
        _configurationStore = configurationStore;
        _collectionStore = collectionStore;
        Radios = _collectionStore.Radios;
    }

    public ObservableCollection<T6S3> Radios { get; set; }
    
    public void Export()
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

}