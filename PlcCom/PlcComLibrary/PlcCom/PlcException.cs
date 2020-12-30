using System;
using System.Runtime.Serialization;

/*
https://docs.microsoft.com/en-us/dotnet/api/system.exception?view=netcore-3.0#implementing-custom-exceptions
*/

namespace PlcComLibrary.PlcCom
{
    [Serializable()]
    public class PlcException : Exception
    {
        private int _notAPrime;
        public int NotAPrime { get { return _notAPrime; } }

        protected PlcException() : base()
        { }

        public PlcException(int value) : base(String.Format("{0} is not a prime number.", value))
        {
            _notAPrime = value;
        }

        public PlcException(int value, string message) : base(message)
        {
            _notAPrime = value;
        }

        public PlcException(int value, string message, Exception innerException) : base(message, innerException)
        {
            _notAPrime = value;
        }

        protected PlcException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
