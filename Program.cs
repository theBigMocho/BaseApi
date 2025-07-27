using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BaseApi.Data;
using BaseApi.Services;
using BaseApi.Plugins.Core;
using BaseApi.Plugins.ClaudeCode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configurar Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Registrar servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Registrar sistema de plugins
builder.Services.AddSingleton<IPluginManager, PluginManager>();
builder.Services.AddScoped<IChatService, ChatService>();

// Registrar plugins específicos
builder.Services.AddScoped<ClaudeCodeChatPlugin>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inicializar sistema de plugins
using (var scope = app.Services.CreateScope())
{
    var pluginManager = scope.ServiceProvider.GetRequiredService<IPluginManager>();
    var claudeCodePlugin = scope.ServiceProvider.GetRequiredService<ClaudeCodeChatPlugin>();
    
    // Registrar plugins
    pluginManager.RegisterPlugin(claudeCodePlugin);
    
    // Habilitar plugins
    claudeCodePlugin.IsEnabled = true;
    
    // Inicializar plugins
    await pluginManager.InitializePluginsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configurar archivos estáticos
app.UseStaticFiles();

// Configurar página por defecto
app.UseDefaultFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Fallback para SPA - redirigir todas las rutas no encontradas al index.html
app.MapFallbackToFile("index.html");

app.Run();
