using System.Runtime.InteropServices;

namespace CanControl.CANInfo
{
    #region CAN接受和发送数据定义
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ZLG_PVCI_CAN_OBJ
    {
        public uint ID;
        /// <summary>
        /// 时间戳：时间标示从 CAN 卡上电开始计时，计时单位为 0.1ms
        /// </summary>
        public uint TimeStamp;
        /// <summary>
        /// 是否使用时间标识。为 1 时 TimeStamp 有效， TimeFlag 和 TimeStamp 只在此帧为接收帧时有意义。
        /// </summary>
        public byte TimeFlag;
        public byte SendType;
        public byte RemoteFlag;
        public byte ExternFlag;
        public byte DataLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Reserved;
    }

    #endregion
}

