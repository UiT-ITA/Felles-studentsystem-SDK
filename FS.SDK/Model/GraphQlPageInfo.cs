using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FS.SDK.Model;

public class GraphQlPageInfo
{
    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage { get; set; } = false;
    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage { get; set; } = false;
    [JsonPropertyName("startCursor")]
    public string StartCursor { get; set; } = string.Empty;
    [JsonPropertyName("endCursor")]
    public string EndCursor { get; set; } = string.Empty;
}
