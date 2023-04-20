using ProtocolLib.Signal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolLib.Protocols.XCP
{
    /// <summary>
    /// XCP通用方法
    /// <para><see cref="XCPHelper"/></para>
    /// </summary>
    public static class XCPHelper
    {
        public const byte STD_CONNECT = 0xFF;
        public const byte STD_DISCONNECT = 0xFE;
        public const byte STD_SHORTUPLOAD = 0xF4;
        public const byte STD_SET_MTA = 0xF6;
        public const byte STD_UPLOAD = 0xF5;
        public const byte STD_SENDKEY = 0xF7;
        public const byte STD_GETSEED = 0xF8;
        public const byte CAL_DOWNLOAD = 0xF0;

        public static XCPResponse TransformBytetoRes(byte code)
        {
            switch (code)
            {
                case 0xFF:
                    return XCPResponse.Ok;
                case 0xFE:
                    return XCPResponse.Err;
                default:
                    return XCPResponse.Ok;
            }
        }

        internal static byte[] CalKeyWithSeed(List<byte> seeds)
        {
            throw new NotImplementedException();
        }

        internal static byte[] ConvertToByte(string writeData, int valueType)
        {
            byte[] rntData = new byte[8];
            switch ((XCPValueType)valueType)
            {
                case XCPValueType.Scalar_BOOLEAN:
                    int vBool = int.Parse(writeData);
                    bool b = Convert.ToBoolean(vBool);
                    rntData[0] = Convert.ToByte(b);
                    break;
                case XCPValueType.Scalar_UBYTE:
                    UInt16 ui = Convert.ToUInt16(writeData);
                    rntData[0] = Convert.ToByte(ui);
                    break;
                case XCPValueType.Scalar_BYTE:
                    Int16 i = Convert.ToInt16(writeData);
                    rntData[0] = Convert.ToByte(i);
                    break;
                case XCPValueType.Scalar_UWORD:
                    {
                        UInt64 temp = Convert.ToUInt64(writeData);
                        rntData[1] = Convert.ToByte(temp & 0xff);//
                        rntData[0] = Convert.ToByte((temp >> 8) & 0xff);//
                    }
                    break;
                case XCPValueType.Scalar_SWORD:
                    {
                        Int64 temp = Convert.ToInt64(writeData);
                        rntData[1] = Convert.ToByte(temp & 0xff);//
                        rntData[0] = Convert.ToByte((temp >> 8) & 0xff);//
                    }
                    break;
                case XCPValueType.Scalar_ULONG:
                    {
                        UInt64 temp = Convert.ToUInt64(writeData);
                        rntData[3] = Convert.ToByte(temp & 0xff);//
                        rntData[2] = Convert.ToByte((temp >> 8) & 0xff);//
                        rntData[1] = Convert.ToByte((temp >> 16) & 0xff);//
                        rntData[0] = Convert.ToByte((temp >> 24) & 0xff);//
                    }
                    break;
                case XCPValueType.Scalar_LONG:
                    {
                        Int64 temp = Convert.ToInt64(writeData);
                        rntData[3] = Convert.ToByte(temp & 0xff);//
                        rntData[2] = Convert.ToByte((temp >> 8) & 0xff);//
                        rntData[1] = Convert.ToByte((temp >> 16) & 0xff);//
                        rntData[0] = Convert.ToByte((temp >> 24) & 0xff);//
                    }
                    break;
                case XCPValueType.Scalar_FLOAT32_IEEE:
                    {
                        Single temp = Convert.ToSingle(writeData);
                        byte[] temp1 = BitConverter.GetBytes(temp);
                        rntData[0] = temp1[3];
                        rntData[1] = temp1[2];
                        rntData[2] = temp1[1];
                        rntData[3] = temp1[0];
                    }
                    break;
                case XCPValueType.Scalar_FLOAT64_IEEE:
                    {
                        double temp = Convert.ToDouble(writeData);
                        byte[] temp1 = BitConverter.GetBytes(temp);

                        rntData[0] = temp1[7];
                        rntData[1] = temp1[6];
                        rntData[2] = temp1[5];
                        rntData[3] = temp1[4];
                        rntData[4] = temp1[3];
                        rntData[5] = temp1[2];
                        rntData[6] = temp1[1];
                        rntData[7] = temp1[0];
                    }
                    break;
                case XCPValueType.Lookup1D_BOOLEAN:
                    {
                        bool temp = Convert.ToBoolean(writeData);
                        rntData[0] = Convert.ToByte(temp);//length
                    }
                    break;
            }
            return rntData;
        }

        /// <summary>
        /// 解析4位byte数据
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="resData"></param>
        /// <returns></returns>
        internal static string DealData4Byte(XCPSignal signal, byte[] resData)
        {
            Int64 temp = 0;
            for (int i = 0; i < signal.Length; i++)
            {
                temp = temp << 8 | resData[i];
            }

            switch ((XCPValueType)signal.ValueType)
            {
                case XCPValueType.Scalar_BOOLEAN:
                case XCPValueType.Scalar_UBYTE:
                case XCPValueType.Scalar_UWORD:
                case XCPValueType.Scalar_SWORD:
                case XCPValueType.Scalar_ULONG:
                case XCPValueType.Scalar_LONG:
                    return Convert.ToString(temp);
                case XCPValueType.Scalar_BYTE:
                    return Convert.ToString((int)temp);
                case XCPValueType.Scalar_FLOAT32_IEEE:
                    {
                        var v = BitConverter.ToSingle(resData, 0);
                        return XCPHelper.MethodStruConvert(signal.Compu_Methd, v);
                    }
                case XCPValueType.Scalar_FLOAT64_IEEE:
                    byte[] temp1 = new byte[4];
                    temp1[0] = (byte)(temp & 0xff);
                    temp1[1] = (byte)((temp >> 8) & 0xff);
                    temp1[2] = (byte)((temp >> 16) & 0xff);
                    temp1[3] = (byte)((temp >> 24) & 0xff);
                    return Convert.ToString(BitConverter.ToSingle(temp1, 0));
            }

            return "--";
        }

        internal static void TransformAddress(string eCUAddress, int address_TYPE, out byte[] address)
        {
            address = new byte[4];
            //int add = int.Parse(eCUAddress,System.Globalization.NumberStyles.HexNumber)
            if (ulong.TryParse(eCUAddress, System.Globalization.NumberStyles.HexNumber, null, out ulong Address))
            {
                if (address_TYPE == 0)
                {
                    address[0] = (Byte)(Address >> 24 & 0xff);
                    address[1] = (Byte)(Address >> 16 & 0xff);
                    address[2] = (Byte)(Address >> 8 & 0xff);
                    address[3] = (Byte)(Address & 0xff);
                }
                else if (address_TYPE == 1)//EcuType.CompareTo("Intel") == 0)
                {
                    address[0] = (Byte)(Address & 0xff);
                    address[1] = (Byte)(Address >> 8 & 0xff);
                    address[2] = (Byte)(Address >> 16 & 0xff);
                    address[3] = (Byte)(Address >> 24 & 0xff);
                }
            }
        }

        public static int RecordStrConvert(string record)
        {
            if (Enum.TryParse(record, out XCPValueType valueType))
            {
                return (int)valueType;
            }
            else
            {
                throw new Exception("未知类型，" + record);
            }
        }

        /// <summary>
        /// 将method转成系数
        /// </summary>
        /// <param name="conversion_Method"></param>
        /// <returns></returns>
        internal static string MethodStruConvert(CCP_COMPU_METHOD conversion_Method, float oldValue)
        {
            try
            {
                if (conversion_Method.Format == null)
                    return oldValue.ToString();
                var formats = conversion_Method.Format.Split('%', '.', '"');
                int length = int.Parse(formats[2]);//总长
                int length2 = int.Parse(formats[3]);//小数位数
                string format = $"f{length2}";
                if (conversion_Method.Conversion_Type.ToLower().Contains("rat"))
                {
                    string[] coffs = conversion_Method.Coefficients.Split(' ');
                    float a = float.Parse(coffs[1]);
                    float b = float.Parse(coffs[2]);
                    float c = float.Parse(coffs[3]);
                    float d = float.Parse(coffs[4]);
                    float e = float.Parse(coffs[5]);
                    float f = float.Parse(coffs[6]);
                    float x = ((a * oldValue * oldValue) + (b * oldValue) + c) / (d * oldValue * oldValue + e * oldValue + f);
                    return x.ToString(format);
                }
            }
            catch (Exception ex)
            {
                ShowLog("MethodStruConvert", ex);

            }
            return oldValue.ToString();
        }

        internal static void ParseCommModeBasic(byte v, out ByteOrder bo, out bool slaveBlockavali)
        {
            int by = v & 1;
            bo = by == 0 ? ByteOrder.Intel : ByteOrder.Motorola;
            slaveBlockavali = (v & (1 >> 6)) != 0;
        }

        /// <summary>
        /// 显示日志事件
        /// </summary>
        public static event Logger ShowLog;
    }
}
