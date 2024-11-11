using Impostor.Api.Plugins;

namespace ImpostorBanPlugin;

public class BanPluginStartUp : IPluginHttpStartup
{
    public void ConfigureHost(IHostBuilder host)
    {
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<CountingManager>();
    }

    public void ConfigureWebApplication(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/api/counts", async context =>
            {
                var countingManager = context.RequestServices.GetRequiredService<CountingManager>();
                var result = new
                {
                    games = countingManager.GetTotalGames(),
                    players = countingManager.GetTotalPlayers()
                };
                await context.Response.WriteAsJsonAsync(result);
            });
        });
    }
}
