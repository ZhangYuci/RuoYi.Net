using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        const string ruoyi_format_string = "\r\n  " +
             "_____         __     ___     _   _ ______ _______ \r\n " +
             "|  __ \\        \\ \\   / (_)   | \\ | |  ____|__   __|\r\n " +
             "| |__) |   _  __\\ \\_/ / _    |  \\| | |__     | |   \r\n " +
             "|  _  / | | |/ _ \\   / | |   | . ` |  __|    | |   \r\n " +
             "| | \\ \\ |_| | (_) | |  | |  _| |\\  | |____   | |   \r\n " +
             "|_|  \\_\\__,_|\\___/|_|  |_| (_)_| \\_|______|  |_|   \r\n" +
             "\r\n";

        var builder = WebApplication.CreateBuilder(args).Inject();
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            //Set properties and call methods on options
            //serverOptions.Limits.MaxRequestBodySize = 50 * 1024 * 1024;
            //serverOptions.Listen(IPAddress.IPv6Any,5000); // 监听所有 IPv6 地址
            //serverOptions.Listen(IPAddress.Any, 5000);      // 监听所有 IPv4 地址
            serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(3);
            serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
        });

        // 用AspectCore替换默认的IOC容器, 用于AOP拦截, 如 事务拦截器: TransactionalAttribute 
        builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());

        var app = builder.Build();

        var logger = app.Services.GetService<ILogger<Program>>();

        logger.LogInformation(ruoyi_format_string);

        app.Run();
    }
}