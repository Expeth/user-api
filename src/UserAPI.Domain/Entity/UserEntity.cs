namespace UserAPI.Domain.Entity
{
    public class UserEntity
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUri { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public int PasswordHashingAlgorithm { get; set; }
        public string Email { get; set; }

        public UserEntity(string id, string firstName, string middleName, string lastName, string profilePictureUri,
            string username, string passwordHash, string passwordSalt, int passwordHashingAlgorithm, string email)
        {
            Id = id;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            ProfilePictureUri = profilePictureUri;
            Username = username;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            Email = email;
            PasswordHashingAlgorithm = passwordHashingAlgorithm;
        }
    }
}