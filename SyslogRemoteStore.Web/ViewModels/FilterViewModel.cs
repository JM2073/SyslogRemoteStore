using System;

namespace SyslogRemoteStore.Web.ViewModels
{
    public class FilterViewModel
    {
        public string SourceIp { get; set; }
        public string Severity { get; set; }
    }

    private void Filter()
    {
        public List<Log> FilterLog(List<Log> logs, FilterViewModel filter)
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

    public class FilterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}


