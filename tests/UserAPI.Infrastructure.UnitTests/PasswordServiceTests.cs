using NUnit.Framework;
using UserAPI.Application.Common.Abstraction.Service;
using UserAPI.Infrastructure.Service;

namespace UserAPI.Infrastructure.UnitTests
{
    public class PasswordServiceTests
    {
        private IPasswordService _passwordService;
        
        [SetUp]
        public void Setup()
        {
            _passwordService = new Pbkdf2PasswordService();
        }

        [Test]
        public void Should_GenerateSameHash_When_SameStringAndSameSaltPassed()
        {
            var str = "password123";
            var salt = "c2FsdDEyMw==";

            var result1 = _passwordService.GenerateHash(str, salt);
            var result2 = _passwordService.GenerateHash(str, salt);
            
            Assert.AreEqual(result1.Hash, result2.Hash);
        }
        
        [Test]
        public void Should_GenerateDifferentHash_When_SameStringAndDifferentSaltPassed()
        {
            var str = "password123";
        
            var result1 = _passwordService.GenerateHash(str);
            var result2 = _passwordService.GenerateHash(str);
            
            Assert.AreNotEqual(result1.Hash, result2.Hash);
        }
        
        [Test]
        public void Should_ValidateHash_When_PassedGeneratedHash()
        {
            var str = "password123";
        
            var result = _passwordService.GenerateHash(str);
            var validity = _passwordService.ValidateHash(result.Hash, str, result.Salt);

            Assert.IsTrue(validity);
        }
        
        [Test]
        public void ShouldNot_ValidateHash_When_PassedDifferentHash()
        {
            var str = "password123";
        
            var result = _passwordService.GenerateHash(str);
            var validity = _passwordService.ValidateHash(result.Hash + ".", str, result.Salt);

            Assert.IsFalse(validity);
        }
        
        [Test]
        public void ShouldNot_ValidateHash_When_PassedGeneratedHashWithDifferentSalt()
        {
            var str = "password123";
            var salt = "c2FsdDEyMw==";
            
            var result = _passwordService.GenerateHash(str);
            var validity = _passwordService.ValidateHash(result.Hash, str, salt);

            Assert.IsFalse(validity);
        }
    }
}