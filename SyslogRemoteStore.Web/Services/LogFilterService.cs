using SyslogRemoteStore.Web.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;


namespace SyslogRemoteStore.Web.Services
{
    public class LogFilterService : INotifyPropertyChanged
    {
        private bool _error = true;
        private bool _info = true;
        private bool _debug = true;
        private bool _warning = true;
        private string _message;

        public string Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }

        public bool Error
        {
            get => _error;
            set => SetField(ref _error, value);
        }

        public bool Info
        {
            get => _info;
            set => SetField(ref _info, value);
        }

        public bool Debug
        {
            get => _debug;
            set => SetField(ref _debug, value);
        }

        public bool Warning
        {
            get => _warning;
            set => SetField(ref _warning, value);
        }
        
        public List<Log> FilterLog(List<Log> logs)
        {

            List<Log> filteredLogs = logs;

           // filteredLogs = filteredLogs.Where(l => (Error == true && l.Severity.Contains("error")) || (Info == true && l.Severity.Contains("info")) || (Debug == true && l.Severity.Contains("debug")) || (Warning == true && l.Severity.Contains("warning"))).ToList();

            if (!string.IsNullOrEmpty(this.Message))
            {
                filteredLogs = filteredLogs.Where(l => l.Message.Contains(Message)).ToList();
            }

            return filteredLogs;
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}


 



