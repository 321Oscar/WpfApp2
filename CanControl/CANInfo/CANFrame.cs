using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanControl.CANInfo
{
    /// <summary>
    /// CAN 发送数据帧
    /// </summary>
    public class CANSendFrame
    {
        public long cid = 0;
        public Byte[] w = new Byte[8];

        public CANSendFrame(int id, byte[] data)
        {
            if (data.Length < 8)
                return;
            cid = id;

            w[0] = data[0];
            w[1] = data[1];
            w[2] = data[2];
            w[3] = data[3];
            w[4] = data[4];
            w[5] = data[5];
            w[6] = data[6];
            w[7] = data[7];
        }
        public override string ToString()
        {
            string log = $"ID:{this.cid:X};Data:";//(int) + "\n\r";
            for (int j = 0; j < this.w.Length; j++)
            {
                log += this.w[j].ToString("X") + " ";
            }
            return log;
        }
    }

    /// <summary>
    /// CAN接收数据帧
    /// </summary>
    public class CANRecieveFrame : IComparable<CANRecieveFrame>
    {
        /// <summary>
        /// CAN ID
        /// </summary>
        public int cid = 0;
        public int index = 0;
        public UInt16[] w = new ushort[4];
        public Byte[] b = new byte[8];
        public string timeStamp;

        public CANRecieveFrame(int id, byte[] data, uint timeStamp)
        {
            //if (data.Length < 8)
            //    return;

            this.timeStamp = timeStampConvertDateTimeStr(timeStamp);

            cid = id;
            index = data[0] >> 4;
            for (int i = 0; i < data.Length; i++)
            {
                b[i] = data[i];
            }
            try
            {
                w[0] = (ushort)((data[0] << 8) + data[1]);
                w[1] = (ushort)((data[2] << 8) + data[3]);
                w[2] = (ushort)((data[4] << 8) + data[5]);
                w[3] = (ushort)((data[6] << 8) + data[7]);
            }
            catch (Exception err)
            {

            }
        }

        public int CompareTo(CANRecieveFrame other)
        {
            int rst = this.timeStamp.CompareTo(other.timeStamp);
            return rst;
        }

        public override string ToString()
        {
            string log = $"ID:{this.cid:X};Data:";//(int) + "\n\r";
            for (int j = 0; j < this.b.Length; j++)
            {
                log += this.b[j].ToString("X") + " ";
            }
            return log;
        }

        private string timeStampConvertDateTimeStr(uint timeStamp)
        {
            //DateTime t = DateTime.Now;

            //System.DateTime startTime = new DateTime(t.Year, t.Month, t.Day,0,0,0);
            //t = startTime.AddMilliseconds(timeStamp);
            int hour = (int)(timeStamp / 36000000);
            int minute = (int)(timeStamp - hour * 36000000) / 600000;
            int second = (int)(timeStamp - hour * 36000000 - minute * 600000) / 10000;
            int ms = (int)(timeStamp - hour * 36000000 - minute * 600000 - second * 10000) / 10;
            int mms = (int)(timeStamp - hour * 36000000 - minute * 600000 - second * 10000 - ms * 10);
            //t = new DateTime(t.Year, t.Month, t.Day, hour, minute, second, ms);
            return $"{hour.ToString().PadLeft(2, '0')}:{minute.ToString().PadLeft(2, '0')}:{second.ToString().PadLeft(2, '0')} {ms.ToString().PadLeft(3, '0')}.{mms}";
        }
    }

   
}
