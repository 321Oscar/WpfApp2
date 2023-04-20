using System;
using System.Runtime.Serialization;

namespace CanControl.CANInfo
{
    [Serializable]
    public class USBCANOpenException : Exception
    {
        public USBCANOpenException()
        {
        }

        public USBCANOpenException(string message) : base(message)
        {
        }

        public USBCANOpenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected USBCANOpenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}