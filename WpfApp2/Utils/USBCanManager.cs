using CanControl.CANInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Model;

namespace WpfApp2.Utils
{
    public sealed class USBCanManager
    {
        private static readonly Lazy<USBCanManager> lazy =
            new Lazy<USBCanManager>(() => new USBCanManager());
      
        public static USBCanManager Instance { get { return lazy.Value; } }

        private USBCanManager()
        {
            usbCans = new Dictionary<ProjectItem, UsbCan>();
        }

        private readonly Dictionary<ProjectItem, UsbCan> usbCans;

        public bool Exist(ProjectItem project)
        {
            return usbCans.TryGetValue(project, out _);
        }

        public void AddUsbCan(UsbCan usbCan, ProjectItem project)
        {
            usbCans.Add(project, usbCan);
        }

        /// <summary>
        /// 删除CAN
        /// </summary>
        /// <param name="project"></param>
        /// <param name="isOpen">若CAN打开，则先关闭再删除</param>
        public void RemoveUsbCan(ProjectItem project, bool isOpen = true)
        {
            bool isExist = usbCans.TryGetValue(project, out UsbCan usbCan);
            if (isExist)
            {
                if (isOpen)
                {
                    try
                    {
                        usbCan.close_device();
                    }
                    catch (Exception ex)
                    {
                        //LogHelper.Error("关闭失败", ex);
                    }
                }
                usbCans.Remove(project);
            }
        }

