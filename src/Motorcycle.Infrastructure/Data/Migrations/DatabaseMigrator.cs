using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Motorcycle.Infrastructure.Data.Context;

namespace Motorcycle.Infrastructure.Data.Migrations;

public static class DatabaseMigrator
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<MotorcycleDbContext>>();
            try
        {
            var context = services.GetRequiredService<MotorcycleDbContext>();
            var retry = 0;
            const int maxRetries = 10;
            
            logger.LogInformation("Iniciando migração do banco de dados");
            
            while (retry < maxRetries)
            {
                try
                {
                    context.Database.Migrate();
                    logger.LogInformation("Migração do banco de dados concluída com sucesso");
                    break;
                }
                catch (Exception ex)
                {
                    retry++;
                    if (retry >= maxRetries)
                        throw;
                        
                    logger.LogWarning(ex, "Erro ao migrar o banco de dados. Tentativa {Retry} de {MaxRetries}. Aguardando 5 segundos.", retry, maxRetries);
                    Thread.Sleep(5000); // Aguardar 5 segundos antes de tentar novamente
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro durante a migração do banco de dados");
            throw;
        }
        }

        return host;
    }
}