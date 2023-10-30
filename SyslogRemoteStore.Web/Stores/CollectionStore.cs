using SyslogRemoteStore.Web.Models;

namespace SyslogRemoteStore.Web.Stores;

public class CollectionStore : BaseStore
{
    private List<T6S3> _radios = new List<T6S3>();
    public List<T6S3> Radios 
    {
        get => _radios;
        set
        {
            SetValue(ref _radios, value);
        }
    }


}

