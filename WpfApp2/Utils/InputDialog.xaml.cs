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

namespace WpfApp2.Utils
{
    /// <summary>
    /// InputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InputDialog : Window
    {
        public InputDialog(string question,string defaultAnswer = "")
        {
            InitializeComponent();
            lbQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            _ = txtAnswer.Focus();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Answer => txtAnswer.Text;
    }
}
