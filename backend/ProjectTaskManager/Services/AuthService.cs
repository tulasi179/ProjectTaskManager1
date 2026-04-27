using Projecttaskmanager.Data;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
namespace Projecttaskmanager.Services;


public class AuthService(AppDbContext context , IConfiguration configuration) :IAuthService
{
    public async Task<(TokenResponce? Token, string? Error)> LoginAsync(UserResponce request)
    {
        var user = await context.User.FirstOrDefaultAsync(u => u.Username == request.Username);
        
        if (user is null)
            return (null, "Invalid username or password.");

        if (!user.IsActive)
            return (null, "Please verify your email before logging in.");

        if (new PasswordHasher<Users>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return (null, "Invalid username or password.");

        return (await CreateTokenResponse(user), null);
    }
    private async Task<TokenResponce> CreateTokenResponse(Users user)
    {
        return new TokenResponce
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)

        };
    }

  public async Task<Users?> RegisterAsync(UserResponce request)
{
    if (await context.User.AnyAsync(u => u.Username == request.Username))
        return null;

    var user = new Users
    {
        Username = request.Username,
        Email = request.Email,
        Role = request.Role,
        IsActive = false
    };

    user.PasswordHash = new PasswordHasher<Users>().HashPassword(user, request.Password);

    context.User.Add(user);
    await context.SaveChangesAsync();
    return user;
}
        //This runs when the access token expires.
     public async Task<TokenResponce?> RefreshTokensAsync(RefreshTokenRequestDto request)
    {
       var user = await ValidateRefreshTokenAsync(request.UserId , request.RefreshToken);
       if(user is null)
       return null;

       return await CreateTokenResponse(user);
    }


    private async Task<Users?> ValidateRefreshTokenAsync(int userId , string refreshToken)
    {
       var user = await context.User.FindAsync(userId);
       if(user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <=DateTime.UtcNow)
        {
            return null;
        } 
        return user;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];//256 Bits security
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(Users users)
    {
        var refreshToken = GenerateRefreshToken();
        users.RefreshToken = refreshToken;
        users.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync();
        return refreshToken;
    }
     private string CreateToken(Users user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            //Loads secret key from appsettings.json.
            //configuration.GetValue<string>("AppSettings:Token")This reads the secret key from your configuration file.
            //Encoding.UTF8.GetBytes(...)  JWT cryptographic functions cannot work with strings directly.They require byte arrays.So this converts the string into bytes.
            //new SymmetricSecurityKey(...) Now those bytes are used to create a security key object.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            //This line tells the system: How the token should be signed.
            //This class holds the information needed to sign the token. It needs two things:1️ Security key,2️ Signing algorithm


            //creates Jwt Object.
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires : DateTime.UtcNow.AddMinutes(10),
                signingCredentials :creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await context.User
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user is null)
                throw new KeyNotFoundException("User not found.");

            user.PasswordHash = new PasswordHasher<Users>().HashPassword(user, dto.NewPassword);
            await context.SaveChangesAsync();
        }

       
}