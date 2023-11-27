using System.Net;
using System.Text.RegularExpressions;

namespace SyslogRemoteStore.Web.Models;

public class Log
{
    public Log(string message, string sourceIp, int sourcePort)
    {
        SourceIp = sourceIp;
        SourcePort = sourcePort;
        ParseMessage(message);
    }

    public string FullMessage { get; set; }
    public string Message { get; set; }
    public bool Received { get; set; }
    public string SourceIp { get; set; }
    public int SourcePort { get; set; }
    public string SourceItem { get; set; }
    public string Facilty { get; set; }
    public string Severity { get; set; }
    public string TimeStamp { get; set; }
    public string Tag { get; set; }

    public void ParseMessage(string message)
    {
        // Split the input string using regular expressions to capture the components
        string pattern = @"<(\d+|\S*)>(\d+)\s(\S+)\s([^.]+). ([^<]+)(?: - - - - )?\s(\S+)";
        Match match = Regex.Match(message, pattern);

        if (match.Success)
        {
            int priorityValue = Int32.TryParse(match.Groups[1].Value, out priorityValue) ? priorityValue : 0;
            int facility = priorityValue >> 3;
            int severity = priorityValue & 7;

            FullMessage = message;
            Severity = GetSeverity(severity);
            Received = Convert.ToBoolean(int.Parse(match.Groups[2].Value));
            TimeStamp = match.Groups[3].Value;
            Facilty = GetFacilty(facility);
            SourceItem = match.Groups[4].Value;
            Tag = match.Groups[5].Value;
            Message = match.Groups[6].Value;
        }
    }

    private string GetSeverity(int severity)
    {
        string result = string.Empty;
        switch (severity)
        {
            case 0:
                result = "emergency";
                break;
            case 1:
                result = "alert";
                break;
            case 2:
                result = "critical";
                break;
            case 3:
                result = "error";
                break;
            case 4:
                result = "warning";
                break;
            case 5:
                result = "notice";
                break;
            case 6:
                result = "informational";
                break;
            case 7:
                result = "debug";
                break;
        }

        return result;
    }

    private string GetFacilty(int facility)
    {
        string result = string.Empty;
        switch (facility)
        {
            case 0:
                result = "kernel messages";
                break;
            case 1:
                result = "user-level messages";
                break;
            case 2:
                result = "mail system";
                break;
            case 3:
                result = "system daemons";
                break;
            case 4:
                result = "security/authorization messages";
                break;
            case 5:
                result = "messages generated internally by Syslog";
                break;
            case 6:
                result = "line printer subsystem";
                break;
            case 7:
                result = "network news subsystem";
                break;
            case 8:
                result = "UUCP subsystem";
                break;
            case 9:
                result = "clock daemon";
                break;
            case 10:
                result = "security/authorization messages";
                break;
            case 11:
                result = "FTP daemon";
                break;
        }

        return result;
    }
    
    public string GetFormatedIp()
    {
        IPAddress ip = IPAddress.Parse(this.SourceIp);
        return $"{(IPAddress.Parse(this.SourceIp).IsIPv4MappedToIPv6 ? ip.MapToIPv4().ToString() : ip.MapToIPv6().ToString())}:{this.SourcePort}";
    }
}