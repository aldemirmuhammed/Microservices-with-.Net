using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using System;

namespace FreeCourse.Services.Discount.Extension
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postresql database.");

                    var retry = Policy.Handle<NpgsqlException>()
                            .WaitAndRetry(
                                retryCount: 5,
                                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2,4,8,16,32 sc
                                onRetry: (exception, retryCount, context) =>
                                {
                                    logger.LogError($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                                });

                    //if the postgresql server container is not created on run docker compose this
                    //migration can't fail for network related exception. The retry options for database operations
                    //apply to transient exceptions                    
                    retry.Execute(() => ExecuteMigrations(configuration));

                    logger.LogInformation("Migrated postresql database.");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the postresql database");
                }
            }

            return host;
        }

        private static void ExecuteMigrations(IConfiguration configuration)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>("ConnectionStrings:PostgreSql"));
            connection.Open();

            using var command = new NpgsqlCommand
            {
                Connection = connection
            };

            command.CommandText = "DROP TABLE IF EXISTS Discount";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE Discount(Id SERIAL PRIMARY KEY, 
                                                                UserId VARCHAR(100) NOT NULL,
                                                                Rate smallint not null,
                                                                Code varchar(50) not null,
                                                                CreatedDate timestamp not null default CURRENT_TIMESTAMP)";
            command.ExecuteNonQuery();


            command.CommandText = "INSERT INTO Discount(UserId, Rate, Code) VALUES('728ab5d7-b82a-4861-94df-35f06deabbbb', '50', 'ABC');";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Discount(UserId, Rate, Code) VALUES('728ab5d7-b82a-4861-94df-35f06deabbbb', '30', 'EFG');";
            command.ExecuteNonQuery();
        }
    }
}
