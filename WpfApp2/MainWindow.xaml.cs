using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.Model;
using WpfApp2.View;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowTreeView();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ShowTreeView()
        {
            Root r = RootHelper.InitRootByJson(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Project.json");
            tvProperties.ItemsSource = r.project;

            //DBCSinal a = (DBCSinal)r.project[0].Form[0].Singals.Signal[0];
        }

        private void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void MenuItem1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            MessageBox.Show(treeViewItem.Header.ToString());
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowWeb_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewProject_Click(object sender, RoutedEventArgs e)
        {
            AddProjectWindow apw = new AddProjectWindow();
            apw.Show();
        }

        private void tvProperties_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //if(select is projectItem ) open
            if(tvProperties.SelectedItem is ProjectItem)
            {
                //ProjectWindow pw = new ProjectWindow(tvProperties.SelectedItem as ProjectItem);
                //pw.Show();

                DockForm sdf = new DockForm(tvProperties.SelectedItem as ProjectItem);
                sdf.Show();

                //SnakeWPFSample snake = new SnakeWPFSample();
                //snake.Show();
            }
            //MessageBox.Show((tvProperties.SelectedItem as ProjectItem).Name);
        }

        private void btnDockTest_Click(object sender, RoutedEventArgs e)
        {
            DockForm df = new();
            df.Show();
        }
    }


}
