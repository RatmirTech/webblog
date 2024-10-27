using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace webblog.backend.BlogApi.Data
{
    /// <summary>
    /// Регистрация зависимостей проекта
    /// </summary>
    public static class Entry
    {
        /// <summary>
        /// Добавить подключения бд
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionStringPgsql"></param>
        /// <returns></returns>
        public static IServiceCollection AddConnections(
            this IServiceCollection services,
            string connectionStringPgsql)
        {
            if (services is null)
            {
                throw new System.ArgumentNullException(nameof(services));
            }

            if (connectionStringPgsql is null)
            {
                throw new System.ArgumentNullException(nameof(connectionStringPgsql));
            }

            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseNpgsql(connectionStringPgsql);
            });

            services.AddScoped<IDbConnectionFactory>(_ =>
                new DbConnectionFactory(connectionStringPgsql));
            return services;
        }

        public interface IDbConnectionFactory
        {
            IDbConnection GetDbConnection(DatabaseType dbType);
        }

        public class DbConnectionFactory : IDbConnectionFactory
        {
            private readonly string _pgsqlConnectionString;

            public DbConnectionFactory(string pgsqlConnectionString)
            {
                _pgsqlConnectionString = pgsqlConnectionString;
            }

            public IDbConnection GetDbConnection(DatabaseType dbType)
            {
                try
                {
                    return dbType switch
                    {
                        DatabaseType.PostgreSQL => new NpgsqlConnection(_pgsqlConnectionString),
                        _ => throw new ArgumentOutOfRangeException(nameof(dbType), $"Not supported database type: {dbType}"),
                    };
                }
                catch (Exception ex) when (ex is NpgsqlException)
                {
                    // Возвращаем пользователю обобщенное сообщение об ошибке
                    throw new InvalidOperationException("Ошибка сервера. Обратитесь в тех поддержку.");
                }
            }

        }

        public enum DatabaseType
        {
            PostgreSQL
        }
    }
}
