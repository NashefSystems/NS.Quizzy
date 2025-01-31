using Microsoft.AspNetCore.Http;

namespace NS.Quizzy.Server.BL.CustomExceptions
{
    public class ConflictException : BaseCustomException
    {
        public ConflictException(string message) :
            base(StatusCodes.Status409Conflict, message)
        { }
    }
}
