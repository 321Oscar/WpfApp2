﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolLib.Signal
{
    /// <summary>
    /// 信号特性，根据特性生成控件 TextBox、CheckBox...
    /// </summary>
    [AttributeUsage(AttributeTargets.Class |
        AttributeTargets.Constructor |
        AttributeTargets.Field |
        AttributeTargets.Method |
        AttributeTargets.Property,
        AllowMultiple = true)]
    public class SignalAttribute : System.Attribute
    {
        private string description;
        /// <summary>
        /// textBox
        /// </summary>
        /// <param name="description"></param>
        public SignalAttribute(string description)
        {
            this.description = description;
        }

        /// <summary>
        /// NumricBox
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="min">最小值，数值类型</param>
        /// <param name="max">最大值，数值类型</param>
        public SignalAttribute(string description, double min = 0, double max = 0)
        {
            this.description = description;
            this.type = "int";
            this.min = min;
            this.max = max;
        }

        private int[] enumKey;
        private string[] enumString;

        /// <summary>
        /// 枚举类型
        /// </summary>
        /// <param name="description"></param>
        /// <param name="enumKey">枚举值</param>
        /// <param name="enumString">枚举显示值</param>
        public SignalAttribute(string description, int[] enumKey, string[] enumString)
        {
            this.enumKey = enumKey;
            this.enumString = enumString;
            this.description = description;
            this.type = "enum";
        }

        private string type;
        private double min;
        private double max;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get => description; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string Type { get => type; }
        /// <summary>
        /// 最小值，数值类型
        /// </summary>
        public double Min { get => min; }
        /// <summary>
        /// 最大值，数值类型
        /// </summary>
        public double Max { get => max; }
        /// <summary>
        /// 枚举类型
        /// </summary>
        public int[] EnumKey { get => enumKey; }
        public string[] EnumString { get => enumString; }
    }
}
