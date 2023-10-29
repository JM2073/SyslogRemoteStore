namespace SyslogRemoteStore.Web.Stores;
public class ConfigurationStore : BaseStore
{
    public int Port { get; set; } = 514;
    public string Ip { get; set; } = "127.0.0.1";
}