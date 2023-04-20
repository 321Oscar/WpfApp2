using CanControl.CANInfo;
using ProtocolLib.Protocols.DBC;
using ProtocolLib.Signal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolLib.Protocols
{
    public abstract class BaseProtocol
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">协议类型</param>
        /// <param name="fileName">协议文档名称,以“;”分割</param>
        /// <returns></returns>
        public static List<BaseSignal> GetSingalsByProtocol(int t, string[] fileName)
        {
            List<BaseSignal> singals;
            BaseProtocol protocol;

            switch (t)
            {
                case (int)ProtocolType.DBC:
                    {
                        protocol = new DBCProtocol();
                    }
                    break;
                case (int)ProtocolType.Excel:
                    throw new Exception("Excel协议暂不支持");
                //case (int)ProtocolType.XCP:
                //    protocol = new XCPProtocol();
                //    break;
                default:
                    singals = null;
                    //LeapMessageBox.Instance.ShowInfo("不支持的协议");
                    return null;

            }

            List<BaseSignal> singalList = new List<BaseSignal>();
            List<BaseSignal> sigals = new List<BaseSignal>();
            for (int i = 0; i < fileName.Length; i++)
            {
                sigals = protocol.ProtocolFile(fileName[i]);
                if (sigals == null || sigals.Count == 0)
                    continue;
                foreach (var item in sigals)
                {
                    if (singalList.Find(x => x.SignalName == item.SignalName) == null)
                        singalList.Add(item);
                }
            }
            singals = singalList;


            return singals;
        }
        /// <summary>
        /// 协议名称
        /// </summary>
        public abstract string ProtocolName { get; set; }

        public string ProtocolClassPath;

        /// <summary>
        /// 解析报文中的信号
        /// </summary>
        /// <param name="can_msg"></param>
        /// <param name="singals"></param>
        /// <returns><see cref="Dictionary{SignalItem, string}"/>信号+值</returns>
        public abstract Dictionary<BaseSignal, string> Multip(CANRecieveFrame[] can_msg, List<BaseSignal> singals);
        public abstract void Multip(CANRecieveFrame[] can_msg, IEnumerable<BaseSignal> singals);

        /// <summary>
        /// 解析报文，yeild return
        /// </summary>
        /// <param name="can_msg"></param>
        /// <param name="singals"></param>
        /// <returns></returns>
        public abstract IEnumerable<BaseSignal> MultipYeild(CANRecieveFrame[] can_msg, List<BaseSignal> singals);

        /// <summary>
        /// 解析协议中的信号
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public abstract List<BaseSignal> ProtocolFile(string FilePath);

        /// <summary>
        /// 解析报文中的信号
        /// </summary>
        /// <param name="can_msg"></param>
        /// <param name="signalItem"></param>
        /// <returns>信号值</returns>
        public abstract string Single(CANRecieveFrame[] can_msg, BaseSignal signalItem);

        /// <summary>
        /// 将信号值组帧
        /// </summary>
        /// <param name="signalValue"></param>
        /// <returns></returns>
        public abstract CANSendFrame[] BuildFrames(Dictionary<BaseSignal, string> signalValue);
        public abstract CANSendFrame BuildFrame(BaseSignal signal, string value);

    }
}
