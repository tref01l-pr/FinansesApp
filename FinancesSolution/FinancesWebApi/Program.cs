using System.Text.Json.Serialization;
using FinancesWebApi.Data;
using FinancesWebApi.Interfaces;
using FinancesWebApi.Interfaces.Services;
using FinancesWebApi.Repositories;
using FinancesWebApi.Services;
using Microsoft.EntityFrameworkCore;

namespace FinancesWebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddTransient<Seed>();
        builder.Services.AddTransient<IJwtService, JwtService>();
        builder.Services.AddTransient<IMaskConverter, MaskConverter>();
        builder.Services.AddControllers().AddJsonOptions(o =>
            o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        var app = builder.Build();

        if (args.Length == 1 && args[0].ToLower() == "--seeddata")
            SeedData(app);

        void SeedData(IHost app)
        {
            var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

            using (var scope = scopedFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<Seed>();
                service.SeedDataContext();
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.MapControllers();

        app.UseAuthorization();

        app.Run();
    }
}