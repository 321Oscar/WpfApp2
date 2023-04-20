using CanControl.CANInfo;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Model;
using WpfApp2.View;

namespace WpfApp2.Utils
{
    public class ProjectWindowViewModel : BindableBase
    {
        private bool canIsOpen;
        private ProjectItem projectItem;
        private ObservableCollection<FormItem> forms;
        private IEventAggregator ea;
        private ObservableCollection<string> messages;

        public ObservableCollection<FormItem> Forms { get => forms; set => SetProperty(ref forms, value); }
        public bool CanIsOpen { get => canIsOpen; set=>SetProperty(ref canIsOpen,value); }
        public ProjectItem ProjectItem { get => projectItem; set => SetProperty(ref projectItem, value); }
        public DelegateCommand ConnectCanCommand { get;private set; }
        public DelegateCommand DisConnectCanCommand { get; private set; }
        public ObservableCollection<string> Messages { get => messages; set => SetProperty(ref messages, value); }

        public ProjectWindowViewModel(ProjectItem projectItem)
        {
            ProjectItem = projectItem;
            Forms = new ObservableCollection<FormItem>(projectItem.Form);
            ConnectCanCommand = new DelegateCommand(Connect);
            DisConnectCanCommand = new DelegateCommand(DisConnect).ObservesCanExecute(() => CanIsOpen);
            //订阅事件
            //ea.GetEvent<LogInfoEven>().Subscribe(ShowLog);
        }

        private bool CanConnect()
        {
            return !!CanIsOpen;
        }

        public void Connect()
        {
            if (!USBCanManager.Instance.Exist(projectItem))
            {
                UsbCan usbCan = new UsbCan(projectItem.DeviceType, projectItem.DeviceIndex, 0);
                USBCanManager.Instance.AddUsbCan(usbCan, projectItem);
            }

            if (CanIsOpen)
            {
                return;
            }
            else
            {
                ///can.open 
                if (USBCanManager.Instance.Open(projectItem))
                {
                    List<int> caninds = new List<int>();
                    foreach (var item in projectItem.CanIndex)
                    {
                        caninds.Add(item.CanChannel);
                        if ((DeviceType)projectItem.DeviceType == DeviceType.VCI_USBCAN_2E_U)
                        {
                            ///Can.SetReference
                            USBCanManager.Instance.SetRefenrece(projectItem, item.CanChannel);
                        }
                        ///can.init
                        _ = USBCanManager.Instance.InitCan(projectItem, item.CanChannel);
                        ///can.start
                        _ = USBCanManager.Instance.StartCanIndex(projectItem, item.CanChannel);
                    }
                    CanIsOpen = true;

                    //启动接收线程
                    ea.GetEvent<LogInfoEven>().Publish($"{(DeviceType)projectItem.DeviceType} [{projectItem.CanIndex.Count}] 已打开 ");
                    //this.tblog.Text = $"{(DeviceType)projectItem.DeviceType} [{projectItem.CanIndex.Count}] 已打开 ";
                    USBCanManager.Instance.StartRecv(projectItem, caninds.ToArray());//caninds
                    return;
                }
                else
                {
                    //this.tblog.Text = $"{(DeviceType)projectItem.DeviceType} 打开失败 ";
                    USBCanManager.Instance.RemoveUsbCan(projectItem);
                    return;
                }

            }
        }

        public void DisConnect()
        {
            if (USBCanManager.Instance.Close(projectItem))
            {
                //btnStartCan.Text = "Start";
                CanIsOpen = false;
            }
        }

        public void AddForm()
        {
            ModifiedFormItemForm modifiedFormItemForm = new ModifiedFormItemForm(projectItem);
            if (modifiedFormItemForm.ShowDialog() == true)
            {
                Forms.Add(((FormItemViewModel)modifiedFormItemForm.DataContext).FormItem);
            }
        }

        private void ShowLog(string log)
        {
            Messages.Add(log);
        }
    }
}
