using ProtocolLib.Signal;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp2.Utils
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.Black;
            if(value is BaseSignal)
            {
                BaseSignal signal = value as BaseSignal;

                if ((signal.DValue > signal.Maximum) || signal.DValue < signal.Minimum)
                    return Brushes.Red;
                else
                {
                    return Brushes.Black;
                }
            }
            return Brushes.Black;
        }

       

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}

        //public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class MultipColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
                return Brushes.Black;
            else
            {
                double dval = (double)values[0];
                double dmin = (double)values[1];
                double dmax = (double)values[2];
                if (dval > dmax)
                    return Brushes.Red;
                if(dval < dmin)
                    return Brushes.Red;
                return Brushes.Black;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
