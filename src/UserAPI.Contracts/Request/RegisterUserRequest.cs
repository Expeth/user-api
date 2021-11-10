namespace UserAPI.Contracts.Request
{
    public sealed class RegisterUserRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public RegisterUserRequest(string email, string username, string password, string firstName, string lastName,
            string middleName)
        {
            Email = email;
            Username = username;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
        }
    }
}