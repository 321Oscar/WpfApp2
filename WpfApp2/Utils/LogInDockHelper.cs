using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.View;

namespace WpfApp2.Utils
{
    public class LogInDockHelper
    {
        public static void ShowInDock(string log,string dockFormName)
        {
            var forms = Application.Current.Windows;
            foreach (var item in forms)
            {
                if (item is DockForm d)
                {
                    if (d.Name == dockFormName)
                    {
                        d.ShowLog(log);
                    }
                }
            }
        }

        public delegate void PrintLog(string log);

        public static Dictionary<string, PrintLog> PrintLogFuncs { get; set; } = new Dictionary<string, PrintLog>();
    }
}
