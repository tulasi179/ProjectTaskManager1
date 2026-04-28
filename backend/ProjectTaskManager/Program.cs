using Projecttaskmanager.Data;
using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Services;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Projecttaskmanager.Middleware;
using System.Text;
using Projecttaskmanager.Repositories;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers()//401
.AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = 
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddOpenApi();


builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["AppSettings:Audience"],
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
                ValidateIssuerSigningKey = true  ,     
                ClockSkew = TimeSpan.Zero// strict expirey token. instead of 5 min
            };
        });
//Scoped means that thing live out through the whole request.
//until it return the final request
builder.Services.AddSingleton<IUserSearchService, UserSearchService>();
builder.Services.AddScoped<IUsersService, UsersServices>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); 
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskDependencyRepository, TaskDependencyRepository>();

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskDependencyService, TaskDependencyService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
//DI(Dependency Injection)
//when ever something wants to inject the  IUserService then it will auotomatically
// get the UsersService implementation.



//frontend runs on different prot and backend runs on different port 
//CORS help frontend to call the apis
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://agreeable-dune-0e39f5a00.1.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Auto-build trie on startup
//deploy error so adding try catch.
try
{
    using var scope = app.Services.CreateScope();// temporory scope for trie
    var userSearchService = scope.ServiceProvider.GetRequiredService<IUserSearchService>();//gets the service
    await userSearchService.BuildTrieAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Trie build failed: {ex.Message}. App will continue.");
}

// Configure the HTTP request pipeline.
//only runns locally setup the scalar
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.MapScalarApiReference();
    //both do the same thing displays the api doc

}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
//degugging.