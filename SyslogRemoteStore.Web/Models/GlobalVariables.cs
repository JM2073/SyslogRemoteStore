namespace SyslogRemoteStore.Web.Models;

public class GlobalVariables
{
    public  string DefaultIp { get; set; }
    public int DefaultPort { get; set; }
    public string WarningHex { get; set; }
    public string ErrorHex { get; set; }
    public int DebugHex { get; set; }
    public int InfoHex { get; set; }

}