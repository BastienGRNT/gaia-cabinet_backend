using DotNetEnv;
using gaiacabinet_api.Database;
using Microsoft.EntityFrameworkCore;
using gaiacabinet_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace gaiacabinet_api;

public class Program
{
    public static void Main(string[] args)
    {
        //Charger le fichier .env
        Env.Load();
        
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // Injection de dépendance
        builder.Services.AddSingleton<IClock, SystemClock>();
        builder.Services.AddScoped<IAuthServices, AuthServices>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        // Connexion à la Base de donnée
        var DbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        if (string.IsNullOrEmpty(DbConnectionString))
        {
            throw new InvalidOperationException("DB_CONNECTION_STRING manquant (dotenv/ENV).");
        }
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(DbConnectionString));
        
        // Gérer les options des JWT
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddOptions<AuthJwtOptions>()
            .BindConfiguration("AuthJwt")
            .Validate(o => !string.IsNullOrWhiteSpace(o.SigningKey) && o.SigningKey.Length >= 32,
                "AuthJwt:SigningKey manquante ou trop courte (>= 32).")
            .ValidateOnStart();
        var jwt = builder.Configuration.GetSection("AuthJwt").Get<AuthJwtOptions>()!;
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new()
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        
        //Lancement du seeder pour les données utile en Dev
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DataSeeder.SeedDevelopmentData(db);
        }

        app.Run();
    }
}
