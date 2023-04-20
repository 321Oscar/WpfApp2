using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolLib.Signal
{
    public class BaseSignal : INotifyPropertyChanged, IComparable<BaseSignal>
    {
        protected string name;
        private string strValue;
        private int valueType;
        private int length;
        private int byteOrder;
        private string unit;
        private string comment;
        private double minimum;
        private double max;
        private bool isSelected;
        private double dValue;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
            //{
            //    if (value != isSelected)
            //    {
            //        isSelected = value;
            //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            //    }
            //}
        }

        public double DValue
        {
            get => dValue;
            set => SetProperty(ref dValue, value);
        }

        /// <summary>
        /// 信号名称
        /// </summary>
        public string SignalName { get => name; set => name = value; }
        public string Unit { get => unit; set => unit = value; }
        public string Comment { get => comment; set => comment = value; }
        public double Minimum { get => minimum; set => minimum = value; }
        public double Maximum { get => max; set => max = value; }
        public string StrValue
        {
            get => dValue.ToString();
            set
            {
                if (value != dValue.ToString() && double.TryParse(value, out dValue))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StrValue)));
                }
            }
        }

        public int ValueType { get => valueType; set => valueType = value; }
        [SignalAttribute("长度", min: 0, max: 16)]
        public int Length { get => length; set => length = value; }
        /// <summary>
        /// 大小端：Intel 小端格式：地址的增长顺序与值的增长顺序相同
        /// </summary>
        [Signal("1:Intel;0:Motorola", new int[2] { 0, 1 }, new string[] { "Motorola", "Intel" })]
        public int ByteOrder { get => byteOrder; set => byteOrder = value; }

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

        #region 比较
        public int CompareTo(BaseSignal other)
        {
            return this.SignalName.CompareTo(other.SignalName);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else if (this.GetType() != obj.GetType())
                return false;
            else
                return ((BaseSignal)obj).SignalName == this.SignalName;
        }

        public override int GetHashCode()
        {
            return this.SignalName.GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            return $"{SignalName}:{DValue:f2}";
        }
    }
}
