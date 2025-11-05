using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace FS.SDK;


public class FSApiClientConfig
{
    private string _apiKeyName = null!;
    private string _apiKey = null!;
    private string _baseUrl = "https://api.fellesstudentsystem.no/graphql";

    public string ApiKeyName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_apiKeyName))
                throw new ArgumentException("apikeyName required", nameof(ApiKeyName));
            return _apiKeyName;
        }
        set { _apiKeyName = value; }
    }
    public string ApiKey
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new ArgumentException("apikey required", nameof(ApiKey));
            return _apiKey;
        }
        set { _apiKey = value; }
    }
    public string BaseUrl
    {
        get { return _baseUrl; }
        set { _baseUrl = value; }
    }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    public ProductInfoHeaderValue UserAgent { get; set; } = new("FS.SDK.NET", "0.0.2-alpha");

    public HttpMessageHandler? MessageHandler { get; set; } = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
}
