using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WannabeGCalendar.Configuration;
using WannabeGCalendar.Data;

namespace WannabeGCalendar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Konfigurace připojení k databázi
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySQL(connectionString);
            });

            // Konfigurace CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // Konfigurace mailových služeb
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddTransient<IMailService, MailService>();

            // Přidání JWT autentizace
            var key = Encoding.ASCII.GetBytes("YourSuperSecretKey"); // Změňte na bezpečný klíč
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // Přidání kontrolerových služeb
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Konfigurace statických souborů
            app.UseStaticFiles();

            // Konfigurace HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            // Přidání autentizace a autorizace
            app.UseAuthentication();
            app.UseAuthorization();

            // Používání CORS politiky
            app.UseCors("AllowAll");

            // Mapování kontrolerů
            app.MapControllers();

            // Nastavení URL aplikace
            app.Urls.Add("https://0.0.0.0:5050");

            app.Run();
        }
    }
}