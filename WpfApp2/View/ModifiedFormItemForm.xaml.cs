using ProtocolLib.Signal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// ModifiedFormItemForm.xaml 的交互逻辑
    /// </summary>
    public partial class ModifiedFormItemForm : Window
    {
        public ModifiedFormItemForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 新增or修改
        /// </summary>
        /// <param name="projectItem"></param>
        /// <param name="formItem"></param>
        /// <param name="isnew"></param>
        /// <param name="modified"></param>
        public ModifiedFormItemForm(ProjectItem projectItem, FormItem formItem, bool modified) : this()
        {
            this.DataContext = new FormItemViewModel(projectItem, formItem, modified);
            //load combobox

            ((FormItemViewModel)DataContext).Init(cbbCanIndex, cbbFormType);

            CollectionViewSource signals = (CollectionViewSource)this.FindResource("signalCollectionViewSource");
            signals.Source = ((FormItemViewModel)DataContext).AllSignals;
        }

        /// <summary>
        /// 新增Form
        /// </summary>
        /// <param name="projectItem"></param>
        /// <param name="formItem"></param>
        /// <param name="isnew"></param>
        public ModifiedFormItemForm(ProjectItem projectItem) : this(projectItem, new FormItem(), true) { }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            //cb.TemplatedParent.GetValue()
        }

        /// <summary>
        /// 关键字改变时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEmployeeKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtEmployeeKeyword = sender as TextBox;
            string keyword = txtEmployeeKeyword.Text.Trim();
            if (string.IsNullOrEmpty(keyword))//无关键字，显示scv1下的listbox(有分组)
            {
                signallb1.Visibility = Visibility.Visible;
                signallbQ.Visibility = Visibility.Collapsed;
            }
            else//有关键字，显示scv2下的listbox(无分组)
            {
                signallb1.Visibility = Visibility.Collapsed;
                signallbQ.Visibility = Visibility.Visible;
            }
            CollectionViewSource employeeCvs = (CollectionViewSource)this.FindResource("signalCollectionViewSource");
            employeeCvs.View.Refresh();//刷新View
        }

        /// <summary>
        /// 根据关键字(工号或姓名)筛选员工
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void employeeCollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            string keyword = tbQuery.Text.Trim().ToUpper();
            DBCSignal signal = e.Item as DBCSignal;
            if (signal != null)
            {
                if (string.IsNullOrEmpty(keyword))//无关键字，直接Accept
                {
                    e.Accepted = true;
                }
                else
                {
                    //有关键字、筛选员工号或姓名中包含关键字的员工
                    e.Accepted = signal.MessageID.ToUpper(null).Contains(keyword) || signal.SignalName.ToUpper(null).Contains(keyword);
                }
            }
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            ((FormItemViewModel)DataContext).BtnOk();
            this.DialogResult = true;
            this.Close();
        }

        private void BTNcancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
