using DotNetEnv;
using gaiacabinet_api.Database;
using Microsoft.EntityFrameworkCore;
using gaiacabinet_api.Services;

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
        // Connexion à la Base de donnée
        var DbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        if (string.IsNullOrEmpty(DbConnectionString))
        {
            throw new InvalidOperationException("DB_CONNECTION_STRING manquant. Vérifie ton .env");
        }
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(DbConnectionString));
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

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
