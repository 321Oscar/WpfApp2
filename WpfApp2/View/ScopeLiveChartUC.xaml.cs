using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.Utils;

namespace WpfApp2.View
{
    /// <summary>
    /// ScopeLiveChartUC.xaml 的交互逻辑
    /// </summary>
    public partial class ScopeLiveChartUC : UserControl
    {
        public ScopeLiveChartUC()
        {
            InitializeComponent();

            DataContext = new ModeLvc();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((ModeLvc)DataContext).OnClick();
        }
    }

    public class ModeLvc : ViewModelLvcBase
    {
        public SeriesCollection LineSeriesCollection { get; set; }

        private double axisXMax;
        public double AxisXMax
        {
            get => axisXMax;
            set { SetProperty(ref axisXMax, value); }
        }

        private double axisXMin;
        public double AxisXMin
        {
            get => axisXMin;
            set { SetProperty(ref axisXMin, value); }
        }

        private double axisYMax;
        public double AxisYMax
        {
            get => axisYMax;
            set { SetProperty(ref axisYMax, value); }
        }


        private double axisYMin;
        public double AxisYMin
        {
            get => axisYMin;
            set { SetProperty(ref axisYMin, value); }
        }

        private Random Randoms = new Random();

        public Func<double, string> CustomFormatterX { get; set; }
        public Func<double, string> CustomFormatterY { get; set; }

        private ChartValues<double> ValueList { get; set; }

        private int TableShowCount = 2000;

        private string CustomFormattersX(double val)
        {
            return string.Format("{0}", val);
        }

        private string CustomFormattersY(double val)
        {
            return string.Format("{0}", val);
        }

        private bool isgetdata = false;
        public void OnClick()
        {
            if (!isgetdata)
            {
                isgetdata = true;
                ValueList.Clear();
                Task.Run(new Action(() =>{

                    while (isgetdata)
                    {
                        int yValue = Randoms.Next(2, 1000);
                        Application.Current.Dispatcher.Invoke(()=> {
                            if (ValueList.Count > TableShowCount)
                            {
                                ValueList.RemoveAt(0);
                            }
                            ValueList.Add(yValue);
                        });
                        //ValueList.Add(yValue);

                        //int maxY = (int)ValueList.Max();
                        //AxisYMax = maxY + 30;

                        //if (ValueList.Count > TableShowCount)
                        //{
                        //    AxisXMax = ValueList.Count - 1;
                        //    AxisXMin = ValueList.Count - TableShowCount;
                        //}

                        //if(ValueList.Count > TableShowCount)
                        //{
                        //    ValueList.RemoveAt(0);
                        //}

                        Thread.Sleep(10);
                    }
                }));
            }
            else
            {
                isgetdata = false;
            }
            
        }

        public ModeLvc()
        {
            TableShowCount = 100;
            AxisXMax = TableShowCount;
            AxisXMin = 0;
            AxisYMax = 10;
            AxisYMin = 0;

            ValueList = new ChartValues<double>();
            LineSeriesCollection = new SeriesCollection();

            CustomFormatterX = CustomFormattersX;
            CustomFormatterY = CustomFormattersY;

            LineSeries lineSeries = new LineSeries();
            lineSeries.DataLabels = false;
            lineSeries.PointGeometry = null;
            lineSeries.Values = ValueList;//ValueList; new ChartValues<double>();
            LineSeriesCollection.Add(lineSeries);
        }
    }
}
