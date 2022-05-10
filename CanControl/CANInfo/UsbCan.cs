using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CanControl.CANInfo
{
    public class UsbCan
    {
        private int DeviceType;
        private int DeviceInd;
        private int[] CANInd;
        INIT_CONFIG InitConfig;
        //PVCI_ERR_INFO pErrInfo;
        //private uint RefType;

        public bool IsOpen = false;

        /// <summary>
        /// USBCAN口
        /// </summary>
        /// <param name="Device_Type">设备类型</param>
        /// <param name="Device_Ind">设备索引</param>
        /// <param name="CAN_Ind">CAN口索引</param>
        public UsbCan(int Device_Type, int Device_Ind, int[] CAN_Ind)
        {
            DeviceType = Device_Type;
            DeviceInd = Device_Ind;
            CANInd = CAN_Ind;
        }

        /// <summary>
        /// USBCAN口
        /// </summary>
        /// <param name="Device_Type">设备类型</param>
        /// <param name="Device_Ind">设备索引</param>
        /// <param name="CAN_Ind">CAN口索引</param>
        public UsbCan(int Device_Type, int Device_Ind, int CAN_Ind)
        {
            DeviceType = Device_Type;
            DeviceInd = Device_Ind;
            CANInd = new int[] { CAN_Ind };
        }

        /// <summary>
        /// 配置Can口
        /// </summary>
        /// <param name="AccCode"></param>
        /// <param name="AccMask"></param>
        /// <param name="Reserved"></param>
        /// <param name="Filter"></param>
        /// <param name="Timing0">设置波特率</param>
        /// <param name="Timing1"></param>
        /// <param name="Mode"></param>
        /// <param name="canIndex">第几路CAN</param>
        /// <returns></returns>
        public int Config(char Mode, int canIndex, char Timing0, uint AccCode = 0, uint AccMask = 0xffffffff, uint Reserved = 0,
            char Filter = (char)1, char Timing1 = (char)28)
        {
            InitConfig = new INIT_CONFIG();
            InitConfig.AccCode = AccCode;
            InitConfig.AccMask = AccMask;
            InitConfig.Reserved = Reserved;
            InitConfig.Filter = Filter;
            InitConfig.Timing0 = Timing0;
            InitConfig.Timing1 = Timing1;
            InitConfig.Mode = Mode;

            INIT_CONFIG[] cfg = new INIT_CONFIG[1];
            cfg[0] = InitConfig;

            return (int)ControlCanDLLHelper.ZLG_VCI_InitCAN(DeviceType, DeviceInd, canIndex, cfg);
        }

        public uint Open_Device()
        {
            return ControlCanDLLHelper.ZLG_VCI_OpenDevice(DeviceType, DeviceInd, 0);
        }

        public bool close_device()
        {
            IsOpen = false;
            return ControlCanDLLHelper.ZLG_VCI_CloseDevice(DeviceType, DeviceInd);
        }

        public int start_can(int canindex = 0)
        {
            return ControlCanDLLHelper.ZLG_VCI_StartCAN(DeviceType, DeviceInd, canindex);
        }

        /// <summary>
        /// 设置波特率（个别设备）
        /// </summary>
        /// <param name="baud"></param>
        /// <param name="canIndex"></param>
        /// <returns></returns>
        unsafe public bool SetReference(uint baud, int canIndex = 0)
        {
            return ControlCanDLLHelper.ZLG_VCI_SetReference((uint)DeviceType, (uint)DeviceInd, (uint)canIndex, 0, (byte*)&baud) == 1;
        }

        /// <summary>
        /// 发送Can口数据
        /// </summary>
        /// <param name="msg">数据</param>
        /// <param name="extframe">数据帧类型</param>
        /// <param name="canindex">Can通道</param>
        /// <param name="sendType">发送类型<see cref="CANSendType"/></param>
        /// <returns></returns>
        public int CAN_Send(CANSendFrame msg, int extframe, int canindex = 0, byte sendType = 2)
        {

            ZLG_PVCI_CAN_OBJ obj = new ZLG_PVCI_CAN_OBJ();

            obj.DataLen = 8;
            obj.SendType = sendType;
            obj.RemoteFlag = 0;
            obj.ExternFlag = (byte)extframe;
            obj.ID = (uint)msg.cid;
            obj.data = new byte[8];

            obj.data[0] = msg.w[0];
            obj.data[1] = msg.w[1];
            obj.data[2] = msg.w[2];
            obj.data[3] = msg.w[3];
            obj.data[4] = msg.w[4];
            obj.data[5] = msg.w[5];
            obj.data[6] = msg.w[6];
            obj.data[7] = msg.w[7];

            ZLG_PVCI_CAN_OBJ[] cans = new ZLG_PVCI_CAN_OBJ[1];
            cans[0] = obj;


            return (int)ControlCanDLLHelper.ZLG_VCI_Transmit(DevType: DeviceType,
                                         DevIndex: DeviceInd,
                                         CANIndex: canindex,
                                         pSend: cans,
                                         Len: 1);
        }

        public CANRecieveFrame[] CAN_Receive(int canindex = 0)
        {
            uint res = ControlCanDLLHelper.VCI_GetReceiveNum((uint)DeviceType, (uint)DeviceInd, (uint)canindex);

            ZLG_PVCI_CAN_OBJ[] rcvbuf = new ZLG_PVCI_CAN_OBJ[res];
            CANRecieveFrame[] can_mail = new CANRecieveFrame[res];

            if (res == 0)
                return can_mail;

            int NumValue = (int)ControlCanDLLHelper.ZLG_VCI_Receive(DevType: DeviceType,
                                DevIndex: DeviceInd,
                                CANIndex: canindex,
                                pReceive: rcvbuf,
                                Len: (uint)rcvbuf.Length,
                                WaitTime: 5);

            if ((NumValue < 0) || (NumValue > res))
                return null;


            //for (int i = 0; i < NumValue && i < 200; i++)
            for (int i = 0; i < NumValue; i++)
            {
                can_mail[i] = new CANRecieveFrame((int)rcvbuf[i].ID, rcvbuf[i].data, rcvbuf[i].TimeStamp);
            }

            return can_mail;
        }

        /// <summary>
        /// 接收数据，按照can索引分组
        /// </summary>
        public List<List<CANRecieveFrame>> ReceiveData;

        /// <summary>
        /// 接收数据线程
        /// </summary>
        Thread recv_thread;

        /// <summary>
        /// 是否启动接收数据
        /// </summary>
        bool recv_start;

        static readonly object locker = new object();

        /// <summary>
        /// 启动/关闭接收数据线程
        /// </summary>
        /// <param name="start">启动/关闭</param>
        /// <param name="canind">接收数据的can索引</param>
        public void StartReceive(bool start, int[] canind)
        {
            this.CANInd = canind;
            var canids = CANInd.ToList();
            canids.Sort();
            CANInd = canids.ToArray();
            recv_start = start;
            if (start)
            {
                recv_thread = new Thread(RecvDataFunc);
                recv_thread.IsBackground = true;
                recv_thread.Start();
            }
            else
            {
                if (recv_thread != null)
                    recv_thread.Join();
                recv_thread = null;
                ReceiveData = null;
            }
        }

        /// <summary>
        /// 接收数据的最大存储数量，默认100000
        /// </summary>
        public int MaxDataCount { get; set; } = 100000;

        /// <summary>
        /// 接收数据循环
        /// </summary>
        private void RecvDataFunc()
        {
            while (recv_start)
            {
                lock (locker)
                {
                    if (ReceiveData == null)
                    {

                        ReceiveData = new List<List<CANRecieveFrame>>();
                        for (int i = 0; i < CANInd.Length; i++)
                            ReceiveData.Add(new List<CANRecieveFrame>());
                    }

                    for (int i = 0; i < CANInd.Length; i++)
                    {
                        var recv = this.CAN_Receive(CANInd[i]);

                        for (int j = 0; j < recv.Length; j++)
                        {
                            if (ReceiveData[i].Count > MaxDataCount)
                            {
                                ReceiveData[i].RemoveAt(0);
                            }
                            ReceiveData[i].Add(recv[j]);
                        }

                    }
                }

                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 根据ID接收数据
        /// </summary>
        /// <param name="canIndex"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public CANRecieveFrame[] Receive(int canIndex, int[] ids)
        {
            List<CANRecieveFrame> data = new List<CANRecieveFrame>();

            string idstr = string.Empty;
            for (int i = 0; i < ids.Length; i++)
            {
                idstr += $"{ids[i]},";
            }

            try
            {
                List<CANRecieveFrame> alldata = new List<CANRecieveFrame>(ReceiveData[canIndex]);

                if (alldata == null || alldata.Count == 0)
                    return data.ToArray();
                data = alldata.Where(x => x != null && idstr.Contains(x.cid.ToString())).ToList();
                data.Sort();
                if (data.Count > 0)
                {
                    int removeCount = ReceiveData[canIndex].RemoveAll(x => x != null && idstr.Contains(x.cid.ToString()));
                    //LogHelper.InfoReceive($"接收：{data.Count}。删除：{removeCount}");
                }
                else
                {
                    //LogHelper.InfoReceive($"没有接收到目标ID数据帧{ids:X}");
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Error("Receive", ex);
            }

            return data.ToArray();

        }


    }

}

