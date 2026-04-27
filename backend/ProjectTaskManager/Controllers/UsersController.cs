using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.Models;
using Projecttaskmanager.Services;
using Projecttaskmanager.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Projecttaskmanager.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUsersService service) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<UserResponce>>> GetUsers()
        => Ok(await service.GetAllUsersAsync());


    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponce>> GetUser(int id)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");
//?
        if (!isAdmin && currentUserId != id)
            return Forbid();
        var user = await service.GetUserByIdAsync(id); // throws 404 if not found
        return Ok(user);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Users>> CreateUser(UserResponce user)
    {
        var newUser = new Users
        {
            Username = user.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
            Email = user.Email,
            Role = user.Role
        };
        var createdUser = await service.AddUsersAsync(newUser);
        return Ok(createdUser);
    }
    

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserResponce dto)
    {
        await service.UpdateUserAysnc(id, dto); // throws 404 if not found
        return Ok("User updated successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await service.DeleteUsersAysnc(id); // throws 404 if not found
        return Ok("User deleted successfully");
    }

    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await service.ChangePasswordAsync(currentUserId, dto);
        return Ok("Password changed successfully.");
    }
}