using SyslogRemoteStore.Web.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SyslogRemoteStore.Web.Services
{
    public class LogFilterService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string SourceIp { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }



        public bool Error = true;
        public bool Info = true;
        public bool Debug = true;
        public bool Warning = true;


        public bool GetSevError() => Error;
        public bool GetSevInfo() => Info;
        public bool GetSevDebug() => Debug;
        public bool GetSevWarning() => Warning;

        public void SeverityError(bool value) => Error = value;
        public void SeverityInfo(bool value) => Info = value;
        public void SeverityDebug(bool value) => Debug = value;
        public void SeverityWarning(bool value) => Warning = value;


        public List<Log> IpFilterLog(List<Log> logs)
        {

            List<Log> filteredLogs = logs;
            
            if (!string.IsNullOrEmpty(this.SourceIp))
            {
                filteredLogs = filteredLogs.Where(l => l.SourceIp.Contains(this.SourceIp)).ToList();
                //filteredLogs = filteredLogs.Where(l => (Error == true && l.Severity.Contains("error")) || (Info == true && l.Severity.Contains("info")) || (Debug == true && l.Severity.Contains("debug")) || (Warning == true && l.Severity.Contains("warning"))).ToList();
                filteredLogs = filteredLogs
                    .Where(l =>
                    {
                        var result = (Error && l.Severity.Contains("error")) ||
                     (Info && l.Severity.Contains("info")) ||
                     (Debug && l.Severity.Contains("debug")) ||
                     (Warning && l.Severity.Contains("warning"));
                        //Console.WriteLine($"Log Severity: {l.Severity}, Result: {result}");
                        return result;
                    })
                    .ToList();

            }
            


            return filteredLogs;
            
        }

        public List<Log> SeverityFilterLog(List<Log> logs)
        {

            List<Log> filteredLogs = logs;

            //filteredLogs = filteredLogs.Where(l => (Error == true && l.Severity.Contains("error")) || (Info == true && l.Severity.Contains("info")) || (Debug == true && l.Severity.Contains("debug")) || (Warning == true && l.Severity.Contains("warning"))).ToList();

            filteredLogs = filteredLogs
            .Where(l =>
                (Error && l.Severity.ToLower().Contains("error")) ||
                (Info && l.Severity.ToLower().Contains("info")) ||
                (Debug && l.Severity.ToLower().Contains("debug")) ||
                (Warning && l.Severity.ToLower().Contains("warning")))
            .ToList();

            return filteredLogs;
        }

        public List<Log> RadioFilterLog(List<Log> logs)
        {

            List<Log> filteredLogs = logs;

            if (!string.IsNullOrEmpty(this.Message))
            {
                filteredLogs = filteredLogs.Where(l => l.Message.Contains(this.Message, StringComparison.OrdinalIgnoreCase)).ToList();
                filteredLogs = filteredLogs.Where(l => (Error == true && l.Severity.Contains("error")) || (Info == true && l.Severity.Contains("info")) || (Debug == true && l.Severity.Contains("debug")) || (Warning == true && l.Severity.Contains("warning"))).ToList();

            }


            return filteredLogs;

        }
    }
}





