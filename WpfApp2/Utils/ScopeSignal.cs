using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace WpfApp2.Utils
{
    public class ScopeSignal : INotifyPropertyChanged
    {
        private Brush color;
        public Brush LinearColor { get=>color; set { SetProperty(ref color, value); } }
        private double dValue;
        public double DValue { get => dValue; set { SetProperty(ref dValue, value); } }
        public string SignalName { get; set; }
        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected; 
            set
            {
                SetProperty(ref isSelected, value);
                if (SelectedChange != null)
                    SelectedChange(IsSelected, SignalName);
            }
        }

        public delegate void IsSelectedChanged(bool isSelected, string name);
        public event IsSelectedChanged SelectedChange;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets property if it does not equal existing value. Notifies listeners if change occurs.
        /// </summary>
        /// <typeparam name="T">Type of property.</typeparam>
        /// <param name="member">The property's backing field.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected virtual bool SetProperty<T>(ref T member, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(member, value))
            {
                return false;
            }

            member = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property, used to notify listeners.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
