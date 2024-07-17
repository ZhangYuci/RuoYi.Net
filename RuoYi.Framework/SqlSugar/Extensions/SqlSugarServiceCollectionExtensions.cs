using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// SqlSugar 拓展类
/// </summary>
public static class SqlSugarServiceCollectionExtensions
{
    // 注册 SqlSugarScope
    public static IServiceCollection AddSqlSugarScope(this IServiceCollection services, ConnectionConfig[] configs, Action<SqlSugarClient> buildAction)
    {
        // 注册 SqlSugar 
        // SqlSugarScope 必须使用单例
        services.AddSingleton<ISqlSugarClient>(u =>
        {
            var sqlSugarScope = new SqlSugarScope(configs.ToList(), buildAction);
            return sqlSugarScope;
        });

        // 注册非泛型仓储
        services.AddScoped<ISqlSugarRepository, SqlSugarRepository>();

        // 注册 SqlSugar 仓储
        services.AddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));

        return services;
    }
}