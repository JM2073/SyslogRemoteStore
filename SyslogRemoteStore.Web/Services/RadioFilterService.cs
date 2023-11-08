using SyslogRemoteStore.Web.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SyslogRemoteStore.Web.Services
{
    public class RadioFilterService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Message { get; set; }
        public string Severity { get; set; }

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


        public List<Log> FilterLog(List<Log> logs)
        {

            List<Log> filteredLogs = logs;

            if (!string.IsNullOrEmpty(this.Message))
            {
                filteredLogs = filteredLogs.Where(l => l.Message.Contains(this.Message)).ToList();
                filteredLogs = filteredLogs.Where(l => (Error == true && l.Severity.Contains("error")) || (Info == true && l.Severity.Contains("info")) || (Debug == true && l.Severity.Contains("debug")) || (Warning == true && l.Severity.Contains("warning"))).ToList();

            }


            return filteredLogs;

        }
    }
}
