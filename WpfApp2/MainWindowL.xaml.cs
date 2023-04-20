using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// MainWindowL.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindowL : Window
    {
        public MainWindowL()
        {
            InitializeComponent();

            #region 基础数据(员工集合)
            ObservableCollection<Employee> employeeList = new ObservableCollection<Employee>
            {
               new Employee{EmployeeNum="0027",EmployeeName="张三",Sex="男",Title="副经理"},
               new Employee{EmployeeNum="1086",EmployeeName="春丽",Sex="女",Title="秘书"},
               new Employee{EmployeeNum="1031",EmployeeName="王五",Sex="男",Title="普通员工"},
               new Employee{EmployeeNum="1211",EmployeeName="赵阳",Sex="男",Title="普通员工"},
               new Employee{EmployeeNum="1201",EmployeeName="孙迪",Sex="男",Title="普通员工"},
               new Employee{EmployeeNum="1416",EmployeeName="李玥玥",Sex="女",Title="秘书"},
               new Employee{EmployeeNum="0017",EmployeeName="钱哆哆",Sex="男",Title="副经理"},
               new Employee{EmployeeNum="1016",EmployeeName="周畅",Sex="女",Title="秘书"},
               new Employee{EmployeeNum="1231",EmployeeName="郑超",Sex="男",Title="普通员工"},
               new Employee{EmployeeNum="1131",EmployeeName="王思聪",Sex="男",Title="普通员工"},
               new Employee{EmployeeNum="1871",EmployeeName="李文",Sex="男",Title="普通员工"},
               new Employee{EmployeeNum="1266",EmployeeName="周琪妹",Sex="女",Title="秘书"}
            };
            #endregion

            CollectionViewSource employeeCvs = (CollectionViewSource)this.FindResource("employeeCollectionViewSource");
            employeeCvs.Source = employeeList;
        }

        /// <summary>
        /// 右键菜单、按住Ctrl键可多选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //获取关键字
            string keyword = txtEmployeeKeyword.Text.Trim();
            if (string.IsNullOrEmpty(keyword))//如果没有关键字
            {
                if (lbx1.SelectedItem != null)//判断lbx1有没有选中项
                {
                    foreach (var item in lbx1.SelectedItems)
                    {
                        Employee employee = item as Employee;
                        string msg = string.Format("姓名：{0},工号:{1},性别:{2},职位:{3}", employee.EmployeeName, employee.EmployeeNum, employee.Sex, employee.Title);
                        MessageBox.Show(msg);
                    }
                }
            }
            else
            {
                if (lbx2.SelectedItem != null)//有关键字的话，显示lbx2
                {
                    foreach (var item in lbx2.SelectedItems)//判断lbx2有没有选中项
                    {
                        Employee employee = item as Employee;
                        string msg = string.Format("姓名：{0},工号:{1},性别:{2},职位:{3}", employee.EmployeeName, employee.EmployeeNum, employee.Sex, employee.Title);
                        MessageBox.Show(msg);
                    }
                }
            }
        }

        /// <summary>
        /// 关键字改变时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEmployeeKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtEmployeeKeyword.Text.Trim();
            if (string.IsNullOrEmpty(keyword))//无关键字，显示scv1下的listbox(有分组)
            {

                scv1.Visibility = Visibility.Visible;
                scv2.Visibility = Visibility.Collapsed;
            }
            else//有关键字，显示scv2下的listbox(无分组)
            {
                scv1.Visibility = Visibility.Collapsed;
                scv2.Visibility = Visibility.Visible;
            }
            CollectionViewSource employeeCvs = (CollectionViewSource)this.FindResource("employeeCollectionViewSource");
            employeeCvs.View.Refresh();//刷新View
        }
        /// <summary>
        /// 根据关键字(工号或姓名)筛选员工
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void employeeCollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            string keyword = txtEmployeeKeyword.Text.Trim();
            Employee employee = e.Item as Employee;
            if (employee != null)
            {
                if (string.IsNullOrEmpty(keyword))//无关键字，直接Accept
                {
                    e.Accepted = true;
                }
                else
                {
                    //有关键字、筛选员工号或姓名中包含关键字的员工
                    e.Accepted = employee.EmployeeNum.Contains(keyword) || employee.EmployeeName.Contains(keyword);
                }
            }
        }
        /// <summary>
        /// 清空关键字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearKeyword_Click(object sender, RoutedEventArgs e)
        {
            this.txtEmployeeKeyword.Clear();
        }
    }

    public class Employee : INotifyPropertyChanged
    {
        #region 实现更改通知
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        /// <summary>
        /// 重载ToString()方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.EmployeeNum + "  " + this.EmployeeName;
        }

        private string title;
        /// <summary>
        /// 职位
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        private string employeeName;
        /// <summary>
        /// 姓名
        /// </summary>
        public string EmployeeName
        {
            get { return employeeName; }
            set
            {
                employeeName = value;
                RaisePropertyChanged("EmployeeName");
            }
        }
        private string employeeNum;
        /// <summary>
        /// 工号
        /// </summary>
        public string EmployeeNum
        {
            get { return employeeNum; }
            set
            {
                employeeNum = value;
                RaisePropertyChanged("EmployeeNum");
            }
        }
        private string sex;
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex
        {
            get { return sex; }
            set
            {
                sex = value;
                RaisePropertyChanged("Sex");
            }
        }

    }
}
