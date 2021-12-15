namespace UserAPI.Contracts.Request
{
    public sealed class RefreshJwtRequest
    {
        public string RefreshToken { get; set; }

        public RefreshJwtRequest(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}