using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolLib.Protocols
{
    public enum ProtocolType
    {
        /// <summary>
        /// DBC文件
        /// </summary>
        [Description("*.dbc")]
        DBC,
        [Description("*.xls")]
        Excel
    }

    public enum ByteOrder
    {
        Motorola,
        Intel,
    }
}
