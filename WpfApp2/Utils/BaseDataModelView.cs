using Prism.Mvvm;
using ProtocolLib.Protocols;
using ProtocolLib.Signal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Model;
using WpfApp2.Utils;

namespace WpfApp2.Utils
{
    /// <summary>
    /// 数据窗口统一结构
    /// </summary>
    public class BaseDataModelView : BindableBase
    {
        /// <summary>
        /// 所属Project，用以连接CAN，日志显示
        /// </summary>
        public ProjectItem ProjectItem { get; protected set; }
        /// <summary>
        /// 数据窗口
        /// </summary>
        public FormItem FormItem { get; protected set; }
        private string protocolCommand;
        /// <summary>
        /// 协议类的路径
        /// </summary>
        public string ProtocolCommand
        {
            set => protocolCommand = value;
            get
            {
                if(protocolCommand == null)
                {
                    CanIndexItem canIndex = ProjectItem.CanIndex.Find(x => x.CanChannel == FormItem.CanChannel);
                    if (canIndex == null)
                    {
                        throw new Exception("Can通道配置错误");
                    }
                    else
                    {
                        switch ((ProtocolType)canIndex.ProtocolType)
                        {
                            case ProtocolType.DBC:
                                protocolCommand = "ProtocolLib.Protocols.DBC.DBCProtocol";
                                break;
                            case ProtocolType.Excel:
                                throw new Exception("Excel协议未实现");
                            case ProtocolType.XCP:
                                protocolCommand = "ProtocolLib.Protocols.DBC.XCPProtocol";
                                break;
                        }
                    }
                }
                return protocolCommand;
            }
        }
        private BaseProtocol protocol;
        /// <summary>
        /// 协议实体类，用以数据解析
        /// </summary>
        public BaseProtocol Protocol
        {
            get
            {
                if (protocol != null)
                {
                    return protocol;
                }
                else
                {
                    if (string.IsNullOrEmpty(ProtocolCommand))
                    {
                        return null;
                    }
                    else
                    {
                        CanIndexItem canIndex = ProjectItem.CanIndex.Find(x => x.CanChannel == CanChannel);

                        protocol = ReflectionHelper.CreateInstance<BaseProtocol>(ProtocolCommand, "ProtocolLib"
                            , new string[] { canIndex.ProtocolFileName });
                        return protocol;
                    }
                }
            }
        }
        /// <summary>
        /// 当前CAN的CAN口
        /// </summary>
        public int CanChannel { get; set; }
        /// <summary>
        /// 窗口类型
        /// </summary>
        public FormType FormType { get; set; }
        private ObservableCollection<BaseSignal> _BaseSignals;
        /// <summary>
        /// 显示日志
        /// </summary>
        public LogInDockHelper.PrintLog EPrintLog;
        /// <summary>
        /// 窗口中的信号，用以界面显示
        /// </summary>
        public ObservableCollection<BaseSignal> BaseSignals { get=> _BaseSignals; set=>SetProperty(ref _BaseSignals,value); }
        /// <summary>
        /// 更改信号
        /// </summary>
        public virtual void ChangeSignals() 
        {
           // BaseSignals = new ObservableCollection<BaseSignal>(newSignals);
        }

    }
}
