namespace UserAPI.Contracts.Response
{
    public sealed class RegisterUserResponse
    {
        public string UserId { get; }

        public RegisterUserResponse(string userId)
        {
            UserId = userId;
        }
    }
}