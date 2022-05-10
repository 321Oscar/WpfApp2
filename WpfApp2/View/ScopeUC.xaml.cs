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
    /// ScopeUC.xaml 的交互逻辑
    /// </summary>
    public partial class ScopeUC : UserControl
    {
        private readonly List<ChartData> data;
        public ScopeUC()
        {
            InitializeComponent();

            data = new()
            {
                new ChartData { Date = DateTime.Now.Date.AddDays(-15), Total = 121, PassRate = .84 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-14), Total = 88, PassRate = .92 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-13), Total = 180, PassRate = .35 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-12), Total = 150, PassRate = .46 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-11), Total = 78, PassRate = .58 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-10), Total = 99, PassRate = .71 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-9), Total = 143, PassRate = .81 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-8), Total = 56, PassRate = .85 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-7), Total = 108, PassRate = .95 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-6), Total = 79, PassRate = .78 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-5), Total = 63, PassRate = .65 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-4), Total = 157, PassRate = .58 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-3), Total = 148, PassRate = .36 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-2), Total = 115, PassRate = .48 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-1), Total = 89, PassRate = .63 },
            };

            DataContext = new ScopeViewModel(data);

            _ = Task.Run(new Action(() =>
            {
                int count = 0;
                while (count < 1000)
                {
                    data.Add(new ChartData() { Date = DateTime.Now.Date.AddDays(-1), Total = 89, PassRate = .63 });
                    count++;
                    Thread.Sleep(10);
                }

            }));
        }
    }
}
