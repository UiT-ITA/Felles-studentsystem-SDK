using System;
using System.Collections.Generic;
using System.Text;

namespace FS.SDK.TestsTUnit;

[MicrosoftDependencyInjectionDataSource]
public class FSApiClientConfigTests(FSApiClientConfig config)
{
    [Test]
    public async Task Test()
    {
        await Assert.That(config).IsNotNull();
        await Assert.That(config.ApiKey).IsNotNullOrEmpty();
        await Assert.That(config.ApiKeyName).IsNotNullOrEmpty();
        await Assert.That(config.BaseUrl).IsNotNullOrEmpty();
        await Assert.That(config.Timeout).IsNotNull().And.IsGreaterThan(TimeSpan.Zero);
        await Assert.That(config.UserAgent).IsNotNull();
        await Assert.That(config.MessageHandler).IsNotNull();

    }
}

