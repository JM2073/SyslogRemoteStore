using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SyslogRemoteStore.Web.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private bool isLoading = false;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                SetValue(ref isLoading, value);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetValue<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }
}
