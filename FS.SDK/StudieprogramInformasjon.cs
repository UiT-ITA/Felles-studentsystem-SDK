using FS.SDK.GraphQL.Model;
using GraphQlClientGenerator;
using System.Text.Json.Serialization;

namespace FS.SDK;

public static class StudieprogramInformasjon
{

    //public async Task Query()
    //{
    //    var qryBuilder = new QueryQueryBuilder("PubliseringsklareStudieprogramInformasjon")
    //        .WithPubliseringsklareStudieprogram(
    //            queryPubliseringsklartStudieprogramForTerminConnectionQueryBuilder: new QueryPubliseringsklartStudieprogramForTerminConnectionQueryBuilder(),
    //            filter: new PubliseringsklareStudieprogramFilter
    //            {
    //                EierInstitusjonsnummer = "186",
    //                KanPubliseres = true
    //            })
    //        .WithAllFields();

    //    var query = qryBuilder.Build(Formatting.Indented);

    //    var result = await client.Query(query);
    //}

    ///<summary>
    ///Gets all studieprogrammer.
    ///</summary>
    ///<param name = "cancellationToken" ></ param >
    ///< exception cref="FsSdkApiHttpException"></exception>
    ///<returns></returns>
    public static async Task GetAllStudieprogrammer<T>(FSApiClient client, CancellationToken cancellationToken = default)
    {
        var filter = new StudieprogramV2FilterInput();
        filter.EierOrganisasjonskode = "186";

        int pageSize = 10;
        string? after = null;
        bool hasMore = true;

        int pageCount = 0;

        while (hasMore)
        {

            var qBuilder = new QueryQueryBuilder()
                .WithStudieprogramV2(
                    new QueryStudieprogramV2ConnectionQueryBuilder()
                        .WithAllScalarFields()
                        .WithPageInfo(new PageInfoQueryBuilder()
                            .WithAllScalarFields()
                        )
                        .WithNodes(new StudieprogramQueryBuilder()
                            .WithAllScalarFields()
                            .WithCampuser(new StudieprogramCampuserConnectionQueryBuilder()
                                .WithAllScalarFields()
                                )
                        ),
                    filter: filter,
                    first: pageSize,
                    after: after)
                .Build(formatting: FS.SDK.GraphQL.Model.Formatting.Indented);

            var result = await client.QueryStudieprogram(qBuilder, cancellationToken);

            // TODO: Vurder å flytte dette til en samlende funksjon
            if (result is null) throw new Exception("Result from QueryStudieprogram is null");
            FSApiClient.ValidateResult(result);

            Console.WriteLine($"Page {pageCount + 1}: Retrieved {result.Data.studieprogramV2.nodes.Count()} studieprogrammer.");
            hasMore = result.Data.studieprogramV2.pageInfo.HasNextPage ?? false;

        }


        //return result;

    }



}


public class QryResultStudieprogramHead : GraphQlResponse<QueryDataStudieprogramV2> { }

public class QueryDataStudieprogramV2
{
    [JsonPropertyName("studieprogramV2")]
    public QryResultStudieprogramV2 studieprogramV2 { get; set; } = null!;
}

public class QryResultStudieprogramV2
{
    [JsonPropertyName("totalCount")]
    public int totalCount { get; set; } = 0;
    [JsonPropertyName("pageInfo")]
    public PageInfo pageInfo { get; set; } = null!;

    [JsonPropertyName("nodes")]
    public IEnumerable<Studieprogram> nodes { get; set; } = null!;
}