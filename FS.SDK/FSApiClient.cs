using FS.SDK.GraphQL.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace FS.SDK;

public class FSApiClient : IDisposable
{
    // HttpClient Setup
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _defaultBaseUrl = "https://api.fellesstudentsystem.no/graphql";
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
    private static readonly ProductInfoHeaderValue FSSdkUserAgent = new("FS.SDK.NET", "0.0.1-beta");
    internal static HttpHeaderValueCollection<ProductInfoHeaderValue>? UserAgent { get; private set; }

    // Serializer utility setup
    private static readonly JsonSerializer Serializer = JsonSerializer.Create(JsonSerializerSettings);

    internal static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Converters = { new StringEnumConverter() },
        DateParseHandling = DateParseHandling.DateTimeOffset
    };

    // Constructor
    public FSApiClient(string apikeyName, string apikey, ProductInfoHeaderValue? myUserAgent = null, HttpMessageHandler? messageHandler = null, TimeSpan? timeout = null, string? baseUrl = null)
    {
        _baseUrl = baseUrl ?? _defaultBaseUrl;

        //if (String.IsNullOrWhiteSpace(accessToken))
        //    throw new ArgumentException("access token required", nameof(accessToken));

        if (String.IsNullOrWhiteSpace(apikeyName))
            throw new ArgumentException("apikeyName required", nameof(apikeyName));

        if (String.IsNullOrWhiteSpace(apikey))
            throw new ArgumentException("apikey required", nameof(apikey));


        messageHandler ??= new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };

        _httpClient = new HttpClient(messageHandler)
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = timeout ?? DefaultTimeout,
            // DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", accessToken), AcceptEncoding = { new StringWithQualityHeaderValue("gzip") } }
            DefaultRequestHeaders = { AcceptEncoding = { new StringWithQualityHeaderValue("gzip") } }
        };

        // Add Gravitee-specific DefaultRequestHeaders
        _httpClient.DefaultRequestHeaders.Add(apikeyName, apikey);

        UserAgent = _httpClient.DefaultRequestHeaders.UserAgent;
        if (myUserAgent is not null)
            UserAgent.Add(myUserAgent);
        else
            UserAgent.Add(FSSdkUserAgent);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private static void ValidateResult(FsSdkApiQueryResponse response)
    {
        if (response.Errors is not null && response.Errors.Any())
            throw new FsSdkApiException(
                $"Query execution failed:{Environment.NewLine}{String.Join(Environment.NewLine, response.Errors.Select(e => $"{e.Message} (locations: {String.Join(";", e.Locations.Select(l => $"line: {l.Line}, column: {l.Column}"))})"))}"
            );
    }


    /// <summary>
    /// Executes raw GraphQL query.
    /// </summary>
    /// <param name="query">query text</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="FsSdkApiHttpException"></exception>
    /// <returns></returns>
    public Task<FsSdkApiQueryResponse?> Query(string query, CancellationToken cancellationToken = default) => Request<FsSdkApiQueryResponse>(query, cancellationToken);

    /// <summary>
    /// Executes raw GraphQL mutation.
    /// </summary>
    /// <param name="mutation">query text</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="FsSdkApiHttpException"></exception>
    /// <returns></returns>
    //public Task<FsSdkApiMutationResponse> Mutation(string mutation, CancellationToken cancellationToken = default) => Request<FsSdkApiMutationResponse>(mutation, cancellationToken);


    private async Task<TResult?> Request<TResult>(string query, CancellationToken cancellationToken)
    {
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
            throw await FsSdkApiHttpException.Create(new Uri(_baseUrl), HttpMethod.Post, response, DateTimeOffset.Now - requestStart).ConfigureAwait(false);

        using var stream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(streamReader);
        return Serializer.Deserialize<TResult>(jsonReader);
    }


    private static HttpContent JsonContent(object data) => new StringContent(JsonConvert.SerializeObject(data, settings: JsonSerializerSettings), Encoding.UTF8, "application/json");



    /// <summary>
    /// Gets all studieprogrammer.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="FsSdkApiHttpException"></exception>
    /// <returns></returns>
    public async Task<FsSdkApiQueryResponse?> GetAllStudieprogrammer(CancellationToken cancellationToken = default)
    {
        var result = await Query(new StudieprogramQueryBuilder()
            .WithAllScalarFields()
            .Build(), cancellationToken);

        if (result is null) return null;
        ValidateResult(result);
        return result;
    }
}


public class FsSdkApiQueryResponse : GraphQlResponse<QueryData> { }

public class QueryData
{
    //public Viewer Viewer { get; set; }
}

// TODO: Let's wait with mutations until we actually need them
// public class FsSdkApiMutationResponse : GraphQlResponse<FsSdkMutation> { }


