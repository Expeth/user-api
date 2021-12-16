using System;

namespace UserAPI.Infrastructure.Model
{
    public class JwtCfg
    {
        public string PublicKeyFileLocation { get; set; }
        public string PrivateKeyFileLocation { get; set; }
        public TimeSpan Lifetime { get; set; }
    }
}