namespace UserAPI.Contracts.Response
{
    public sealed class RefreshJwtResponse
    {
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }

        public RefreshJwtResponse(string jwt, string refreshToken)
        {
            Jwt = jwt;
            RefreshToken = refreshToken;
        }
    }
}