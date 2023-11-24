using System.Net;
using System.Text.RegularExpressions;

namespace SyslogRemoteStore.Web.Models;

public class Log
{
    public Log(string message)
    {
        ParseMessage(message);
    }

    public string Message { get; set; }
    public string SourceItem { get; set; }
    public string Facilty { get; set; }
    public string Severity { get; set; }
    public string TimeStamp { get; set; }

    private void ParseMessage(string message)
    {
        string[] result = message.Split("]", StringSplitOptions.None);

        Message = result.Length > 1 ? result[1].Trim() : result[0].Trim();

        int startIndex = message.IndexOf('<');
        int endIndex = message.IndexOf('>');
        string prioritySubstring = (startIndex != -1 && endIndex != -1) ? message.Substring(startIndex + 1, endIndex - startIndex - 1) : "0";

        int priorityValue = Int32.TryParse(prioritySubstring, out priorityValue) ? priorityValue : 0;
        Facilty = GetFacilty(priorityValue / 8);
        Severity = GetSeverity(priorityValue % 8);
      
        startIndex = message.IndexOf("eti=\"", StringComparison.Ordinal);
        endIndex = message.IndexOf("\"]", StringComparison.Ordinal);
        
        TimeStamp = (startIndex != -1 && endIndex != -1)
            ? message.Substring(startIndex + 5, endIndex - startIndex - 5).Trim()
            : string.Empty;
    }

    private string GetSeverity(int severity)
    {
        string result = severity switch
        {
            0 => "emergency",
            1 => "alert",
            2 => "critical",
            3 => "error",
            4 => "warning",
            5 => "notice",
            6 => "informational",
            7 => "debug",
            _ => string.Empty
        };

        return result;
    }

    private string GetFacilty(int facility)
    {
        string result = facility switch
        {
            0 => "kernel messages",
            1 => "user-level messages",
            2 => "mail system",
            3 => "system daemons",
            4 => "security/authorization messages",
            5 => "messages generated internally by Syslog",
            6 => "line printer subsystem",
            7 => "network news subsystem",
            8 => "UUCP subsystem",
            9 => "clock daemon",
            10 => "security/authorization messages",
            11 => "FTP daemon",
            _ => string.Empty
        };

        return result;
    }
}