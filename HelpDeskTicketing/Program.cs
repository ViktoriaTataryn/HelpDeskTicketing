using System.Text;
using HelpDeskTicketing.Core.Interfaces;
using HelpDeskTicketing.Core.Services;
using HelpDeskTicketing.Filters;
using HelpDeskTicketing.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<HelpDeskContext>(opt =>
    opt.UseSqlServer("Server=DESKTOP-42S05RP;Database=HelpDeskTicketingDb;Integrated Security=True;TrustServerCertificate=True;"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)   
    .AddJwtBearer(options =>
    {
        /*options.TokenValidationParameters = new TokenValidationParameters()   
        {                 
            ValidateIssuerSigningKey = true,     
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(   
                builder?.Configuration.GetSection("AppSettings:Token").Value ?? string.Empty)),   
            ValidateIssuer = false,     
            ValidateAudience = false,
                
        };*/
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["AppSettings:Token"]!)), 
            ValidateIssuer = false,
            ValidateAudience = false
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"];
                Console.WriteLine($"Received Token: {token}");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITicketService, TicketService>();


builder.Services.AddControllers(opt => 
{ opt.Filters.Add<ErrorFilter>() ;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HelpDeskTicketing API", Version = "v1" });

    // Додавання підтримки Bearer токена в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", 
        Type = SecuritySchemeType.Http, //тип безпеки
        Scheme = "Bearer", //тип токена
        BearerFormat = "JWT", //формат токена
        In = ParameterLocation.Header, //місце де буде передаватись токен
        Description = "Введіть ваш токен в поле нижче.\n"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();