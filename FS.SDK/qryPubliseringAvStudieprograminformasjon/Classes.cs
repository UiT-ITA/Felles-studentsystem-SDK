using FS.SDK.GraphQL.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FS.SDK.qryPubliseringAvStudieprograminformasjon;



public class QryResultStudieprogramHead : GraphQlResponse<QueryData_publiseringsklareStudieprogram> { }

public class QueryData_publiseringsklareStudieprogram
{
    [JsonPropertyName("publiseringsklareStudieprogram")]
    public QryResultpubliseringsklareStudieprogram PubliseringsklareStudieprogram { get; set; } = null!;
}

public class QryResultpubliseringsklareStudieprogram
{
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; } = 0;
    [JsonPropertyName("pageInfo")]
    public PageInfo PageInfo { get; set; } = null!;

    [JsonPropertyName("nodes")]
    public IEnumerable<Studieprogram> Nodes { get; set; } = null!;
}
