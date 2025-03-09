using Microsoft.AspNetCore.Http;

namespace NS.Quizzy.Server.BL.CustomExceptions
{
    public class ForbiddenException : BaseCustomException
    {
        public ForbiddenException(string message) :
            base(StatusCodes.Status403Forbidden, message)
        { }
    }
}
