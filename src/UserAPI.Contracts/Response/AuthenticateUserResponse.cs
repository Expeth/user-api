namespace UserAPI.Contracts.Response
{
    public sealed class AuthenticateUserResponse
    {
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }

        public AuthenticateUserResponse(string jwt, string refreshToken)
        {
            Jwt = jwt;
            RefreshToken = refreshToken;
        }
    }
}