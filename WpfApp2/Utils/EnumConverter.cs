using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp2
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum enumValue = default(Enum);
            if (parameter is Type)
            {
                enumValue = (Enum)Enum.Parse((Type)parameter, value.ToString());
            }
            return enumValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int returnValue = 0;
            if (parameter is Type)
            {
                returnValue = (int)Enum.Parse((Type)parameter, value.ToString());
            }
            return returnValue;
        }
    }

    public enum FormType
    {
        /// <summary>
        /// 示波   
        /// </summary>
        Scope,

        Get,

        Set,

        RollingCounter
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
}
