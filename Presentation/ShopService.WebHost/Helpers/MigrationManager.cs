using Microsoft.EntityFrameworkCore;
using ShopService.Infrastructure.EntityFramework;

namespace ShopService.WebHost.Helpers;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        return host;
    }
}

