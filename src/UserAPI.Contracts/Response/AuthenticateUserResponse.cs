namespace UserAPI.Contracts.Response
{
    public sealed class AuthenticateUserResponse
    {
        public string Jwt { get; set; }
        public string RenewToken { get; set; }

        public AuthenticateUserResponse(string jwt, string renewToken)
        {
            Jwt = jwt;
            RenewToken = renewToken;
        }
    }
}