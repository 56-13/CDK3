using System;
using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;

namespace CDK
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T src, T dest, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(src, dest))
            {
                src = dest;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }


        public void AddWeakPropertyChanged(EventHandler<PropertyChangedEventArgs> handler)
        {
            WeakEventManager<NotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(this, "PropertyChanged", handler);
        }

        public void RemoveWeakPropertyChanged(EventHandler<PropertyChangedEventArgs> handler)
        {
            WeakEventManager<NotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(this, "PropertyChanged", handler);
        }
    }
}
