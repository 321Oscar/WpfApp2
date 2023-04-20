using CanControl.CANInfo;
using Prism.Mvvm;
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
using WpfApp2.Model;
using WpfApp2.View;
using static WpfApp2.Utils.LogInDockHelper;

namespace WpfApp2.Utils
{
    /// <summary>
    /// Get,Set,RL ViewModel
    /// </summary>
    public class BaseSignalViewModel : BaseDataModelView, IDataUCClosing
    {
        private CancellationTokenSource tokenSource;
        private bool isReadOnly;
        private int interval = 10;
        private bool isGetdata;
        private bool? isSelectedAll;
        private bool disposedValue;
        private decimal addStep = 1m;
        private decimal multipStep = 1.0m;
        /// <summary>
        /// 是否可修改信号值
        /// </summary>
        public bool ISReadOnly { get => isReadOnly; private set => SetProperty(ref isReadOnly, value); }
        /// <summary>
        /// 获取数据事件间隔，ms为单位
        /// </summary>
        public int IntervalTime { get => interval; set => SetProperty(ref interval, value); }
        /// <summary>
        /// 是否在执行获取数据线程
        /// </summary>
        public bool IsGetData
        {
            get => isGetdata;
            set => SetProperty(ref isGetdata, value);
        }
        
        public decimal AddStep { get => addStep; set => SetProperty(ref addStep, value); }
        public decimal MultipStep { get => multipStep; set => SetProperty(ref multipStep, value); }
        /// <summary>
        /// 全选
        /// </summary>
        public bool? IsAllSignalsSelected
        {
            get
            {
                var selected = BaseSignals.Select(x => x.IsSelected).Distinct().ToList();
                isSelectedAll = selected.Count == 1 ? selected.Single() : null;
                return isSelectedAll;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectALL(value.Value, BaseSignals);
                    SetProperty(ref isSelectedAll, value);
                }
            }
        }

        public BaseSignalViewModel(ProjectItem project, FormItem formItem)
        {
            ProjectItem = project;
            this.FormItem = formItem;
            //load Signals in DataGird
            LoadSignals();

             ISReadOnly = formItem.FormType == (int)FormType.Get;
            //ProtocolCommand = 
        }

        #region -- Private --
        private void SelectALL(bool select, IEnumerable<BaseSignal> signals)
        {
            foreach (var item in signals)
            {
                item.IsSelected = select;
            }
        }

