using Prism.Mvvm;
using ProtocolLib.Protocols;
using ProtocolLib.Signal;
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
    public class FormItemViewModel : BindableBase
    {
        private ProjectItem projectItem;
        private ObservableCollection<BaseSignal> selectedSignals;
        private bool isModified;
        private string formName;
        private CanIndexItem canIndex;
        private FormType formType;
        private List<BaseSignal> allSignals;

        public ProjectItem ProjectItem { get => projectItem; set => projectItem = value; }
        public FormItem FormItem { get; set; }
        public ObservableCollection<BaseSignal> SelectedSignals { get => selectedSignals; set => selectedSignals = value; }
        public bool IsModified { get => isModified; set => SetProperty(ref isModified, value); }

        public string FormName { get => formName; set => SetProperty(ref formName, value); }
        public CanIndexItem CanIndex { get => canIndex; set => SetProperty(ref canIndex, value); }
        public FormType FormType { get => formType; set => SetProperty(ref formType, value); }
        public List<BaseSignal> AllSignals { get => allSignals; set => SetProperty(ref allSignals, value); }
        public bool IsNew { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectItem"></param>
        /// <param name="formItem"></param>
        /// <param name="canModified">是否修改</param>
        public FormItemViewModel(ProjectItem projectItem, FormItem formItem, bool canModified)
        {
            IsModified = canModified;//修改
            IsNew = !canModified;
            ProjectItem = projectItem;
            this.FormItem = formItem;
            FormName = formItem.Name;
            CanIndex = projectItem.CanIndex.Find(x => x.CanChannel == formItem.CanChannel);
            FormType = (FormType)formItem.FormType;
        }

        internal void Init(ComboBox index,ComboBox formtype)
        {
            BindCanIndex(index);
            BindCombobox(formtype);
            LoadSignals(null);
        }

        public void LoadSignals(TreeView treeView, string queryStr = null)
        {
            string fileName = CanIndex.ProtocolFileName;
            string[] fileNames = fileName.Split(';');
            List<BaseSignal> x = BaseProtocol.GetSingalsByProtocol(CanIndex.ProtocolType, fileNames);
            AllSignals = !string.IsNullOrEmpty(queryStr)
                ? (from c in x
                   where c.SignalName.ToLower(null).Contains(queryStr.ToLower(null), StringComparison.CurrentCulture) || ((DBCSignal)c).MessageID.ToLower(null).Contains(queryStr.ToLower(null), StringComparison.CurrentCulture)
                   select c).ToList()
                : x;
            if (FormItem.Singals == null || FormItem.Singals.Signal.Count == 0)
                return;
            foreach (var item in AllSignals)
            {
                if(this.FormItem.Singals.Signal.Any(x=>x.SignalName == item.SignalName))
                {
                    item.IsSelected = true;
                }
            }
        }

        public void BindCanIndex(ComboBox comboBox)
        {
            var canindex = new ObservableCollection<CanIndexItem>(ProjectItem.CanIndex);
            comboBox.DisplayMemberPath = "CanChannel";
            comboBox.ItemsSource = canindex;
        }

        public void BindCombobox(ComboBox combobox)
        {
            var ft = new ObservableCollection<FormType>();
            foreach (FormType item in Enum.GetValues(typeof(FormType)))
            {
                ft.Add(item);
            }
            combobox.ItemsSource = ft;
        }

        public void BtnOk()
        {
            if (projectItem.Form.Find(x => x.Name == FormItem.Name) == null)
            {
                projectItem.Form.Add(FormItem);
                FormItem.Singals = new Singals();
            }
            FormItem.Singals.Signal.Clear();
            foreach (var item in AllSignals.FindAll(x => x is DBCSignal && x.IsSelected))
            {
                FormItem.Singals.Signal.Add((DBCSignal)item);
            }
        }
    }
}
