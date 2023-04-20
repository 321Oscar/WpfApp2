using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Utils
{
    public interface IDataUCClosing : IDisposable
    {
        public void Closing();
    }
}