        private void LoadSignals()
        {
            BaseSignals = new ObservableCollection<BaseSignal>(this.FormItem.Singals.Signal);
            foreach (var item in BaseSignals)
            {
                item.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(BaseSignal.IsSelected))
                        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(IsAllSignalsSelected)));
                };
            }
        }

        #endregion

        #region -- Public --
        /// <summary>
        /// 绑定TextBox和Combobox
        /// </summary>
        /// <param name="cbbSignals"></param>
        /// <param name="tbSelectedValue"></param>
        public virtual void ComboBoxSignals_SelectionChanged(ComboBox cbbSignals, TextBox tbSelectedValue)
        {
            if (cbbSignals.SelectedIndex != -1)
            {
                BaseSignal selectSignal = cbbSignals.SelectedItem as BaseSignal;

                Binding binding = new()
                {
                    Source = selectSignal,
                    Path = new PropertyPath(nameof(selectSignal.DValue)),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                _ = BindingOperations.SetBinding(tbSelectedValue, TextBox.TextProperty, binding);
            }
        }

        private bool rLStart;

        public void ModifiedSignals()
        {
            ModifiedFormItemForm modifiedFormItemForm = new ModifiedFormItemForm(ProjectItem, FormItem, false);
            if (modifiedFormItemForm.ShowDialog() == true)
            {
                ChangeSignals();
            }
        }
        #endregion

        #region -- Protected --
        public override void ChangeSignals()
        {
            LoadSignals();
        }
        /// <summary>
        /// 生成测试数据随机数
        /// </summary>
        private readonly Random r = new Random();
        protected virtual void AutoGetData(CancellationToken token)
        {
            while (IsGetData && !token.IsCancellationRequested)
            {
                try
                {
                    //foreach (var signal in BaseSignals)
                    //{
                    //    if (signal.IsSelected)
                    //    {
                    //        signal.DValue = r.Next(0, 100) * 1.0d;
                    //        EPrintLog(signal.ToString());
                    //    }
                    //}
                    var rx_mails = USBCanManager.Instance.Receive(ProjectItem, BaseSignals.Select(x => ((DBCSignal)x).MsgIDInt).Distinct().ToList().ToArray(), CanChannel, $"[{this.FormType}]");
                    Protocol.Multip(rx_mails, BaseSignals);
                    Thread.Sleep(IntervalTime);
                }
                catch (Exception ex)
                {
                    EPrintLog(ex.Message);
                    IsGetData = false;
                }
               
            }
        }
        #endregion

        #region -- Get --
        public void btnGet()
        {
            if (!IsGetData)
            {
                IsGetData = true;

                this.tokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(() => this.AutoGetData(this.tokenSource.Token), this.tokenSource.Token);
            }
            else
            {
                IsGetData = false;
                // cancel the worker tasks
                this.tokenSource.Cancel();
                this.tokenSource = null;
            }
        }
        #endregion

        #region -- Set --
        public void SendFrame()
        {
            byte sendtype = 2;//(byte)tscbb.SelectedIndex;
            string log = string.Empty;
            var currentIDs = BaseSignals.Select(x => ((DBCSignal)x).MsgIDInt).Distinct().ToList().ToArray();
            for (int i = 0; i < currentIDs.Length; i++)
            {
                Dictionary<BaseSignal, string> keyValues = new Dictionary<BaseSignal, string>();
                var signals = BaseSignals.Where(x => ((DBCSignal)x).MessageID == currentIDs[i].ToString("X"));
                foreach (var item in signals)
                {
                    keyValues.Add(item, item.StrValue);
                }
                var frame = Protocol.BuildFrames(keyValues);
                try
                {
                    if (USBCanManager.Instance.Send(ProjectItem, CanChannel, sendData: frame[0], $"[{this.FormType}]", sendtype))
                    {
                        EPrintLog($"{frame[0].cid:X}发送成功");
                    }
                }
                catch (USBCANOpenException ex)
                {
                    EPrintLog(ex.Message);
                    //LeapMessageBox.Instance.ShowInfo(ex.Message);
                    break;
                }

            }
        }
        public enum ChangeType
        {
            Add,
            Reduce,
            Multip,
            Division
        }
        public virtual void ChangeValueAndSend(ChangeType changeType, ComboBox cbbSignals)
        {
            if (cbbSignals.SelectedIndex != -1)
            {
                DBCSignal selectSignal = cbbSignals.SelectedItem as DBCSignal;

                decimal oldValue = Convert.ToDecimal(selectSignal.DValue);
                decimal newValue = oldValue;
                switch (changeType)
                {
                    case ChangeType.Add:
                        newValue = oldValue + AddStep;
                        break;
                    case ChangeType.Reduce:
                        newValue = oldValue - AddStep;
                        break;
                    case ChangeType.Multip:
                        newValue = oldValue * MultipStep;
                        break;
                    case ChangeType.Division:
                        newValue = oldValue / MultipStep;
                        break;
                }

                selectSignal.DValue = (double)newValue;

                //cbbSignals_SelectedIndexChanged(null, null);

                this.SendFrame();
            }
        }
        #endregion

        #region RL
        public bool RLStart { get => rLStart; set => SetProperty(ref rLStart, value); }
        private readonly Dictionary<int, CancellationTokenSource> rLCancels = new();
        public async void RLStartOrCancel()
        {
            if (!RLStart)//Start
            {
                rLCancels.Clear();
                RLStart = true;
                var currentIDs = BaseSignals.Select(x => ((DBCSignal)x).MsgIDInt).Distinct().ToList().ToArray();
                Task t1 = null;
                for (int i = 0; i < currentIDs.Length; i++)
                {
                    //判断发送事件间隔
                    int cycleTime = ((DBCSignal)BaseSignals.First(x => ((DBCSignal)x).MsgIDInt == currentIDs[i])).CycleTime;
                    if (cycleTime == 0)
                        continue;
                    CancellationTokenSource signalCancel = new CancellationTokenSource();
                    rLCancels.Add(currentIDs[i], signalCancel);
                    EPrintLog($"{ currentIDs[i]:X}：启动发送");
                    if (i == 0)
                        t1 = SendData(currentIDs[i], signalCancel.Token);
                    else
                    {
                        Task t = SendData(currentIDs[i], signalCancel.Token);
                    }
                }
                if (t1 != null)
                    await t1;
            }
            else
            {
                foreach (var rLCancel in rLCancels.Values)
                {
                    rLCancel.Cancel();
                    rLCancel.Dispose();
                }
                rLCancels.Clear();
                RLStart = false;
            }
        }

        //private async Task<int> SendTest(int id, CancellationToken token)
        //{
        //    await Task.Run(() => {
        //        while (true)
        //        {
        //            EPrintLog($"{ id:X}：发送中");
        //            Thread.Sleep(10);
        //        }
        //    }, token);
        //    return id;
        //}

        private async Task<bool> SendData(int msgID, CancellationToken token)
        {
            await Task.Run(() =>
            {
                while (RLStart && !token.IsCancellationRequested)
                {
                    EPrintLog($"{ msgID:X}：发送中");
                    Dictionary<BaseSignal, string> keyValues = new Dictionary<BaseSignal, string>();
                    IEnumerable<BaseSignal> signals = BaseSignals.Where(x => ((DBCSignal)x).MsgIDInt == msgID);
                    bool needCheckSum = false;
                    foreach (BaseSignal item in signals)
                    {
                        if (!item.IsSelected)
                            continue;
                        if (item.SignalName.ToLower().Contains("rolling"))
                        {
                            if (item.DValue >= 15)
                            {
                                //uS.SetData("0");
                                item.DValue = 0;
                            }
                            else
                            {
                                item.DValue += 1;
                            }
                        }
                        if (item.SignalName.ToLower().Contains("checksum"))
                        {
                            needCheckSum = true;
                        }

                        keyValues.Add(item, item.StrValue);
                    }
                    try
                    {
                        if(keyValues.Count > 0)
                        {
                            CANSendFrame[] frame = Protocol.BuildFrames(keyValues);

                            /// 这里直接写死了checksum的位置和长度
                            /// 重新计算checksum
                            if (needCheckSum)
                            {
                                byte crc = 0;
                                for (ushort i = 0; i < 7; i++)
                                {
                                    crc = (byte)(crc + frame[0].w[i]);
                                }
                                crc = (byte)(crc ^ 0xff);
                                frame[0].w[7] = crc;
                            }

                            if (USBCanManager.Instance.Send(ProjectItem, CanChannel, sendData: frame[0], $"[{FormType}]", 2))
                            {
                                EPrintLog($"{FormItem.Name}：{frame[0].cid:X}发送成功");
                            }
                        }
                    }
                    catch (USBCANOpenException ex)
                    {
                        EPrintLog($"{ex.Message}");
                        RLStart = false;
                        break;
                    }
                    catch (Exception er)
                    {
                        EPrintLog($"{FormItem.Name} ：{er.Message}");
                    }
                    finally
                    {
                        int cycleTime = ((DBCSignal)BaseSignals.First(x => ((DBCSignal)x).MsgIDInt == msgID)).CycleTime;

                        Thread.Sleep(cycleTime);
                    }
                }
            }, token);
            return true;
        }

        public void ChangeValueByButton(string signalName, string strVal)
        {
            try
            {
                double dVal = double.Parse(strVal);
                BaseSignals.First(x => x.SignalName == signalName).DValue = dVal;
            }
            catch (Exception ex)
            {
                EPrintLog($"{ex.Message}");
            }
            
        }
        #endregion

        #region IDispose
        public void Closing()
        {
            IsGetData = false;
            RLStart = false;
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
            if (rLCancels.Count > 0)
            {
                foreach (CancellationTokenSource rLCancel in rLCancels.Values)
                {
                    rLCancel.Cancel();
                    rLCancel.Dispose();
                }
            }
            rLCancels.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    this.Closing();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~BaseSignalViewModel()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
