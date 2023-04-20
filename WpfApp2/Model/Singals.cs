using ProtocolLib.Signal;
using System.Collections.Generic;

namespace WpfApp2.Model
{
    public class Singals
    {
        /// <summary>
        /// 
        /// </summary>
        public List<DBCSignal> Signal { get; set; }

        public Singals()
        {
            Signal = new List<DBCSignal>();
        }

    }

}
