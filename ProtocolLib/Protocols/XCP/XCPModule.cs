using CanControl.CANInfo;
using ProtocolLib.Signal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolLib.Protocols.XCP
{
    /*
     * 一个xcp模块
     * 
     * 属性：
     * 所属的project
     * mode:polling or block
     * master id
     * slave id
     * 
     * 字段：
     * 当前xcp状态：连接，未连接，upload，download等
     * 根据slaveid接收的数据队列
     * 
     * 方法：
     * xcp connect 
     *  通过CAN发送数据 并接收slaveid的消息
     *  返回 TRUE/False
     * xcp disconnect
     * xcp upload
     * xcp shortupload
     * xcp set_mta
     * xcp download
     * xcp getseed
     * xcp sendkey
     * 
     * 事件：
     * XCPConnectStatusChanged
     * XCPCMDStatusChanged
     * 
     * private：
     * sendCMD
     * recieve data 封装一层 判断slaveID
     * 
     */
    public class XCPModule
    {
        private uint masterid;
        private uint slaveid;
        private XCPMode current_mode = XCPMode.Polling;
        private XCPConnectStatus connectStatus = XCPConnectStatus.Init;
        private XCPCMDStatus currentCMDStatus;
        private Queue<CANRecieveFrame> receiveData;
        private AutoResetEvent ReceiveEvent;
        private Thread receiveThread;
        private bool receiveRunning = false;
        private ByteOrder byteOrder;
        private bool slaveBlockAvail;

        private object projectItem;
        /// <summary>
        /// 主机 CAN ID
        /// </summary>
        public uint Masterid { get => masterid; set => masterid = value; }
        /// <summary>
        /// 从机CAN ID
        /// </summary>
        public uint Slaveid { get => slaveid; set => slaveid = value; }
        /// <summary>
        /// 主机 发送模式
        /// </summary>
        public XCPMode Current_Mode { get => current_mode; set => current_mode = value; }
        /// <summary>
        /// 当前的命令
        /// </summary>
        public XCPCMDStatus CurrentCMDStatus
        {
            get => currentCMDStatus;
            set
            {
                if (value != currentCMDStatus)
                {
                    WhenCMDChange(value.ToString());
                }
                currentCMDStatus = value;
            }
        }
        /// <summary>
        /// 当前连接状态
        /// </summary>
        public XCPConnectStatus ConnectStatus
        {
            get => connectStatus;
            set
            {
                if (value != connectStatus)
                {
                    WhenConnectChange(value.ToString());
                }
                connectStatus = value;
            }
        }

        /// <summary>
        /// 从机中的byteorder
        /// </summary>
        public ByteOrder ByteOrder { get => byteOrder; set => byteOrder = value; }
        /// <summary>
        /// 从机是否支持block
        /// </summary>
        public bool SlaveBlockAvail { get => slaveBlockAvail; set => slaveBlockAvail = value; }

        public delegate void XCPConnectStatusChanged(object sender, EventArgs args);

        public event XCPConnectStatusChanged OnConnectStatusChanged;

        private void WhenConnectChange(string status)
        {
            if (OnConnectStatusChanged != null)
                OnConnectStatusChanged(status, null);
        }

        public delegate void XCPCMDStatusChanged(object sender, EventArgs args);

        public event XCPCMDStatusChanged OnCMDStatusChanged;

        private void WhenCMDChange(string status)
        {
            if (OnCMDStatusChanged != null)
                OnCMDStatusChanged(status, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterid"></param>
        /// <param name="slaveid"></param>
        /// <param name="projectItem"></param>
        public XCPModule(uint masterid, uint slaveid, object projectItem)
        {
            this.masterid = masterid;
            this.slaveid = slaveid;
            this.projectItem = projectItem;

            ReceiveEvent = new AutoResetEvent(false);
            receiveData = new Queue<CANRecieveFrame>();
            ByteOrder = ByteOrder.Intel;
            SlaveBlockAvail = true;
        }

        private void startReceiveThread(object canIndex)
        {
            if (receiveThread != null)
            {
                //receiveThread.Join();
                receiveThread = null;
            }

            receiveThread = new Thread(new ParameterizedThreadStart(Receive));
            receiveThread.IsBackground = true;
            receiveThread.Start(canIndex);
            receiveRunning = true;
        }

        public void ClearQueue()
        {
            this.receiveData.Clear();
        }

        /// <summary>
        /// 发送连接命令
        /// </summary>
        /// <returns></returns>
        public bool Connect(uint canIndex = 0, uint connectMode = 0x00)
        {
            ConnectStatus = XCPConnectStatus.Connecting;

            if (SendCMD(new byte[] { XCPHelper.STD_CONNECT, (byte)connectMode }, out byte[] resData, canIndex) == XCPResponse.Ok)
            {
                ConnectStatus = XCPConnectStatus.Connected;
                //resource resData[1]

                /// resData[2]
                /// Common_mode_basic 
                /// 解析byteorder 
                XCPHelper.ParseCommModeBasic(resData[2], out this.byteOrder, out this.slaveBlockAvail);

                return true;
            }
            else
            {
                ConnectStatus = XCPConnectStatus.ConnectFail;
            }

            return false;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="canIndex"></param>
        /// <returns></returns>
        public bool DisConnect(uint canIndex = 0)
        {
            if (!CheckConnect())
            {
                return true;
            }

            if (SendCMD(new byte[] { 0xFE }, out _, canIndex) == XCPResponse.Ok)
            {
                ConnectStatus = XCPConnectStatus.DisConnect;
                return true;
            }

            return false;
        }

        public void ShortUpload(ref XCPSignal signal, uint canIndex = 0)
        {
            if (!CheckConnect())
            {
                throw new XCPException("XCP未连接！");
            }

            try
            {
                if (signal.Length > 6)
                    throw new XCPException("short upload 不支持长度大于 6 的数据!");
                if (signal.Length <= 0)
                    throw new XCPException("信号长度错误!");

                CurrentCMDStatus = XCPCMDStatus.ShortUpload;
                byte[] sendData = new byte[8];
                sendData[0] = XCPHelper.STD_SHORTUPLOAD;

                sendData[1] = (byte)signal.Length;
                sendData[2] = 0x00;//保留位
                sendData[3] = (byte)signal.AddressExtension;

                XCPHelper.TransformAddress(signal.ECUAddress, signal.ByteOrder, out byte[] addr);
                sendData[4] = addr[0];
                sendData[5] = addr[1];
                sendData[6] = addr[2];
                sendData[7] = addr[3];


                if (SendCMD(sendData, out byte[] resData, canIndex) == XCPResponse.Ok)
                {
                    CurrentCMDStatus = XCPCMDStatus.ShortUploadSucc;

                    byte[] valueByte = new byte[signal.Length];
                    for (int i = 0; i < signal.Length; i++)
                    {
                        valueByte[i] = resData[i + 2];//第三位开始才是数据
                    }
                    signal.StrValue = XCPHelper.DealData4Byte(signal, valueByte);
                }
                else
                {
                    CurrentCMDStatus = XCPCMDStatus.ShortUploadFail;
                }
            }
            catch (XCPException xcpEx)
            {
                throw xcpEx;
            }
        }

        public void Upload(ref XCPSignal signal, uint canIndex = 0)
        {
            if (!CheckConnect())
            {
                throw new XCPException("XCP未连接！");
            }

            //set mta
            if (Set_MTA(signal, canIndex) != XCPResponse.Ok)
            {
                CurrentCMDStatus = XCPCMDStatus.Set_MTAFail;
                return;
            }

            //upload
            CurrentCMDStatus = XCPCMDStatus.Upload;
            List<byte[]> res;
            if (signal.Length > 7)
            {
                if (SendLongCMD(new byte[] { XCPHelper.STD_UPLOAD, (byte)signal.Length }, out List<byte[]> resData, canIndex, (uint)(((signal.Length - 1) / 7) + 1)) == XCPResponse.Ok)
                {
                    CurrentCMDStatus = XCPCMDStatus.UploadSucc;
                    byte[] valueByte = new byte[signal.Length];
                    //resData.CopyTo(valueByte, 1);
                    signal.StrValue = XCPHelper.DealData4Byte(signal, valueByte);
                }
                else
                {
                    CurrentCMDStatus = XCPCMDStatus.UploadFail;
                }
            }
            else
            {
                if (SendCMD(new byte[] { XCPHelper.STD_UPLOAD, (byte)signal.Length }, out byte[] resData, canIndex) == XCPResponse.Ok)
                {
                    CurrentCMDStatus = XCPCMDStatus.UploadSucc;
                    byte[] valueByte = new byte[7];
                    resData.CopyTo(valueByte, 1);
                    signal.StrValue = XCPHelper.DealData4Byte(signal, valueByte);
                }
                else
                {
                    CurrentCMDStatus = XCPCMDStatus.UploadFail;
                }
            }

        }

        public void UnlockResource(uint resourceCode, uint canIndex = 0)
        {
            if (!CheckConnect())
            {
                throw new XCPException("XCP未连接！");
            }

            //get seed
            CurrentCMDStatus = XCPCMDStatus.GetSeed;
            //seed data
            List<byte> seeds = new List<byte>();
            if (SendCMD(new byte[] { XCPHelper.STD_GETSEED, 0x00, (byte)resourceCode }, out byte[] seedResp, canIndex) == XCPResponse.Ok)
            {
                if (seedResp[1] == 0)
                {
                    CurrentCMDStatus = XCPCMDStatus.UnlockSucc;
                    return;
                }

                for (int i = 2; i < seedResp.Length; i++)
                {
                    seeds.Add(seedResp[i]);
                }
                //seed 长度> CAN中最长字节数
                if (seedResp[1] > 6)
                {
                    do
                    {
                        if (SendCMD(new byte[] { XCPHelper.STD_GETSEED, 0x01 }, out seedResp, canIndex) == XCPResponse.Ok)
                        {
                            for (int i = 2; i < seedResp.Length; i++)
                            {
                                seeds.Add(seedResp[i]);
                            }
                        }
                        else
                        {
                            CurrentCMDStatus = XCPCMDStatus.GetSeedFail;
                            return;
                        }
                    } while (seedResp[1] > 6);
                }
            }
            else
            {
                CurrentCMDStatus = XCPCMDStatus.GetSeedFail;
                return;
            }

            //calculate key & send key to slave
            byte[] key = XCPHelper.CalKeyWithSeed(seeds);

            //send key 
            if (key.Length <= 6)
            {
                byte[] sendData = new byte[8];
                sendData[0] = XCPHelper.STD_SENDKEY;
                sendData[1] = (byte)key.Length;
                for (int i = 2; i < key.Length + 2; i++)
                {
                    sendData[i] = key[i - 2];
                }
                if (SendCMD(sendData, out _, canIndex) == XCPResponse.Ok)
                {
                    CurrentCMDStatus = XCPCMDStatus.UnlockSucc;
                    return;
                }
            }
            else
            {
                int remainingByte = key.Length;
                int sendCount = key.Length / 6 + 1;
                for (int i = 0; i < sendCount; i++)
                {
                    byte[] sendData = new byte[8];
                    sendData[0] = XCPHelper.STD_SENDKEY;
                    sendData[1] = (byte)remainingByte;
                    //剩余数据
                    for (int j = 0; j < (remainingByte / 6 > 0 ? 6 : remainingByte); j++)
                    {
                        sendData[j + 2] = key[i * 6 + j];
                    }
                    if (SendCMD(sendData, out _, canIndex) != XCPResponse.Ok)
                    {
                        CurrentCMDStatus = XCPCMDStatus.SendKeyFail;
                        return;
                    }
                    remainingByte -= 6;
                }

                CurrentCMDStatus = XCPCMDStatus.UnlockSucc;
            }
        }

        public void Download(XCPSignal signal, uint canIndex = 0)
        {
            if (!CheckConnect())
            {
                throw new XCPException("XCP未连接！");
            }
            //unlock download resource

            //set mta
            if (Set_MTA(signal, canIndex) != XCPResponse.Ok)
            {
                CurrentCMDStatus = XCPCMDStatus.Set_MTAFail;
                return;
            }

            //send download
            CurrentCMDStatus = XCPCMDStatus.DownLoad;
            //数值转换
            byte[] convertBytes = XCPHelper.ConvertToByte(signal.StrValue, signal.ValueType);
            byte[] sendByte = new byte[5];
            if (signal.Length > 4)
            {
                if (signal.ByteOrder == 0)
                {
                    sendByte[0] = convertBytes[0];
                    sendByte[1] = convertBytes[1];
                    sendByte[2] = convertBytes[2];
                    sendByte[3] = convertBytes[3];
                    sendByte[4] = convertBytes[4];
                }
                else if (signal.ByteOrder == 1)//Intel
                {
                    sendByte[0] = convertBytes[7];
                    sendByte[1] = convertBytes[6];
                    sendByte[2] = convertBytes[5];
                    sendByte[3] = convertBytes[4];
                    sendByte[4] = convertBytes[3];
                }
            }
            else
            {
                for (int i = 0; i < signal.Length; i++)
                {
                    //   sendByte[i] = ConvertByte[i];
                    if (signal.ByteOrder == 0)
                    {
                        sendByte[i] = convertBytes[i];
                    }
                    else if (signal.ByteOrder == 1)
                    {
                        sendByte[i] = convertBytes[3 - i];
                    }
                }
            }
            byte[] sendData = new byte[(signal.Length > 4 ? 5 : signal.Length) + 2];
            //CAL_Download
            sendData[0] = XCPHelper.CAL_DOWNLOAD;
            //data length
            sendData[1] = (byte)signal.Length;
            for (int i = 0; i < (signal.Length > 4 ? 5 : signal.Length); i++)
            {
                sendData[i + 2] = sendByte[i];
            }
            if (SendCMD(sendData, out _, canIndex) == XCPResponse.Ok)
                CurrentCMDStatus = XCPCMDStatus.DownLoadSucc;
            else
                CurrentCMDStatus = XCPCMDStatus.DownLoadFail;
        }

        private bool CheckConnect()
        {
            return ConnectStatus == XCPConnectStatus.Connected;
        }

        private XCPResponse Set_MTA(XCPSignal signal, uint canIndex = 0)
        {
            //地址转变
            XCPHelper.TransformAddress(signal.ECUAddress, signal.ByteOrder, out byte[] address);

            byte[] sendData = new byte[8];
            sendData[0] = XCPHelper.STD_SET_MTA;
            sendData[1] = sendData[2] = 0;
            sendData[3] = (byte)signal.AddressExtension;
            sendData[4] = address[0];
            sendData[5] = address[1];
            sendData[6] = address[2];
            sendData[7] = address[3];

            return SendCMD(sendData, out byte[] resData, canIndex);
        }

        private readonly object locker = new object();

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="data"></param>
        /// <param name="resData"></param>
        /// <param name="canIndex"></param>
        /// <returns></returns>
        private XCPResponse SendCMD(byte[] data, out byte[] resData, uint canIndex)
        {
            resData = new byte[1] { 0xFF };
            lock (locker)
            {
                try
                {
                    if (!receiveRunning)
                        this.startReceiveThread(canIndex);

                    CanSend(projectItem, (int)canIndex, new CANSendFrame((int)masterid, data), (byte)data.Length);

                    //等待
                    if (ReceiveEvent.WaitOne(1000))
                    {
                        resData = receiveData.Dequeue().b;
                        return XCPHelper.TransformBytetoRes(resData[0]);
                    }
                    else
                    {
                        return XCPResponse.Out_time;
                    }

                }
                catch (USBCANOpenException canEx)
                {
                    this.ConnectStatus = XCPConnectStatus.ConnectFail;
                    throw canEx;
                }
                catch (Exception ex)
                {
                    ShowLog("SendCMD", ex);
                    return XCPResponse.Out_time;
                }
                finally
                {
                    receiveRunning = false;
                }
            }

        }

        /// <summary>
        /// 发送命令，接收多条数据
        /// </summary>
        /// <param name="data">发送数据</param>
        /// <param name="resData">接收的数据</param>
        /// <param name="canIndex">调用的CAN通道号</param>
        /// <param name="receiveCount">接收次数</param>
        /// <returns></returns>
        private XCPResponse SendLongCMD(byte[] data, out List<byte[]> resData, uint canIndex, uint receiveCount)
        {
            resData = new List<byte[]>();

            lock (locker)
            {
                try
                {
                    if (!receiveRunning)
                        this.startReceiveThread(canIndex);

                    CanSend(projectItem, (int)canIndex, new CANSendFrame((int)masterid, data),  (byte)data.Length);

                    //等待
                    for (int i = 0; i < receiveCount; i++)
                    {
                        if (ReceiveEvent.WaitOne(1000))
                        {
                            resData.Add(receiveData.Dequeue().b);
                            if (XCPHelper.TransformBytetoRes(resData[0][0]) != XCPResponse.Ok)
                            {
                                return XCPResponse.Err;
                            }
                            //return XCPHelper.TransformBytetoRes(resData[0]);
                        }
                        else
                        {
                            return XCPResponse.Out_time;
                        }
                    }
                }
                catch (USBCANOpenException canEx)
                {
                    this.ConnectStatus = XCPConnectStatus.ConnectFail;
                    throw canEx;
                }
                catch (Exception ex)
                {
                    ShowLog("SendLongCMD", ex);
                    return XCPResponse.Out_time;
                }
                finally
                {
                    receiveRunning = false;
                }
            }

            return XCPResponse.Out_time;
        }

        private void Receive(object canindex)
        {
            int canIndex = Convert.ToInt32(canindex);
            do
            {
                try
                {
                    var data = CanRecieve(projectItem, new int[] { (int)this.slaveid }, canIndex);
                    for (uint i = 0; i < data.Length; ++i)
                    {
                        var can = data[i];
                        uint id = (uint)data[i].cid;

                        ///判断回应
                        if (GetId(id) == slaveid)
                        {
                            receiveData.Enqueue(can);
                            ReceiveEvent.Set();
                        }
                    }
                }
                catch (USBCANOpenException usncanEx)
                {
                    ShowLog("XCPModule.Recieve：Usb CAN　Error", usncanEx);
                    receiveRunning = false;
                }
                catch (Exception ex)
                {
                    ShowLog("XCPModule.Recieve", ex);
                }

            } while (receiveRunning);
        }

        private uint GetId(uint id)
        {
            return id & 0x1FFFFFFFU;
        }
        
        /// <summary>
        /// 显示日志事件
        /// </summary>
        public event Logger ShowLog;
        
        /// <summary>
        /// Can 接收事件
        /// </summary>
        public event CANRecieve CanRecieve;
        //Send(projectItem, (int)canIndex, new CAN_msg_byte((int)masterid, data), data_length: (byte)data.Length);
        
        /// <summary>
        /// Can 发送事件
        /// </summary>
        public event CANSend CanSend;
    }

    public class XCPException : Exception
    {
        public XCPException(string msg) : base(msg) { }
    }
    public delegate void Logger(string log, Exception ex);
    public delegate CANRecieveFrame[] CANRecieve(object CanKey, int[] Ids, int CanIndex);
    public delegate bool CANSend(object CanKey, int CanIndex, CANSendFrame sendFrame, byte dataLength);
}
