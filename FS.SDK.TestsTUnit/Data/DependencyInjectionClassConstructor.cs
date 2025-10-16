using TUnit.Core.Interfaces;

namespace FS.SDK.TestsTUnit;

public class DependencyInjectionClassConstructor : IClassConstructor
{
    public Task<object> Create(Type type, ClassConstructorMetadata classConstructorMetadata)
    {
        Console.WriteLine(@"You can also control how your test classes are new'd up, giving you lots of power and the ability to utilise tools such as dependency injection");

        string apiKeyName = "X-Gravitee-Api-Key";
        string apiKey = "fe9af2ff-a0e4-4d8f-a4e9-a4dcda81d483";

        FSApiClient fSApiClient = new FSApiClient(apikeyName: apiKeyName, apikey: apiKey);

        if (type == typeof(AndEvenMoreTests))
        {
            return Task.FromResult<object>(new AndEvenMoreTests(new DataClass()));
        }

        throw new NotImplementedException();
    }
}
