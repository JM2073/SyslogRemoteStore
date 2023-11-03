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


        public ObservableCollection<Log> FilterLog(ObservableCollection<Log> logs)
        {
            
                ObservableCollection<Log> filteredLogs = logs;

                if (!string.IsNullOrEmpty(this.SourceIp))
                {
                    filteredLogs = filteredLogs.Where(l => l.SourceIp.Contains(this.SourceIp)).ToList();
                }

                if (!string.IsNullOrEmpty(this.Severity))
                {
                    filteredLogs = filteredLogs.Where(l => l.Severity.Contains(this.Severity)).ToList();
                }

                return filteredLogs;
            
        }
    }
}


 



