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
    public sealed class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<ProjectItem> ProjectItems { get; }

        public MainWindowViewModel()
        {
            var projects = RootHelper.InitRootByJson(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\Project.json");
            ProjectItems = new ObservableCollection<ProjectItem>(projects.project);
        }

        public void OpenProject()
        {

        }

        public void AddProject()
        {
            AddProjectWindow apw = new AddProjectWindow();
            if (apw.ShowDialog() == true)
            {
                ProjectItems.Add(((ProjectViewModel)apw.DataContext).ProjectItem);
            }
        }
    }
}
