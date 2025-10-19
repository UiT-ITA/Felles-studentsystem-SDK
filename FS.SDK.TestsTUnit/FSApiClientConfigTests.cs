using System;
using System.Collections.Generic;
using System.Text;

namespace FS.SDK.TestsTUnit;

[MicrosoftDependencyInjectionDataSource]
public class FSApiClientConfigTests(FSApiClientConfig config)
{
    [Test]
    public async Task TestSettingsIsNotNull()
    {
        await Assert.That(config).IsNotNull();
        await Assert.That(config.ApiKey).IsNotNullOrEmpty();
        await Assert.That(config.ApiKeyName).IsNotNullOrEmpty();
        await Assert.That(config.BaseUrl).IsNotNullOrEmpty();
        await Assert.That(config.Timeout).IsNotNull().And.IsGreaterThan(TimeSpan.Zero);
        await Assert.That(config.UserAgent).IsNotNull();
        await Assert.That(config.MessageHandler).IsNotNull();
    }

    [Test]
    public async Task BaseUrlSet()
    {
        await Assert.That(config.BaseUrl).IsEqualTo("https://gw-uit.intark.uh-it.no/fs-graphql/");
    }

    [Test]
    public async Task ApiKeyNameSet()
    {
        await Assert.That(config.ApiKeyName).IsEqualTo("X-Gravitee-Api-Key");
    }
    [Test]
    public async Task ApiKeySet()
    {
        await Assert.That(config.ApiKey).StartsWith("fe").And.EndsWith("83");
    }



    [Test]
    public async Task TimeoutSet()
    {
        await Assert.That(config.Timeout).IsEqualTo(TimeSpan.FromSeconds(30));
    }
    [Test]
    public async Task UserAgentSet()
    {
        await Assert.That(config.UserAgent.ToString()).IsEqualTo("FS.SDK.NET/0.0.1-beta");
    }
    [Test]
    public async Task MessageHandlerSet()
    {
        await Assert.That(config.MessageHandler).IsTypeOf<System.Net.Http.HttpMessageHandler>();
    }
}

