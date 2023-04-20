using CanControl.CANInfo;
using Prism.Mvvm;
using ProtocolLib.Protocols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfApp2.Model;

namespace WpfApp2.Utils
{
    public class ProjectViewModel : BindableBase
    {
        private ProjectItem projectItem;
        private DeviceType? deviceType;
        private BaudRateType? baudRate;
        private ProtocolType? protocolType;
        private int? selectedCanChannel;
        private bool? selectedChannelUsed;
        private string selectedChannelFiles;
        public ProjectItem ProjectItem { get => projectItem; set => SetProperty(ref projectItem, value); }
        public DeviceType? DeviceType { get => deviceType; set => SetProperty(ref deviceType, value); }
        public BaudRateType? BaudRate { get => baudRate; set => SetProperty(ref baudRate, value); }

        public ProtocolType? ProtocolType { get => protocolType; set => SetProperty(ref protocolType, value); }
        public static List<int> CanIndexes => new() { 0, 1, 2, 3, 4, 5, 6 };

        public int? SelectedCanChannel { get => selectedCanChannel; set => SetProperty(ref selectedCanChannel, value); }

        public bool? SelectedChannelUsed { get => selectedChannelUsed; set => SetProperty(ref selectedChannelUsed, value); }
        public string SelectedChannelFiles { get => selectedChannelFiles; set => SetProperty(ref selectedChannelFiles, value); }

        public ProjectViewModel(ProjectItem projectItem)
        {
            ProjectItem = projectItem;
        }

        public void Init(ComboBox devices, ComboBox baud, ComboBox protocols, ComboBox canChannel)
        {
            //Can Device
            BindCombobox<DeviceType>(devices);
            devices.SelectionChanged += (sender, args) =>
            {
                ComboBox cbb = sender as ComboBox;
                canChannel.Items.Clear();
                if (DeviceType.HasValue)
                {
                    switch (DeviceType.Value)
                    {
                        case CanControl.CANInfo.DeviceType.VCI_USBCAN1:
                            _ = canChannel.Items.Add(0);
                            break;
                        case CanControl.CANInfo.DeviceType.VCI_USBCAN_2E_U:
                            _ = canChannel.Items.Add(0);
                            _ = canChannel.Items.Add(1);
                            break;
                        default:
                            break;
                    }
                    canChannel.SelectedIndex = 0;
                }
            };
            canChannel.SelectionChanged += (sender, args) => { LoadChannelConfig(); };
            BindCombobox<BaudRateType>(baud);
            BindCombobox<ProtocolType>(protocols);
        }

        public void ChangeProtocolFiles()
        {
            switch (ProtocolType.Value)
            {
                case ProtocolLib.Protocols.ProtocolType.DBC:
                    break;
                case ProtocolLib.Protocols.ProtocolType.Excel:
                    break;
                case ProtocolLib.Protocols.ProtocolType.XCP:
                    break;
                default:
                    break;
            }
        }

        public void LoadChannelConfig()
        {
            if (SelectedCanChannel.HasValue)
            {
                CanIndexItem canIndex = projectItem.CanIndex.Find(x => x.CanChannel == SelectedCanChannel.Value);
                if(canIndex == null)
                {
                    //Clear Config
                    SelectedChannelUsed = null;
                    BaudRate = null;
                    ProtocolType = null;
                    SelectedChannelFiles = null;
                }
                else
                {
                    SelectedChannelUsed = canIndex.isUsed;
                    BaudRate = (BaudRateType)canIndex.BaudRate;
                    ProtocolType = (ProtocolType?)canIndex.ProtocolType;
                    SelectedChannelFiles = canIndex.ProtocolFileName;
                }
            }
        }

        public void SaveChannelConfig()
        {
            if (SelectedCanChannel.HasValue)
            {
                CanIndexItem canIndex = projectItem.CanIndex.Find(x => x.CanChannel == SelectedCanChannel.Value);
                if (null == canIndex)
                {
                    canIndex = new();
                    ProjectItem.CanIndex.Add(canIndex);
                }

                canIndex.isUsed = SelectedChannelUsed.Value;
                if (SelectedChannelUsed.Value)
                {
                    canIndex.ProtocolType = (int)ProtocolType.Value;
                    canIndex.ProtocolFileName = SelectedChannelFiles;
                    canIndex.BaudRate = (int)(BaudRateType)Enum.Parse(typeof(BaudRateType), BaudRate.Value.ToString(), false);
                }
            }
        }

        internal void SelectFile()
        {
            throw new NotImplementedException();
        }

        internal void SaveProject()
        {
            //ProjectItem.DeviceIndex = Devicein;
            ProjectItem.DeviceType = (int)DeviceType;
        }

        private static void BindCombobox<T>(ComboBox combobox)
        {
            ObservableCollection<T> ft = new ObservableCollection<T>();
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                ft.Add(item);
            }
            combobox.ItemsSource = ft;
        }
    }
}
