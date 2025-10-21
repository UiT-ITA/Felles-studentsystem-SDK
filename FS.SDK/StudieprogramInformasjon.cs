using FS.SDK.GraphQL.Model;
using GraphQlClientGenerator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    public static async Task<(List<Studieprogram>, Exception?)> GetAllStudieprogrammer(FSApiClient client, CancellationToken cancellationToken = default)
    {
        // Prepare
        //var filter = new StudieprogramV2FilterInput() 
        //{ 
        //    EierOrganisasjonskode = "186"
        //};
        var filter = new PubliseringsklareStudieprogramFilter
        {
            EierInstitusjonsnummer = "186",
            KanPubliseres = true, 
            //Terminer = new PubliseringsklartStudieprogramTerminInput[]
            //{
            //    new PubliseringsklartStudieprogramTerminInput
            //    {
            //        Arstall = 2024,
            //        Terminbetegnelse = "HOST"
            //    }
            //}
        };
        int pageSize = 10;

        List<Studieprogram> allStudieprogrammer = new List<Studieprogram>();

        // Iterate pages
        string? after = null;
        bool hasMore = true;
        while (hasMore)
        {
            // Query
            // https://fs.sikt.no/apier/graphql/Uttrekk%20av%20studieinformasjon/

            var qBuilder = new QueryQueryBuilder()
                .WithPubliseringsklareStudieprogram(
                    new QueryPubliseringsklartStudieprogramForTerminConnectionQueryBuilder()
                        .WithAllScalarFields()
                        .WithPageInfo(new PageInfoQueryBuilder().WithAllScalarFields())
                        //.WithNodes(


                        ,
                    filter: filter,
                    first: pageSize,
                    after: after
                    )
                .Build(formatting: FS.SDK.GraphQL.Model.Formatting.Indented);

            // Run it
            var result = await client.QueryStudieprogram(qBuilder, cancellationToken);

            // Evaluate
            if (result is null) return(allStudieprogrammer, new Exception("Result from QueryStudieprogram is null"));
            if (FSApiClient.ValidateResult(result) != null) return (allStudieprogrammer, FSApiClient.ValidateResult(result));

            // Next page
            hasMore = result.Data.studieprogramV2.pageInfo.HasNextPage ?? false;
            after = result.Data.studieprogramV2.pageInfo.EndCursor;

            // Extract data
            result.Data.studieprogramV2.nodes.ToList().ForEach(sp => { allStudieprogrammer.Add(sp); });
        }

        return (allStudieprogrammer, null);
    }
}


public class QryResultStudieprogramHead : GraphQlResponse<QueryData_publiseringsklareStudieprogram> { }

public class QueryData_publiseringsklareStudieprogram
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