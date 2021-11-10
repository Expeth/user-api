namespace UserAPI.Contracts.Request
{
    public sealed class AuthenticateUserRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public AuthenticateUserRequest(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}