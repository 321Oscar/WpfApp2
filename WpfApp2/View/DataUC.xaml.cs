using ProtocolLib.Signal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfApp2.View
{
    /// <summary>
    /// DataUC.xaml 的交互逻辑
    /// </summary>
    public partial class DataUC : UserControl
    {
        /// <summary>
        /// 数据窗口：Get/Set
        /// </summary>
        public DataUC()
        {
            InitializeComponent();
        }

        public DataUC(ProjectItem project, FormItem formItem) : this()
        {
            OwnerProject = project;
            this.formItem = formItem;
            //load Signals in DataGird
            DbcSignals = new ObservableCollection<DBCSignal>(formItem.Singals.Signal);
            this.dataGridSignals.ItemsSource = DbcSignals;

            isReadOnly = formItem.FormType == 1;
            cbbSignals.ItemsSource = DbcSignals;
            this.DataContext = this;
        }

        private ProjectItem OwnerProject;
        private FormItem formItem;
        public ObservableCollection<DBCSignal> DbcSignals { get; }
        private bool isReadOnly;
        private bool isGetData = false;
        public bool ISReadOnly => isReadOnly;
        public int IntervalTime { get; set; } = 10;
        public bool IsGetData
        {
            get => isGetData;
            set => isGetData = value;
        }

        private void cbbSignals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbSignals.SelectedIndex != -1)
            {
                DBCSignal selectSignal = cbbSignals.SelectedItem as DBCSignal;

                ///不能绑定数据，rollingcounter 模式下改变数据会跳变
                ///9->10 先发1，再发10
                //if (formItem.FormType == (int)FormType.Set)
                //{
                //    var threadSafeModel = new SynchronizedNotifyPropertyChanged<DBCSignal>(selectSignal, this);
                //    tbCurrent.da
                //    tbCurrent.DataBindings.Add("Text", threadSafeModel, "StrValue", false, DataSourceUpdateMode.OnPropertyChanged, "0");
                //}

                //tbCurrent.Text = selectSignal.StrValue;

            }
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            //send frame
        }

        private void btnGet_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGetData)
            {
                IsGetData = true;
                _ = Task.Run(new Action(AutoGetData));
            }
            else
            {
                IsGetData = false;
            }
        }
        /// <summary>
        /// 生成测试数据随机数
        /// </summary>
        private readonly Random r = new Random();
        private void AutoGetData()
        {
            while (IsGetData)
            {
                foreach (var signal in DbcSignals)
                {
                    signal.StrValue = r.Next(0, 100).ToString();
                }
                Thread.Sleep(10);
            }
        }
    }
}
