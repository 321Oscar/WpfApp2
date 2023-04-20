using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp2.Model;
using WpfApp2.Utils;

namespace WpfApp2.View
{
    /// <summary>
    /// AddProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddProjectWindow : Window
    {
        public AddProjectWindow()
        {
            InitializeComponent();
            ProjectViewModel pv = new(new ProjectItem());
            this.DataContext = pv;
            pv.Init(cbbCanDevice, cbbCanBaud, cbbCanProtoType, cbbCanChannel);
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            ((ProjectViewModel)DataContext).SaveProject();
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Button_SelectFile_Click(object sender, RoutedEventArgs e)
        {
            ((ProjectViewModel)DataContext).SelectFile();
        }

        private void Button_SaveChannelConfig_Click(object sender, RoutedEventArgs e)
        {
            ((ProjectViewModel)DataContext).SaveChannelConfig();
        }
    }
}
