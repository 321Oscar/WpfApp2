using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanControl.CANInfo
{
    public enum BaudRateType
    {
        [Description("1000Kbps")]
        Kbps1000 = 0x060003,
        [Description("800Kbps")]
        Kbps800 = 0x060004,
        [Description("500Kbps")]
        Kbps500 = 0x060007,
        [Description("250Kbps")]
        Kbps250 = 0x1C0008,
        [Description("125Kbps")]
        Kbps125 = 0x1C0011,
        [Description("100Kbps")]
        Kbps100 = 0x160023,
        [Description("50Kbps")]
        Kbps50 = 0x1C002C,
        [Description("20Kbps")]
        Kbps20 = 0x1600B3,
        [Description("10Kbps")]
        Kbps10 = 0x1C00E0,
        [Description("5Kbps")]
        Kbps5 = 0x1C01C1,

    }

    public enum DeviceType
    {
        //VCI_PCI5121 = 1,
        //VCI_PCI9810 = 2,
        VCI_USBCAN1 = 3,
        //VCI_USBCAN2 = 4,
        //VCI_USBCAN2A = 4,
        //VCI_PCI9820 = 5,
        //VCI_CAN232 = 6,
        //VCI_PCI5110 = 7,
        //VCI_CANLITE = 8,
        //VCI_ISA9620 = 9,
        //VCI_ISA5420 = 10,
        //VCI_PC104CAN = 11,
        //VCI_CANETUDP = 12,
        //VCI_CANETE = 12,
        //VCI_DNP9810 = 13,
        //VCI_PCI9840 = 14,
        //VCI_PC104CAN2 = 15,
        //VCI_PCI9820I = 16,
        //VCI_CANETTCP = 17,
        //VCI_PEC9920 = 18,
        //VCI_PCI5010U = 19,
        //VCI_USBCAN_E_U = 20,
        VCI_USBCAN_2E_U = 21,
        //VCI_PCI5020U = 22,
        //VCI_EG20T_CAN = 23
    }

    public enum CANSendType
    {
        /// <summary>
        /// 正常发送
        /// </summary>
        Normal,
        /// <summary>
        /// 单次发送
        /// </summary>
        SingleSend,
        /// <summary>
        /// 自发自收
        /// </summary>
        Self,
        /// <summary>
        /// 单次自发自收
        /// </summary>
        SingleSelf
    }
}
