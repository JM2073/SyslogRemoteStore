using System.Collections;
using SyslogRemoteStore.Web.Models;
using SyslogRemoteStore.Web.Stores;

namespace SyslogRemoteStore.Web.ViewModels;

public class LogsViewModel : ILogsViewModel
{
    private readonly ConfigurationStore _configurationStore;
    private readonly CollectionStore _collectionStore;

    public LogsViewModel(ConfigurationStore configurationStore, CollectionStore collectionStore)
    {
        _configurationStore = configurationStore;
        _collectionStore = collectionStore;
        Radios = _collectionStore.Radios;
    }
    public List<T6S3> Radios { get; set; } = new List<T6S3>();

    public void Export()
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }
}