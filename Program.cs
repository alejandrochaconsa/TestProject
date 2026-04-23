using TestProject.Services;

namespace TestProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
                {
                    // This is so that we serialize Enums as string
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            // Registering Services so the can be dependency injected
            builder.Services.AddScoped<IItemService, ItemService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            // Middleware to point to a default file in a directory within wwwroot
            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}