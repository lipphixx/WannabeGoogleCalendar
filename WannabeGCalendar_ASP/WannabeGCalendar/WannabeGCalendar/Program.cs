using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WannabeGCalendar.Configuration;
using WannabeGCalendar.Data;

namespace WannabeGCalendar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySQL(connectionString);
            });

            // P�id�n� CORS politiky spr�vn�
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddTransient<IMailService, MailService>();
            
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseStaticFiles();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            //app.UseHttpsRedirection();
            app.UseAuthorization();

            // Pou�it� CORS politiky
            app.UseCors("AllowAll");
            app.MapControllers();

            app.Urls.Add("https://0.0.0.0:5050");
            
            app.Run();
        }
    }
}
