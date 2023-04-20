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

namespace WpfApp2.View
{
    /// <summary>
    /// DataUC.xaml 的交互逻辑
    /// </summary>
    public partial class DataUC : UserControl, INotifyPropertyChanged
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

            ISReadOnly = formItem.FormType == 1;
            cbbSignals.ItemsSource = DbcSignals;
            this.DataContext = this;

            gdData.Columns[2].IsReadOnly = !ISReadOnly;
        }
        #region 属性

        
        private ProjectItem OwnerProject;
        private FormItem formItem;
        public ObservableCollection<DBCSignal> DbcSignals { get; }
        private bool isReadOnly;
        private int interval = 10;
        private bool isGetdata;
        public bool ISReadOnly { get => isReadOnly; private set => isReadOnly = value; }
        public int IntervalTime { get => interval; set { interval = value; OnPropertyChanged(nameof(IntervalTime)); } }
        public bool IsGetData
        {
            get => isGetdata;
            set { isGetdata = value; OnPropertyChanged(nameof(IsGetData)); }
        }
        public bool? IsAllSignalsSelected
        {
            get
            {
                var selected = DbcSignals.Select(x => x.IsSelected).Distinct().ToList();
                return selected.Count == 1 ? selected.Single() : null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectALL(value.Value, DbcSignals);
                    OnPropertyChanged(nameof(IsAllSignalsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region -- Private
        private void SelectALL(bool select, IEnumerable<DBCSignal> signals)
        {
            foreach (var item in signals)
            {
                item.IsSelected = select;
            }
        }

        #endregion

        #region -- Get --
        /// <summary>
        /// Auto Get Start/Stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    if (signal.IsSelected)
                        signal.DValue = r.Next(0, 100) * 1.0d;
                }
                Thread.Sleep(IntervalTime);
            }
        }
        #endregion

        #region -- Set --
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            //send frame
        }
        #endregion
        private void cbbSignals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbSignals.SelectedIndex != -1)
            {
                DBCSignal selectSignal = cbbSignals.SelectedItem as DBCSignal;

                Binding binding = new()
                {
                    Source = selectSignal,
                    Path = new PropertyPath(nameof(selectSignal.DValue)),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(tbSelectedValue, TextBox.TextProperty, binding);
            }
        }

        private void btnSendRolling_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
