using Projecttaskmanager.DTOs;
namespace ProjectTaskManager.Tests.MockData
{
    public static class AuthMockData
    {
        public static UserResponce ValidUser()
        {
            return new UserResponce
            {
                Username = "testuser",
                Password = "1234"
            };
        }

        public static UserResponce InvalidUser()
        {
            return new UserResponce
            {
                Username = "",
                Password = ""
            };
        }
    }
}