using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using System;
using System.Collections.Generic;
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

namespace WpfApp2.View
{
    /// <summary>
    /// DockForm.xaml 的交互逻辑
    /// </summary>
    public partial class DockForm : Window
    {
        public DockForm()
        {
            InitializeComponent();
        }

        public DockForm(ProjectItem projectItem) : this()
        {
            ProjectItem = projectItem;
            this.Title = projectItem.Name;
            //load signals
            tvForms.Items.Clear();
            tvForms.ItemsSource = projectItem.Form;
        }

        ProjectItem ProjectItem;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockingManager);
            //using StreamReader stream = new("layout.xml");
            //serializer.Deserialize(stream);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                LayoutDocument ld = new()
                {
                    Title = "Test",
                    Content = new UCPage()
                };
                documentPanel.Children.Add(ld);
                
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
            using StreamWriter stream = new("layout.xml");
            serializer.Serialize(stream);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XmlLayoutSerializer serializer = new(dockingManager);
            using StreamWriter stream = new("layout.xml");
            serializer.Serialize(stream);
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeView treeView = sender as TreeView;
            if (treeView.SelectedItem != null)
            {
                FormItem form = treeView.SelectedItem as FormItem;

                LayoutDocument ld = new()
                {
                    Title = form.Name,
                    //Content = new DataUC(ProjectItem, form)
                };
                if (form.FormType == (int)FormType.Scope)
                {
                    ld.Content = new ScopeUC();
                }
                else
                {
                    ld.Content = new DataUC(ProjectItem, form);
                }
                documentPanel.Children.Add(ld);
                documentPanel.SelectedContentIndex = documentPanel.Children.Count - 1;
            }

        }
    }
}
