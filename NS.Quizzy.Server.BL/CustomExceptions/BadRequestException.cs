using Microsoft.AspNetCore.Http;

namespace NS.Quizzy.Server.BL.CustomExceptions
{
    public class BadRequestException : BaseCustomException
    {
        public BadRequestException(string message) :
            base(StatusCodes.Status400BadRequest, message)
        { }
    }
}
