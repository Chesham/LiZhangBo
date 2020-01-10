using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LiZhangBo
{
    abstract class PropertyChangedNotifiable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T prop, T value, [CallerMemberName] string propertyName = null)
        {
            prop = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
