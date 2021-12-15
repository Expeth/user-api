namespace UserAPI.Contracts.Response
{
    public sealed class RefreshJwtResponse
    {
        public string Jwt { get; set; }
        public string RenewToken { get; set; }

        public RefreshJwtResponse(string jwt, string renewToken)
        {
            Jwt = jwt;
            RenewToken = renewToken;
        }
    }
}