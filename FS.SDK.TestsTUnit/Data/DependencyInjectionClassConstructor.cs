using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core;
using TUnit.Core.Interfaces;

namespace FS.SDK.TestsTUnit;


public class MicrosoftDependencyInjectionDataSourceAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider ServiceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return ServiceProvider.CreateAsyncScope();
    }

    public override object? Create(IServiceScope scope, Type type)
    {
        return scope.ServiceProvider.GetService(type);
    }

    private static IServiceProvider CreateSharedServiceProvider()
    {
        var builder = new ConfigurationBuilder()
        //.SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Microsoft.Extensions.Configuration.IConfiguration config = builder.Build();
                
        return new ServiceCollection()
            .AddSingleton<FSClient>()
            .AddSingleton<FSApiClient>()
            .AddSingleton<FSApiClientConfig>(sp =>
            {
                var configuration = sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
                var apiKeyName = configuration["apikeyname"];
                var apiKey = configuration["apikey"];
                var baseUrl = configuration["baseurl"] ?? "https://api.fellesstudentsystem.no/graphql";
                return new FSApiClientConfig
                {
                    ApiKeyName = apiKeyName ?? throw new ArgumentNullException("apikeyname"),
                    ApiKey = apiKey ?? throw new ArgumentNullException("apikey"),
                    BaseUrl = baseUrl
                };
            })
            .AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(config)
            //.AddTransient<SomeClass3>()
            .BuildServiceProvider();
    }
}
