using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using WorkManagement.Data;
using WorkManagement.Service;
using WorkManagement.Interfaces;
using WorkManagement.Services;
using WorkManagement.Hubs;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .WithExposedHeaders("Content-Disposition")
               .SetIsOriginAllowed(origin => true));
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddDbContext<WorkManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký các service
// Thêm SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectTaskService, ProjectTaskService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IStatusJobService,StatusJobService>();
builder.Services.AddScoped<ITaskEmployeeService, TaskEmployeeService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddSingleton<IUserConnectionManager, UserConnectionManager>();

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads",
    OnPrepareResponse = ctx =>
    {
        {
            ctx.Context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
            ctx.Context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
            ctx.Context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
        }
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowLocalhost");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map route cho SignalR
app.MapHub<NotificationHub>("/notificationHub");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
