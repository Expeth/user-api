namespace UserAPI.Domain.ValueObject
{
    public sealed class PasswordHash
    {
        public string Hash { get; }
        public string Salt { get; }

        public PasswordHash(string hash, string salt)
        {
            Hash = hash;
            Salt = salt;
        }
    }
}