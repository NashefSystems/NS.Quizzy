using Microsoft.AspNetCore.Http;

namespace NS.Quizzy.Server.BL.CustomExceptions
{
    public class UnauthorizedException : BaseCustomException
    {
        public UnauthorizedException(string message) :
            base(StatusCodes.Status401Unauthorized, message)
        { }
    }
}
