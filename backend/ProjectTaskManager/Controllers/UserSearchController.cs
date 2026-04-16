// Controllers/UserSearchController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.Services;

namespace Projecttaskmanager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // only admin can search & assign
    public class UserSearchController(IUserSearchService userSearchService) : ControllerBase
    {


        // Search users by prefix
        [HttpGet("search")]
        public IActionResult SearchUsers([FromQuery] string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return BadRequest("Prefix is required.");

            var result = userSearchService.SearchUsers(prefix);
            return Ok(result);
        }
    }
}