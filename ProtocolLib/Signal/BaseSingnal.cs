using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            get => strValue;
            set
            {
                if (value != strValue)
                {
                    strValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StrValue"));
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

    }
}
