// Programme.cs — version originale, uniquement commentée bloc par bloc (aucune modification de code)

using DotNetEnv;
using gaiacabinet_api.Common;
using gaiacabinet_api.Controllers;
using gaiacabinet_api.Database;
using gaiacabinet_api.Interfaces;
using gaiacabinet_api.Middlewares;
using gaiacabinet_api.Options;
using Microsoft.EntityFrameworkCore;
using gaiacabinet_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace gaiacabinet_api;

public class Program
{
    public static void Main(string[] args)
    {
        // Charge les variables d'environnement depuis un fichier .env (utile en dev).
        Env.Load();
        
        // Création du builder ASP.NET Core (point d'entrée de la configuration).
        var builder = WebApplication.CreateBuilder(args);
        
        // Enregistrement des contrôleurs MVC.
        builder.Services.AddControllers();
        // Découverte des endpoints pour Swagger.
        builder.Services.AddEndpointsApiExplorer();
        // Génération de la doc OpenAPI/Swagger (activée plus bas en dev).
        builder.Services.AddSwaggerGen();
        // Autorisation (policies/roles peuvent être ajoutés ici).
        builder.Services.AddAuthorization();
        
        // --- Injection de dépendances (DI) ---
        // IClock en Singleton : OK (stateless, partagé).
        builder.Services.AddSingleton<IClock, SystemClock>();
        builder.Services.AddSingleton<HashUtils>();
        // Services applicatifs en Scoped : cohérent avec le cycle de requête HTTP.
        builder.Services.AddScoped<IAuthServices, AuthServices>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        
        builder.Services.AddScoped<DemoService>();
        
        // --- Connexion à la base de données ---
        // Récupération de la chaîne de connexion depuis l'environnement.
        // Si manquante, on lève une exception explicite (fail fast).
        var DbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        if (string.IsNullOrEmpty(DbConnectionString))
        {
            throw new InvalidOperationException("DB_CONNECTION_STRING manquant (dotenv/ENV).");
        }
        // Enregistrement du DbContext EF Core avec le provider PostgreSQL (Npgsql).
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(DbConnectionString));
        
        // --- Configuration des options JWT ---
        // Charge les variables d'environnement dans la configuration.
        builder.Configuration.AddEnvironmentVariables();
        // Lie la section "AuthJwt" à AuthJwtOptions et valide la clé de signature.
        builder.Services.AddOptions<AuthJwtOptions>()
            .BindConfiguration("AuthJwt")
            .Validate(o => !string.IsNullOrWhiteSpace(o.SigningKey) && o.SigningKey.Length >= 32,
                "AuthJwt:SigningKey manquante ou trop courte (>= 32).")
            .ValidateOnStart();
        
        builder.Services.AddOptions<OtpPepperOptions>()
            .BindConfiguration("OtpPepper")
            .Validate(o => !string.IsNullOrWhiteSpace(o.OtpPepper) && o.OtpPepper.Length >= 32,
                "OtpPepper manquant ou trop court (>= 32).")
            .ValidateOnStart();
        
        // Récupère les options JWT pour initialiser l'authentification.
        var jwt = builder.Configuration.GetSection("AuthJwt").Get<AuthJwtOptions>()!;
        
        // Active l'authentification JWT Bearer avec validation stricte.
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
                    ClockSkew = TimeSpan.Zero // Skew nul : tokens expirés immédiatement à l'échéance (très strict).
                };
            });
        
        
        // --- CORS ---
        // Politique pour autoriser le front en dev (localhost:3000) avec credentials.
        // En prod : origin(s) à sortir en config et limiter finement.
        builder.Services.AddCors(o => o.AddPolicy("FrontDev", p =>
            p.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
        ));
        
        // Construction de l'application (pipeline).
        var app = builder.Build();
        
        // Swagger uniquement en environnement de développement.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseMiddleware<ErrorHandlingMiddleware>();

        // Active la politique CORS avant les middlewares d'auth.
        app.UseCors("FrontDev");
        // Redirection HTTP -> HTTPS (sécurité).
        app.UseHttpsRedirection();
        // Authentification puis autorisation (ordre correct).
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Mappe les contrôleurs (routes REST).
        app.MapControllers();
        
        // --- Seed de données de développement ---
        // En dev, initialise des données utiles (non exécuté en prod).
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DataSeeder.SeedDevelopmentData(db);
        }

        // Démarre l'application web.
        app.Run();
    }
}
