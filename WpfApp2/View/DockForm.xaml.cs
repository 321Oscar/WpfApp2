using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using CanControl.CANInfo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
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
    /// DockForm.xaml 的交互逻辑
    /// </summary>
    public partial class DockForm : Window
    {
        public DockForm()
        {
            
        }
        ProjectWindowViewModel projectWindowViewModel;
        public DockForm(ProjectItem projectItem)
        {
            InitializeComponent();
            projectWindowViewModel =  new ProjectWindowViewModel(projectItem);
            this.DataContext = projectWindowViewModel;
            //this.projectItem = projectItem;
            //this.Title = projectItem.Name;
            LogInDockHelper.PrintLogFuncs.Add(projectItem.Name, new LogInDockHelper.PrintLog(ShowLog));
            //load signals
            //Forms = new ObservableCollection<ProjectItem>() { projectItem };

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockingManager);
            //using StreamReader stream = new("layout.xml");
            //serializer.Deserialize(stream);
        }

        private void AddNewProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //LayoutDocument ld = new()
                //{
                //    Title = "Test",
                //    Content = new UCPage()
                //};
                //documentPanel.Children.Add(ld);
                
                //LayoutAnchorable la = new()
                //{
                //    Title = "断点",
                //    Content = new TextBox()
                //};
                //bottomGroup.Children.Add(la);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            XmlLayoutSerializer serializer = new(dockingManager);
            using (StreamWriter stream = new("layout.xml"))
            {
                serializer.Serialize(stream);
                dockingManager.UpdateLayout();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XmlLayoutSerializer serializer = new(dockingManager);
            using StreamWriter stream = new("layout.xml");
            serializer.Serialize(stream);
            while(documentPanel.Children.Count > 0)
            {
                documentPanel.Children[0].Close();
            }

            LogInDockHelper.PrintLogFuncs.Remove(this.Title);
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeView treeView = sender as TreeView;
            if (treeView.SelectedItem is not null and FormItem)
            {
                FormItem form = treeView.SelectedItem as FormItem;

                LayoutDocument ld = new()
                {
                    Title = form.Name,
                    //Content = FormCreateHelper.CreateForm(form, ProjectItem)
                };

                if (form.FormType == (int)FormType.Scope)
                {
                    //ld.Content = new ScopeUC(projectItem, form);
                    //ld.Content = new ScopeLiveChartUC();
                }
                else
                {
                    //ld.Content = new DataUC(ProjectItem, form);
                    //ld.Content = new BaseDataUC(projectItem, form);
                }
                ld.Closing += Ld_Closing;
                documentPanel.Children.Add(ld);
                documentPanel.SelectedContentIndex = documentPanel.Children.Count - 1;
            }

        }

        private void Ld_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LayoutDocument ld = sender as LayoutDocument;
            IDataUCClosing closing = (IDataUCClosing)((UserControl)ld.Content).DataContext;
            closing.Closing();
        }

        /// <summary>
        /// 显示错误日志，不显示实时日志,10ms或频率太快会导致数据不变动
        /// </summary>
        /// <param name="log"></param>
        public void ShowLog(string log)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (lbLog.Items.Count > 100)
                {
                    lbLog.Items.RemoveAt(0);
                }

                int index = lbLog.Items.Add($"{DateTime.Now:yy/MM/dd HH:mm:ss fff}：{log}");
                lbLog.SelectedIndex = index;
            });
        }

        private void btnCanConnect_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnDisconnCan_Click(object sender, RoutedEventArgs e)
        {
            //if (USBCanManager.Instance.Close(projectItem))
            //{
            //    //btnStartCan.Text = "Start";
            //    //CanIsOpen = false;
            //    btnDisconnCan.IsEnabled = CanIsOpen;
            //    ShowLog(this.Name + "，关闭Can成功。");
            //}
        }

        private void MenuItem_AddForm_Click(object sender, RoutedEventArgs e)
        {
            ((ProjectWindowViewModel)DataContext).AddForm();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            if (listbox.SelectedItem is not null and FormItem)
            {
                FormItem form = listbox.SelectedItem as FormItem;

                LayoutDocument ld = new()
                {
                    Title = form.Name,
                    //Content = FormCreateHelper.CreateForm(form, ProjectItem)
                };

                if (form.FormType == (int)FormType.Scope)
                {
                    ld.Content = new ScopeUC(projectWindowViewModel.ProjectItem, form);
                    //ld.Content = new ScopeLiveChartUC();
                }
                else
                {
                    //ld.Content = new DataUC(projectWindowViewModel.ProjectItem, form);
                    ld.Content = new BaseDataUC(projectWindowViewModel.ProjectItem, form);
                }
                ld.Closing += Ld_Closing;
                documentPanel.Children.Add(ld);
                documentPanel.SelectedContentIndex = documentPanel.Children.Count - 1;
            }
        }
    }
}
