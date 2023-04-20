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
using WpfApp2.Model;
using WpfApp2.Utils;

namespace WpfApp2.View
{
    /// <summary>
    /// ScopeUC.xaml 的交互逻辑
    /// </summary>
    public partial class ScopeUC : UserControl
    {
        private List<int> points = new List<int>() { 4,4,3,-1,-2,-2,-2,-2,-2,-2,-2,-4,-3,25,37,8,23,4,50,-6,54,20,50,4,2,5,2,54,45,24,45,24,5,25,45,2,4,-3,5,2,52,34,2,35,60,2,1,34};
        private bool flag = true;
        private int currentIndex = 0;

        public ScopeUC(ProjectItem pItem,FormItem item)
        {
            InitializeComponent();

            DataContext = new ScopeViewModel(pItem, item);

            new Thread(() =>
            {
                while (flag)
                {
                    Thread.Sleep(10);
                    _ = Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (currentIndex == points.Count)
                        {
                            currentIndex = 0;
                        }

                        ecgDrawingVisual.SetupData(points[currentIndex]);
                        currentIndex++;
                    }));
                }
            }).Start();
        }

        public ScopeUC()
        {
            InitializeComponent();
        }

        private void btnSelectColor_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ScopeViewModel;
            await vm.StartOrStopGet();
        }

        private void btnChangeSignals_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ScopeViewModel;
            vm.ModifiedSignals();
        }

        //override close
    }
}
