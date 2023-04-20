using ProtocolLib.Signal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// DataUC.xaml 的交互逻辑
    /// </summary>
    public partial class BaseDataUC : UserControl
    {
        /// <summary>
        /// 数据窗口：Get/Set
        /// </summary>
        public BaseDataUC()
        {
            InitializeComponent();
        }

        public BaseDataUC(ProjectItem project, FormItem formItem) : this()
        {
            var dc = new BaseSignalViewModel(project, formItem);
            dc.EPrintLog += LogInDockHelper.PrintLogFuncs[project.Name];
            this.DataContext = dc;
            setTab.Visibility = formItem.FormType == (int)FormType.Set ? Visibility.Visible: Visibility.Collapsed;
            gbGet.Visibility = formItem.FormType == (int)FormType.Get ? Visibility.Visible : Visibility.Collapsed;
            RLSendTab.Visibility = formItem.FormType == (int)FormType.RollingCounter ? Visibility.Visible : Visibility.Collapsed;
        }

        #region -- Get --
        /// <summary>
        /// Auto Get Start/Stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGet_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).btnGet();
        }
       
        #endregion

        #region -- Set --
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetSendFrame_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).SendFrame();
        }

        private void btnReduce_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).ChangeValueAndSend(changeType:BaseSignalViewModel.ChangeType.Reduce,this.cbbSignals);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).ChangeValueAndSend(changeType: BaseSignalViewModel.ChangeType.Add, this.cbbSignals);
        }

        private void btnDivid_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).ChangeValueAndSend(changeType: BaseSignalViewModel.ChangeType.Division, this.cbbSignals);
        }

        private void btnMultip_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).ChangeValueAndSend(changeType: BaseSignalViewModel.ChangeType.Multip, this.cbbSignals);
        }

        private void cbbSignals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).ComboBoxSignals_SelectionChanged(sender as ComboBox, tbSelectedValue);
        }

        #endregion

        #region RL

        //private  

        private void btnSendRolling_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).RLStartOrCancel();
        }

        private void btnSetValue_Click(object sender, RoutedEventArgs e)
        {
            //change selected signal's value
            ((BaseSignalViewModel)DataContext).ChangeValueByButton(((BaseSignal)this.cbbRSignal.SelectedItem).SignalName, this.selectedValueRL.Text);
        }

        private void btnRLReduce_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).ChangeValueAndSend(changeType: BaseSignalViewModel.ChangeType.Reduce, this.cbbRSignal);
            cbbRSignal_SelectionChanged(null, null);
        }

        private void btnRLAdd_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).ChangeValueAndSend(changeType: BaseSignalViewModel.ChangeType.Add, this.cbbRSignal);
            cbbRSignal_SelectionChanged(null, null);
        }

        private void btnRLDivid_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).ChangeValueAndSend(changeType: BaseSignalViewModel.ChangeType.Division, this.cbbRSignal);
            cbbRSignal_SelectionChanged(null, null);
        }

        private void btnRLMultip_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).ChangeValueAndSend(changeType: BaseSignalViewModel.ChangeType.Multip, this.cbbRSignal);
            cbbRSignal_SelectionChanged(null, null);
        }

        private void cbbRSignal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedValueRL.Text = ((BaseSignalViewModel)DataContext).BaseSignals.First(x=>x.SignalName == (cbbRSignal.SelectedItem as BaseSignal).SignalName).StrValue;
        }
        #endregion

        public void Closing()
        {
            ((BaseSignalViewModel)DataContext).Closing();
        }

        private void btnChangeSignals_Click(object sender, RoutedEventArgs e)
        {
            ((BaseSignalViewModel)DataContext).ModifiedSignals();
        }
    }
}
