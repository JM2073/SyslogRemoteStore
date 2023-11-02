using SyslogRemoteStore.Web.Models;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SyslogRemoteStore.Web.Services
{
    public class FilterService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string SourceIp { get; set; }
        public string Severity { get; set; }


        private void Filter()
        {
            List<Log> FilterLog(List<Log> logs, FilterService filter)
            {
                var filteredLogs = logs;

                if (!string.IsNullOrEmpty(filter.SourceIp))
                {
                    filteredLogs = filteredLogs.Where(l => l.SourceIp.Contains(filter.SourceIp)).ToList();
                }

                if (!string.IsNullOrEmpty(filter.Severity))
                {
                    filteredLogs = filteredLogs.Where(l => l.Severity.Contains(filter.Severity)).ToList();
                }

                return filteredLogs;
            }
        }
    }
}

  /*   public class FilterService : INotifyPropertyChanged
  {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/

 



