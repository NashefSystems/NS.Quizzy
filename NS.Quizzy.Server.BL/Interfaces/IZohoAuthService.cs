namespace NS.Quizzy.Server.BL.Interfaces
{
    internal interface IZohoAuthService
    {
        string GetAuthorizationUrl();
        Task<string> ExchangeCodeForRefreshToken(string authCode);
        Task<string> GetAccessToken();
    }
}
