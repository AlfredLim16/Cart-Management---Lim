using System;

namespace CartManagementBusinessLogic.Exceptions
{
    public class CartExceptions : Exception
    {
        public CartExceptions() { }
        public CartExceptions(string message) : base(message) { }
        public CartExceptions(string message, Exception innerException) : base(message, innerException) { }
    }
}