        public bool Open(ProjectItem project)
        {
            bool isExist = usbCans.TryGetValue(project, out UsbCan usbCan);
            if (isExist)
            {
                if (usbCan.Open_Device() == 0)
                {
                    usbCan.IsOpen = false;
                    return false;
                }
                else
                {
                    usbCan.IsOpen = true;
                }

            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 启动接收数据
        /// </summary>
        /// <param name="project"></param>
        /// <param name="canind"></param>
        public void StartRecv(ProjectItem project, int[] canind)
        {
            bool isExist = usbCans.TryGetValue(project, out UsbCan usbCan);
            if (isExist)
            {
                usbCan.StartReceive(true, canind);
            }
        }

        /// <summary>
        /// 关闭接收数据
        /// </summary>
        /// <param name="project"></param>
        public void CloseRecv(ProjectItem project)
        {
            bool isExist = usbCans.TryGetValue(project, out UsbCan usbCan);
            if (isExist)
            {
                usbCan.StartReceive(false, new int[] { 0 });
            }

        }

        public bool InitCan(ProjectItem project, int can_index)
        {
            bool isExist = usbCans.TryGetValue(project, out UsbCan usbCan);

            bool canGetBaud = ControlCanDLLHelper.BaudRateTypeValue_Normal_Timer0.TryGetValue((BaudRateType)project.CanIndex[can_index].BaudRate, out string baudStr0);
            bool canGetBaud1 = ControlCanDLLHelper.BaudRateTypeValue_Normal_Timer1.TryGetValue((BaudRateType)project.CanIndex[can_index].BaudRate, out string baudStr1);
            if (canGetBaud && canGetBaud1)
            {
                int timer0 = Convert.ToInt32(baudStr0, 16);
                int timer1 = Convert.ToInt32(baudStr1, 16);
                if (isExist)
                {
                    if (usbCan.Config((char)0, can_index, (char)timer0, Timing1: (char)timer1) == 0)
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new Exception("未知波特率，波特率转换错误");
            }

            return true;
        }

        public bool StartCanIndex(ProjectItem project, int can_index)
        {
            bool isExist = usbCans.TryGetValue(project, out UsbCan usbCan);
            if (isExist)
            {
                if (usbCan.start_can(can_index) == 0)
                    return false;
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置CAN通道波特率
        /// </summary>
        /// <param name="project"></param>
        /// <param name="can_index"></param>
        /// <returns></returns>
        unsafe public bool SetRefenrece(ProjectItem project, int can_index)
        {
            bool isExist = usbCans.TryGetValue(project, out UsbCan usbCan);

            bool canGetBaud = ControlCanDLLHelper.BaudRateTypeValue_Special.TryGetValue((BaudRateType)project.CanIndex[can_index].BaudRate, out string baudStr);
            uint baud;
            if (canGetBaud)
            {
                baud = Convert.ToUInt32(baudStr, 16);
            }
            else
            {
                throw new Exception("未知波特率");
            }
            if (isExist)
            {
                return usbCan.SetReference(baud, can_index);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="project">can信息</param>
        /// <param name="canindex">can口通道索引：默认为0</param>
        /// <param name="formName">接收的窗口</param>
        /// <returns><see cref="CANRecieveFrame"/>[]</returns>
        public CANRecieveFrame[] Receive(ProjectItem project, int canindex = 0, string formName = null)
        {
            bool isadd = usbCans.TryGetValue(project, out UsbCan usbCan);

            if (!isadd)
                throw new USBCANOpenException($"{DateTime.Now}  {project.Name},Can口未打开");

            if (!usbCan.IsOpen)
                throw new USBCANOpenException($"{DateTime.Now}  {project.Name},Can口为关闭状态");

            var data = usbCan.CAN_Receive(canindex);
            for (int i = 0; i < data.Length; i++)
            {
                string log = data[i].ToString();
                //LogHelper.WriteToOutput(formName, $"通道[{canindex}] [{data[i].timeStamp:yyyy-MM-dd HH:mm:ss fff}] 接收：" + log);
            }

            return data;
        }

        /// <summary>
        /// 接收特定ID数据
        /// </summary>
        /// <param name="project">can信息</param>
        /// <param name="ids">ID</param>
        /// <param name="canindex">can口通道索引：默认为0</param>
        /// <param name="formName">接收的窗口</param>
        /// <returns><see cref="CANRecieveFrame"/>[]</returns>
        public CANRecieveFrame[] Receive(ProjectItem project, int[] ids, int canindex = 0, string formName = null)
        {
            bool isadd = usbCans.TryGetValue(project, out UsbCan usbCan);

            if (!isadd)
                throw new USBCANOpenException($"{project.Name},Can口未打开");

            if (!usbCan.IsOpen)
                throw new USBCANOpenException($"{project.Name},Can口为关闭状态");

            var data = usbCan.Receive(canindex, ids);
            if (data == null)
                return null;
            for (int i = 0; i < data.Length; i++)
            {
                string log = data[i].ToString();
                //LogHelper.WriteToOutput(formName, $"通道[{canindex}] [{data[i].timeStamp:yyyy-MM-dd HH:mm:ss fff}] 接收：" + log);
            }

            return data;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="project">根据Project获取CanDevice</param>
        /// <param name="canindex">发送的CAN通道</param>
        /// <param name="sendData">数据</param>
        /// <param name="formName">发送的窗口</param>
        /// <returns>发送成功/失败<see cref="bool"/></returns>
        internal bool Send(ProjectItem project, int canindex = 0, CANSendFrame sendData = null, string formName = null, byte sendtype = 2)
        {
            bool isadd = usbCans.TryGetValue(project, out UsbCan usbCan);

            if (!isadd)
                throw new USBCANOpenException($"{project.Name},Can口未打开");

            if (!usbCan.IsOpen)
                throw new USBCANOpenException($"{project.Name},Can口为关闭状态");

            bool sendFlag = usbCan.CAN_Send(sendData, 0, canindex, sendtype) > 0;

            //LogHelper.InfoSend($"{formName} 发送{(sendFlag ? "成功" : "失败")} [通道{canindex}]：{sendData}");

            if (!sendFlag)
                throw new USBCANOpenException("发送失败！");
            return sendFlag;
        }

        internal bool Close(ProjectItem project)
        {
            bool isExist = usbCans.TryGetValue(project, out UsbCan usbCan);
            if (isExist)
            {
                CloseRecv(project);

                return usbCan.close_device();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 关闭所有Can口
        /// </summary>
        internal void Close()
        {
            try
            {
                foreach (var item in usbCans)
                {
                    if (!item.Value.close_device())
                    {
                        //LogHelper.Warn($"关闭失败：{item.Key.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Error("关闭失败", ex);
            }

        }
    }
}
