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
        Excel,
        [Description("*.a2l")]
        XCP
    }

    public enum ByteOrder
    {
        Motorola,
        Intel,
    }

    public enum XCPValueType
    {
        Scalar_BOOLEAN,
        Scalar_UBYTE,//一字节无符号整型
        Scalar_BYTE,//一字节有符号
        Scalar_UWORD,//2字节无符号整型
        Scalar_SWORD,//2字节有符号整型
        Scalar_ULONG,//4字节无符号
        Scalar_LONG,//四字节有符号
        Scalar_FLOAT32_IEEE,//4字节浮点型
        Scalar_FLOAT64_IEEE,//8字节浮点型
        Lookup1D_BOOLEAN,
    }

    public enum XCPResponse
    {
        Ok = 0xff,
        Err = 0xFE,
        Undefined = -2,
        Out_time,
    }

    public enum XCPCMDStatus
    {
        Upload,
        UploadSucc,
        UploadFail,
        ShortUpload,
        ShortUploadSucc,
        ShortUploadFail,
        Set_MTAFail,
        GetSeed,
        GetSeedFail,
        SendKey,
        SendKeyFail,
        UnlockSucc,
        DownLoadSucc,
        DownLoadFail,
        DownLoad
    }

    public enum XCPConnectStatus
    {
        Init,
        Connecting,
        ConnectFail,
        Connected,
        DisConnect,
    }

    public enum XCPMode
    {
        Polling,
        Block
    }
}
