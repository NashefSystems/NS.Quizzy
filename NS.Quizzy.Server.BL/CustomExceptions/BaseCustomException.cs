namespace NS.Quizzy.Server.BL.CustomExceptions
{
    public class BaseCustomException : Exception
    {
        public readonly int StatusCode;

        public BaseCustomException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
