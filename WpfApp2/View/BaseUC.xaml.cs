using ProtocolLib.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
    /// BaseUC.xaml 的交互逻辑
    /// </summary>
    public partial class BaseUC : UserControl
    {
        public BaseUC()
        {
            InitializeComponent();
        }

        public ProjectItem ProjectItem;
        public FormItem FormItem;
        public string ProtocolCommand;
        private BaseProtocol protocol;
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

                        protocol = ReflectionHelper.CreateInstance<BaseProtocol>(ProtocolCommand, Assembly.GetExecutingAssembly().ToString()
                            , new string[] { canIndex.ProtocolFileName });
                        return protocol;
                    }
                }
            }
        }
        public int CanChannel;
        public FormType FormType;

    }

    //internal class FormCreateHelper
    //{

    //    public static BaseUC CreateForm(FormItem formItem, ProjectItem owner)
    //    {
    //        BaseUC userForm = null;

    //        switch ((FormType)formItem.FormType)
    //        {
    //            case FormType.Scope:
    //                userForm = new ScopeUC();
    //                break;
    //            case FormType.Get:
    //                //userForm = new GetForm();
    //                userForm = new BaseDataUC(owner, formItem,FormType.Get);
    //                break;
    //            case FormType.Set:
    //                //userForm = new SetForm();
    //                userForm = new BaseDataUC(owner, formItem, FormType.Set);
    //                break;
    //            case FormType.RollingCounter:
    //                userForm = new BaseDataUC(owner, formItem, FormType.RollingCounter);
    //                break;
    //        }
    //        if (userForm == null)
    //            return null;
    //        //userForm.TopLevel = false;
    //        userForm.Name = formItem.Name;
    //        userForm.CanChannel = formItem.CanChannel;
    //        //userForm.Signals = formItem.Singals;
    //        userForm.ProjectItem = owner;
    //        userForm.FormItem = formItem;
    //        //userForm.Title = formItem.Name;

    //        CanIndexItem canIndex = owner.CanIndex.Find(x => x.CanChannel == formItem.CanChannel);
    //        if (canIndex == null)
    //        {
    //            throw new Exception("Can通道配置错误");
    //        }
    //        else
    //        {
    //            switch ((ProtocolType)canIndex.ProtocolType)
    //            {
    //                case ProtocolType.DBC:
    //                    userForm.ProtocolCommand = "ProtocolLib.Protocols.DBC.DBCProtocol";
    //                    break;
    //                case ProtocolType.Excel:
    //                    throw new Exception("Excel协议未实现");
    //            }
    //        }

    //        return userForm;
    //    }
    //}
}
