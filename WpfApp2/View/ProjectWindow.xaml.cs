using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// ProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectWindow : Window
    {
        public ProjectWindow()
        {
            InitializeComponent();
        }

        public ProjectWindow(ProjectItem projectItem) : this()
        {
            Title = projectItem.Name;
            DataContext = this;

            users.Add(new User() { Name = "John Joe" });
            users.Add(new User() { Name = "Jane Joe" });

            //lbUsers.ItemsSource = projectItem.Form;
        }

        private void cbAllFeatures_CheckedChanged(object sender, RoutedEventArgs e)
        {
            //bool newVal = cbAllFeatures.IsChecked == true;
            //cbFeatureAbc.IsChecked = cbFeaturexyz.IsChecked = cbFeaturewww.IsChecked = newVal;
        }

        private void cbFeatures_CheckedChanged(object sender, RoutedEventArgs e)
        {
            
            //cbAllFeatures.IsChecked = null;
            //if((cbFeatureAbc.IsChecked == true) && (cbFeaturexyz.IsChecked == true) && (cbFeaturewww.IsChecked == true))
            //{
            //    cbAllFeatures.IsChecked = true;
            //}
            //if ((cbFeatureAbc.IsChecked == false) && (cbFeaturexyz.IsChecked == false) && (cbFeaturewww.IsChecked == false))
            //{
            //    cbAllFeatures.IsChecked = false;
            //}
        }

        //private List<User> users = new List<User>();
        private ObservableCollection<User> users = new ObservableCollection<User>();

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            users.Add(new User() { Name = "New User" });
        }

        private void btnChangeUser_Click(object sender, RoutedEventArgs e)
        {
            //if (lbUsers.SelectedItem != null)
            //    (lbUsers.SelectedItem as User).Name = "Random Name";
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            //if (lbUsers.SelectedItem != null)
            //    users.Remove(lbUsers.SelectedItem as User);
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("The New command was invoked");
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
           // txtEditor.Cut();
        }

        private void CutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = (txtEditor != null) && (txtEditor.SelectionLength > 0);
        }

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //txtEditor.Paste();
        }

        private void PasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsText();
        }

        private void ExitCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEnterName_Click(object sender, RoutedEventArgs e)
        {
            //InputDialog inputDialog = new InputDialog("Please enter your name:", "John Joe");
            //if (inputDialog.ShowDialog() == true)
            //    lblName.Text = inputDialog.Answer;
        }

        private void AddTab_Click(object sender, RoutedEventArgs e)
        {
            //TabItem t = new TabItem();
            //t.Header = "new Tab";
            //tbControl.Items.Add(t);
        }

        private UsbCan usbCan;
        private bool IsOpen = false;
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!IsOpen)
            {
                usbCan = new UsbCan(21, 0, 0);
                if (usbCan.Open_Device() == 1)
                {
                    _ = MessageBox.Show("打开成功");
                    IsOpen = true;
                }
            }
            else
            {
                _ = usbCan.close_device();
                IsOpen = false;
            }

        }

        private void showCard_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }

    public class User : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get => this.name;
            set
            {
                if (this.name != value)
                {
                    name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit","Exit",typeof(CustomCommands),new InputGestureCollection() { new KeyGesture(Key.F4,ModifierKeys.Alt)});
    }

}
