using Xunit;
using System.Linq;

namespace ProjectTaskManager.Tests.Validators
{
    public static class AuthValidator
    {
        public static bool IsUsernameValid(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && username.Length >= 3;
        }

        public static bool HasSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            return input.Any(ch => !char.IsLetterOrDigit(ch));
        }

        public static bool IsPasswordValid(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 4;
        }
    }

    public class AuthValidatorTests
    {
        [Fact]
        public void Username_Should_Not_Be_Null()
        {
            var result = AuthValidator.IsUsernameValid(null);
            Assert.False(result);
        }

        [Fact]
        public void Username_Should_Be_Valid()
        {
            var result = AuthValidator.IsUsernameValid("tulasi");
            Assert.True(result);
        }

        [Fact]
        public void Username_Should_Be_Invalid_When_TooShort()
        {
            var result = AuthValidator.IsUsernameValid("ab");
            Assert.False(result);
        }

        [Fact]
        public void Username_Should_Contain_SpecialCharacters()
        {
            var result = AuthValidator.HasSpecialCharacters("tulasi@123");
            Assert.True(result);
        }

        [Fact]
        public void Username_Should_Not_Have_SpecialCharacters()
        {
            var result = AuthValidator.HasSpecialCharacters("tulasi123");
            Assert.False(result);
        }

        [Fact]
        public void Password_Should_Not_Be_Null()
        {
            var result = AuthValidator.IsPasswordValid(null);
            Assert.False(result);
        }

        [Fact]
        public void Password_Should_Be_Valid()
        {
            var result = AuthValidator.IsPasswordValid("1234");
            Assert.True(result);
        }
    }
}