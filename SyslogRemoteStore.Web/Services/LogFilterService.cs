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
        private bool _error = false;
        private bool _info = false;
        private bool _debug = false;
        private bool _warning = false;
        private string _message;
        private string _source;

        public string Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }

        public string Source
        {
            get => _source;
            set => SetField(ref _source, value);
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
            
            if (Error  || Info  || Debug  || Warning )
            {
                filteredLogs = filteredLogs.Where(l => (Error == true && l.Severity.Contains("error")) || (Info == true && l.Severity.Contains("info")) || (Debug == true && l.Severity.Contains("debug")) || (Warning == true && l.Severity.Contains("warning"))).ToList();
            }

            if (!string.IsNullOrEmpty(this.Message))
            {
                filteredLogs = filteredLogs.Where(l => l.Message.Contains(this.Message, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(this.Source))
            {
                filteredLogs = filteredLogs.Where(l => l.SourceItem.Contains(this.Source, StringComparison.OrdinalIgnoreCase)).ToList();
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


 



