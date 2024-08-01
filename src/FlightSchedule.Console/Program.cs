using FlightSchedule.Application.Services;
using FlightSchedule.Domain.Repositories;
using FlightSchedule.Domain.Services;
using FlightSchedule.Infrastructure.Csv;
using FlightSchedule.Infrastructure.Data;
using FlightSchedule.Infrastructure.Repositories;
using FlightSchedule.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FlightSchedule.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: FlightSchedule.ConsoleApp.exe <start-date> <end-date> <agency-id>");
                return;
            }

            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            await InitializeDatabase(services);

            await ExportToCsvAsync(services, args);

            Console.ReadLine();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer("Server=.;Database=FlightSchedule.Temp;User ID=sa;Password=123123;MultipleActiveResultSets=true;TrustServerCertificate=True"));

                services.AddScoped<IUnitOfWork, UnitOfWork>();

                services.AddScoped<IRouteRepository, RouteRepository>();
                services.AddScoped<IFlightRepository, FlightRepository>();
                services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

                services.AddScoped(typeof(IBulkService<>), typeof(BulkService<>));

                services.AddScoped<FlightChangeDetectionService>();
                services.AddScoped<CsvImporter>();
                services.AddScoped<CsvExport>();
            });

        static async Task InitializeDatabase(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Initialize database...");
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var csvImporter = serviceProvider.GetRequiredService<CsvImporter>();
            await csvImporter.Import();
        }

        static async Task ExportToCsvAsync(IServiceProvider serviceProvider, string[] args)
        {
            var startDate = DateTime.Parse(args[0]);
            var endDate = DateTime.Parse(args[1]);
            var agencyId = int.Parse(args[2]);

            var csvExport = serviceProvider.GetRequiredService<CsvExport>();
            var flightChangeDetectionAppService = serviceProvider.GetRequiredService<FlightChangeDetectionService>();

            var changes = await flightChangeDetectionAppService.DetectChangesAsync(startDate, endDate, agencyId);

            var path = await csvExport.Export(changes);
           
            Console.WriteLine($"Download path: {path}");
        }
    }
}
