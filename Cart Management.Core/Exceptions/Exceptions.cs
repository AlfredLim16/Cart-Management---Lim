using System;

namespace Cart_Management.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }

    public class DataException : Exception
    {
        public DataException(string message) : base(message) { }
    }

    public class UserException : Exception
    {
        public UserException(string message) : base(message) { }
    }
}
