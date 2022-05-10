namespace CanControl.CANInfo
{
    #region 其他CAN函数数据定义
    //public struct PVCI_ERR_INFO
    //{
    //    //public uint ErrCode;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    //    //public byte[] Passive_ErrData;
    //    //public byte ArLost_ErrData;
    //}

    //5.定义初始化CAN的数据类型
    /// <summary>
    /// 初始化参数结构
    /// </summary>
    public struct INIT_CONFIG
    {
        public uint AccCode;
        public uint AccMask;
        /// <summary>
        /// 保留
        /// </summary>
        public uint Reserved;
        /// <summary>
        /// 滤波方式，1 表示单滤波，0 表示双滤波
        /// </summary>
        public char Filter;
        /// <summary>
        /// 用来设置波特率-设备类型为 PCI-5010-U、PCI-5020-U、USBCAN-E-U、 USBCAN-2E-U、USBCAN-4E-U、CANDTU 时，波特率和帧过滤不在这里设置
        /// </summary>
        public char Timing0;
        public char Timing1;
        /// <summary>
        /// 模式，0 表示正常模式，1 表示只听模式
        /// </summary>
        public char Mode;
    }

    #endregion
}

