using FS.SDK.GraphQL.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace FS.SDK;


public class FSApiClient : IDisposable
{
    // HttpClient Setup
    private readonly HttpClient _httpClient;
    private readonly FSApiClientConfig _config = null!;
    internal static HttpHeaderValueCollection<ProductInfoHeaderValue>? UserAgent { get; private set; }

    // Serializer utility setup
    private static JsonSerializerOptions jsonSerializerOptions => new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    // Newtonsoft
    //private static readonly JsonSerializer Serializer = JsonSerializer.Create(JsonSerializerSettings);
    //internal static readonly JsonSerializerSettings JsonSerializerSettings = new()
    //{
    //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    //    Converters = { new StringEnumConverter() },
    //    DateParseHandling = DateParseHandling.DateTimeOffset
    //};

    // Constructor
    public FSApiClient(FSApiClientConfig config)
    {
        //config.MessageHandler ??= new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
        _config = config;
        _httpClient = new HttpClient(config.MessageHandler)
        {
            BaseAddress = new Uri(config.BaseUrl),
            Timeout = config.Timeout,
            DefaultRequestHeaders = { AcceptEncoding = { new StringWithQualityHeaderValue("gzip") } }
        };

        // Add Gravitee-specific DefaultRequestHeaders
        _httpClient.DefaultRequestHeaders.Add(config.ApiKeyName, config.ApiKey);
        _httpClient.DefaultRequestHeaders.UserAgent.Add(config.UserAgent);
    }

    /// <summary>
    /// Executes raw GraphQL query.
    /// </summary>
    /// <param name="query">query text</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="FsSdkApiHttpException"></exception>
    /// <returns></returns>
    public async Task<QryResultStudieprogramHead?> QueryStudieprogram(string query, CancellationToken cancellationToken = default) => await Request<QryResultStudieprogramHead?>(query, cancellationToken);
    
    private async Task<TResult?> Request<TResult>(string query, CancellationToken cancellationToken)
    {
#if DEBUG
        Console.WriteLine(query);
#endif

        var requestStart = DateTimeOffset.UtcNow;
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsync(String.Empty, JsonContent(new { query }), cancellationToken);
        }
        catch (Exception exception)
        {
            throw new FsSdkApiHttpException(_httpClient.BaseAddress ?? new Uri("BaseAddress is null"), HttpMethod.Post, DateTimeOffset.Now - requestStart, exception.Message, exception);
        }

        if (!response.IsSuccessStatusCode)
            throw await FsSdkApiHttpException.Create(new Uri(_config.BaseUrl), HttpMethod.Post, response, DateTimeOffset.Now - requestStart).ConfigureAwait(false);

        using var stream = await response.Content.ReadAsStreamAsync();

#if DEBUG
        using var streamReaderDebug = new StreamReader(stream);        
        string text = streamReaderDebug.ReadToEnd();
        Console.WriteLine(text);
        stream.Position = 0;
#endif

        using var streamReader = new StreamReader(stream);
        //using var jsonReader = new JsonTextReader(streamReader);
        //return Serializer.Deserialize<TResult>(jsonReader);

        TResult? res = await JsonSerializer.DeserializeAsync<TResult>(stream, jsonSerializerOptions, cancellationToken);
        return res;
    }





    public void Dispose()
    {
        _httpClient.Dispose();
    }

    internal static FsSdkApiException? ValidateResult(QryResultStudieprogramHead response)
    {
        if (response.Errors is not null && response.Errors.Any())
            return new FsSdkApiException(
                $"Query execution failed:{Environment.NewLine}{String.Join(Environment.NewLine, response.Errors.Select(e => $"{e.Message} (locations: {String.Join(";", e.Locations.Select(l => $"line: {l.Line}, column: {l.Column}"))})"))}"
            );
        return null;
    }


    /// <summary>
    /// Executes raw GraphQL mutation.
    /// </summary>
    /// <param name="mutation">query text</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="FsSdkApiHttpException"></exception>
    /// <returns></returns>
    //public Task<FsSdkApiMutationResponse> Mutation(string mutation, CancellationToken cancellationToken = default) => Request<FsSdkApiMutationResponse>(mutation, cancellationToken);
    
    private static HttpContent JsonContent(object data) => new StringContent(JsonSerializer.Serialize(data, jsonSerializerOptions), Encoding.UTF8, "application/json");

}


// TODO: Let's wait with mutations until we actually need them
// public class FsSdkApiMutationResponse : GraphQlResponse<FsSdkMutation> { }


