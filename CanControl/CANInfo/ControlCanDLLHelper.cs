using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CanControl.CANInfo
{
    public class ControlCanDLLHelper
    {

        #region DLL连接
        /// <summary>
        /// 打开CAN口
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引</param>
        /// <param name="Reserved">保留参数，通常为0</param>
        /// <returns>1表示操作成功，0表示操作失败</returns>
        [DllImport("ControlCAN.dll", EntryPoint = "VCI_OpenDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern uint ZLG_VCI_OpenDevice(int DeviceType, int DeviceInd, int Reserved);
        /// <summary>
        /// 初始化Can通道，有多个CAN通道时，需要多次调用
        /// <remarks>
        ///<para>
        /// 当设备类型为 PCI-5010-U、PCI-5020-U、USBCAN-E-U、USBCAN-2E-U、USBCAN-4E-U 时，必须在调用此函数之前调用 VCI_SetReference 对波特率进行设置。
        ///</para> 
        /// </remarks>
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引</param>
        /// <param name="CANInd">Can口索引</param>
        /// <param name="pInitConfig"></param>
        /// <returns>为 1 表示操作成功，0 表示操作失败。</returns>
        [DllImport("ControlCAN.dll", EntryPoint = "VCI_InitCAN", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern uint ZLG_VCI_InitCAN(int DeviceType, int DeviceInd, int CANInd, INIT_CONFIG[] pInitConfig);

        //
        /// <summary>
        /// 调用接受设备动态连接库
        /// </summary>
        /// <param name="DevType">设备类型</param>
        /// <param name="DevIndex">设备索引</param>
        /// <param name="CANIndex">第几路 CAN。</param>
        /// <param name="pReceive">用来接收的帧结构体 VCI_CAN_OBJ 数组的首指针。</param>
        /// <param name="Len">用来接收的帧结构体数组的长度（本次接收的最大帧数，实际返回值小于等于这个值）。</param>
        /// <param name="WaitTime">缓冲区无数据，函数阻塞等待时间，以毫秒为单位。若为-1 则表示无超时，一直等待。</param>
        /// <returns>返回实际读取到的帧数。如果返回值为 0xFFFFFFFF，则表示读取数据失败，有错误发生，请调用 VCI_ReadErrInfo 函数来获取错误码。</returns>
        [DllImport("ControlCAN.dll", EntryPoint = "VCI_Receive", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern uint ZLG_VCI_Receive(int DevType, int DevIndex, int CANIndex, [Out] ZLG_PVCI_CAN_OBJ[] pReceive, uint Len, int WaitTime);
        //调用报错动态连接库
        //[DllImport("ControlCAN.dll", EntryPoint = "VCI_ReadErrInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public static extern int ZLG_VCI_ReadErrInfo(int DevType, int DevIndex, int CANIndex, PVCI_ERR_INFO pErrInfo);
        /*关闭设备连接库*/
        /// <summary>
        /// 关闭设备连接库
        /// </summary>
        /// <param name="DevType">设备类型</param>
        /// <param name="DevIndex">设备索引</param>
        /// <returns></returns>
        [DllImport("ControlCAN.dll", EntryPoint = "VCI_CloseDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ZLG_VCI_CloseDevice(int DevType, int DevIndex);
        /*CAN启动CAN连接库*/
        /// <summary>
        /// 此函数用以启动 CAN 卡的某一个 CAN 通道。有多个 CAN 通道时，需要多次调用
        /// </summary>
        /// <param name="DevType">设备类型</param>
        /// <param name="DevIndex">设备索引号，比如当只有一个 PCIe-9221 时，索引号为 0，这时再插入一个 PCIe-9221，那么后面插入的这个设备索引号就是 1，以此类推。</param>
        /// <param name="CANIndex">第几路 CAN。即对应卡的 CAN 通道号，CAN0 为 0，CAN1 为 1，以此类推。</param>
        /// <returns>为 1 表示操作成功，0 表示操作失败。</returns>
        [DllImport("ControlCAN.dll", EntryPoint = "VCI_StartCAN", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int ZLG_VCI_StartCAN(int DevType, int DevIndex, int CANIndex);
        /*CAN发送连接库*/
        /// <summary>
        /// CAN发送连接库
        /// </summary>
        /// <param name="DevType">设备类型</param>
        /// <param name="DevIndex">设备索引</param>
        /// <param name="CANIndex">第几路 CAN。</param>
        /// <param name="pSend">要发送的帧结构体 VCI_CAN_OBJ 数组的首指针。</param>
        /// <param name="Len">要发送的帧结构体数组的长度（发送的帧数量）</param>
        /// <returns>返回实际发送成功的帧数。</returns>
        [DllImport("ControlCAN.dll", EntryPoint = "VCI_Transmit", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern uint ZLG_VCI_Transmit(int DevType, int DevIndex, int CANIndex, ZLG_PVCI_CAN_OBJ[] pSend, uint Len);

        /// <summary>
        /// 此函数用以设置 CANET 与 PCI-5010-U/PCI-5020-U/USBCAN-E-U/ USBCAN-2E-U/USBCAN-4E-U/CANDTU/USBCAN-8E-U/CANDTU-NET 等设备的相应参数，
        /// 主要处理不同设备的特定操作
        /// </summary>
        /// <param name="DeviceType"></param>
        /// <param name="DeviceInd"></param>
        /// <param name="CANInd"></param>
        /// <param name="RefType">参数类型</param>
        /// <param name="pData">用来存储参数有关数据缓冲区地址首指针</param>
        /// <returns></returns>
        [DllImport("controlcan.dll", EntryPoint = "VCI_SetReference", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        public unsafe static extern uint ZLG_VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, byte* pData);

        //[DllImport("controlcan.dll")]
        //public static extern uint ZLG_VCI_ReadErrInfo(uint DevType,uint DeviceIndex,uint CanIndex,[Out] );
        /// <summary>
        /// 此函数用以获取指定 CAN 通道的接收缓冲区中，接收到但尚未被读取的帧数量。 
        /// 主要用途是配合 VCI_Receive 使用，即缓冲区有数据，再接收。
        /// 用户无需一直调用 VCI_Receive，可以节约 PC 系统资源，提高程序效率
        /// </summary>
        /// <param name="DeviceType"></param>
        /// <param name="DeviceInd"></param>
        /// <param name="CANInd"></param>
        /// <returns>返回尚未被读取的帧数</returns>
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);


        #endregion

        private static readonly Dictionary<BaudRateType, string> dictionary = new()
        {
            { BaudRateType.Kbps5, "0x1c01c1" },
            { BaudRateType.Kbps1000, "0x060003" },
            { BaudRateType.Kbps800, "0x060004" },
            { BaudRateType.Kbps500, "0x060007" },
            { BaudRateType.Kbps250, "0x1c0008" },
            { BaudRateType.Kbps125, "0x1c0011" },
            { BaudRateType.Kbps100, "0x160023" },
            { BaudRateType.Kbps50, "0x1c002c" },
            { BaudRateType.Kbps20, "0x1600b3" },
            { BaudRateType.Kbps10, "0x1c00e0" },
        };
        private static readonly Dictionary<BaudRateType, string> dictionary1 = new()
        {
            { BaudRateType.Kbps5, "0xBF" },
            { BaudRateType.Kbps10, "0x31" },
            { BaudRateType.Kbps20, "0x18" },
            //{ BaudRateType.Kbps40,"0x87"},
            { BaudRateType.Kbps50, "0x09" },
            { BaudRateType.Kbps100, "0x04" },
            { BaudRateType.Kbps125, "0x03" },
            { BaudRateType.Kbps250, "0x01" },
            { BaudRateType.Kbps500, "0x00" },
            { BaudRateType.Kbps800, "0x00" },
            { BaudRateType.Kbps1000, "0x00" },

        };
        private static readonly Dictionary<BaudRateType, string> dictionary2 = new()
        {
            { BaudRateType.Kbps5, "0xFF" },
            { BaudRateType.Kbps10, "0x1C" },
            { BaudRateType.Kbps20, "0x1C" },
            //{ BaudRateType.Kbps40,"0x87"},
            { BaudRateType.Kbps50, "0x1C" },
            { BaudRateType.Kbps100, "0x1C" },
            { BaudRateType.Kbps125, "0x1C" },
            { BaudRateType.Kbps250, "0x1C" },
            { BaudRateType.Kbps500, "0x1C" },
            { BaudRateType.Kbps800, "0x16" },
            { BaudRateType.Kbps1000, "0x14" }

        };
        /// <summary>
        /// 2e-u等设备的波特率
        /// </summary>
        public static Dictionary<BaudRateType, string> BaudRateTypeValue_Special = dictionary;
        /// <summary>
        /// USBCAN-I等设备的波特率：Timer0
        /// </summary>
        public static Dictionary<BaudRateType, string> BaudRateTypeValue_Normal_Timer0 = dictionary1;
        /// <summary>
        /// USBCAN-I等设备的波特率：Timer1
        /// </summary>
        public static Dictionary<BaudRateType, string> BaudRateTypeValue_Normal_Timer1 = dictionary2;
    }

}

