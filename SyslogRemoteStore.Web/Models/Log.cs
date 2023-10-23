using SyslogRemoteStore.Web.Enums;

namespace SyslogRemoteStore.Web.Models;

public class Log
{
    public SeverityLevel SeverityLevel { get; set; }
    public string Message { get; set; }
}