using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolLib.Signal
{
    public class XCPSignal : BaseSignal
    {
        /// <summary>
        /// 地址
        /// </summary>
        [Signal("ECU地址，16进制")]
        public string ECUAddress { get; set; }
        /// <summary>
        /// 转换方式
        /// </summary>
        [Signal("转换系数")]
        public double Conversion { get; set; }

        /// <summary>
        /// 转换方法
        /// </summary>
        [Signal(false)]
        public CCP_COMPU_METHOD Compu_Methd { get; set; }
        /// <summary>
        /// 标定/测量
        /// </summary>
        [Signal("标定/测量信号", "测量信号", "标定信号")]
        public bool MeaOrCal { get; set; }
        /// <summary>
        /// 大小端
        /// </summary>
        [Signal("大小端", new int[2] { 0, 1 }, new string[] { "Motorola", "Intel" })]
        public new int ByteOrder { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [Signal("数据类型", new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, new string[10] {"BOOLEAN","UBYTE" ,
            "BYTE","UWORD","SWORD","ULONG","LONG","FLOAT32_IEEE","FLOAT64_IEEE","Lookup1D_Boolean"})]
        public new int ValueType { get; set; }
        /// <summary>
        /// 拓展地址
        /// </summary>
        [Signal("拓展地址，Hex")]
        public int AddressExtension { get => addressExtension; set => addressExtension = value; }
        private int addressExtension = 0;
    }

    public class XCPSingals
    {
        public List<XCPSignal> xCPSignalList;
        public XCPSingals()
        {
            xCPSignalList = new List<XCPSignal>();
        }
    }

    public struct CCP_CHARACTERISTIC
    {
        public string cName;
        public string LongIdentifier;
        public string Characteristic_Type;
        public UInt64 cAddress;
        public string Record_Layout;
        public CCP_COMPU_METHOD Conversion_Method;
        public double Lower_Limit;
        public double Upper_Limit;
        public UInt64 cValue;
        public bool ValueValid;
        public byte DataLen;
        public string ValueDisp;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public string[] Databuf_Y_disp;
        public CCP_AXIS_DESCR AXIS_DESCR_X;
        public CCP_AXIS_DESCR AXIS_DESCR_Y;
    }

    public struct CCP_MEASUREMENT
    {
        public string cName;
        public string LongIdentifier;
        public string Data_Type;
        public CCP_COMPU_METHOD Conversion_Method;
        public double Lower_Limit;
        public double Upper_Limit;
        public UInt64 cAddress;

        public UInt64 cValue;
        public bool ValueValid;
        public byte DataLen;
        public string ValueDisp;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public string[] Databuf_Y_disp;
    }

    public struct CCP_AXIS_DESCR
    {
        public string Axis_Type;
        public string Reference_to_Input;
        public string Conversion_Method;
        public int Number_of_Axis_Pts;
        public string Lower_Limit;
        public string Upper_Limit;
        public string AXIS_PTS_REF;
        public CCP_AXIS_PTS AXIS_PTS;
        public string[] Databuf_X_disp;
    }

    public struct CCP_AXIS_PTS
    {
        public string Name;
        public string LongIdentifier;
        public UInt64 ECU_Address;
        public string InputQuantity;
        public string Record_Layout;
        public CCP_COMPU_METHOD Conversion_Method;
        public int Number_of_Axis_Pts;
        public string Lower_Limit;
        public string Upper_Limit;
        public byte DataLen;
    }


    public struct CCP_COMPU_METHOD
    {
        public string NameOfCompuMethod;
        public string LongIdentifier;
        public string Conversion_Type;
        public string Format;
        public string Units;
        public string Coefficients;
    }
}
