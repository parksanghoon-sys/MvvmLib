namespace wpfCodeCheck.Main.Local.Exceptions
{
    public class InsufficientDataException : Exception
    {
        public InsufficientDataException()
        {
            
        }
        public InsufficientDataException(string message)
            :base(message) 
        {
            
        }
        public InsufficientDataException(string message, Exception innerException)
     : base(message, innerException)
        {
        }
    }
}
