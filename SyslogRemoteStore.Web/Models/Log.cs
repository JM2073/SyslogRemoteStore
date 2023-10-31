using System.Text.RegularExpressions;
using SyslogRemoteStore.Web.Enums;

namespace SyslogRemoteStore.Web.Models;

public class Log
{
    public Log(string message, string sourceIp)
    {
        SourceIp = sourceIp;
        ParseMessage(message);
    }
    public string Message { get; set; }
    public bool Received { get; set; }
    public string SourceIp { get; set; }
    public string SourceItem { get; set; }
    public string Facilty { get; set; }
    public string Severity { get; set; }
    public string TimeStamp { get; set; }
    public string Tag { get; set; }

    public void ParseMessage(string message)
    {
        // Split the input string using regular expressions to capture the components
        string pattern = @"<(\d+|\S*)>(\d+)\s(\S+)\s([^,]+),([^.]+). ([^<]+)(?: - - - - )?\s(\S+)";
        Match match = Regex.Match(message, pattern);

        if (match.Success)
        {
            this.Severity = match.Groups[1].Value;
            this.Received = Convert.ToBoolean(int.Parse(match.Groups[2].Value));
            this.TimeStamp = match.Groups[3].Value;
            this.Facilty = match.Groups[4].Value;
            this.SourceItem = match.Groups[5].Value;
            this.Tag = match.Groups[6].Value;
            this.Message = match.Groups[7].Value;
              
        }
    }
}