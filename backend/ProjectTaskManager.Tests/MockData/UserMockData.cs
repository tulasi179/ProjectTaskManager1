using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;

namespace ProjectTaskManager.Tests.MockData;

public class UserMockData
{
    public static List<UserResponce> GetUsers()
    {
        return new List<UserResponce>
        {
            new UserResponce
            {
                Id =1,
                Username= "tulasi",
                Email = "Tulasi@test.com",
                Role = "User",
                IsActive =true
            },
            new UserResponce
            {
                Id = 2,
                Username = "Nikhita",
                Email = "Nikhita@test.com",
                Role="User",
                IsActive= true
            },
            new UserResponce
            {
                Id = 3,
                Username = "tejaswi",
                Email = "tejaswi@test.com",
                Role = "User",
                IsActive = false
            }
        };
    }

    public static UserResponce GetSingleUser(int id) =>
    GetUsers().First(u =>u.Id ==id);

       public static Users GetUserEntity(int id = 1) => new()
    {
        Id           = id,
        Username     = "tulasi",
        Email        = "Tulasi@test.com",
        Role         = "User",
        PasswordHash = "hashed_password_123"
    };
 
    public static Users GetUserEntityByEmail(string email = "Tulasi@test.com") => new()
    {
        Id           = 1,
        Username     = "tulasi",
        Email        = email,
        Role         = "User",
        PasswordHash = "hashed_password_123"
    };
    
}