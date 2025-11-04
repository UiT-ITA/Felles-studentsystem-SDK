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
            hasMore = result.Data.publiseringsklareStudieprogram.pageInfo.HasNextPage ?? false;
            after = result.Data.publiseringsklareStudieprogram.pageInfo.EndCursor;
            // Extract data
            result.Data.publiseringsklareStudieprogram.nodes.ToList().ForEach(sp => { allStudieprogrammer.Add(sp); });
        }

        return (allStudieprogrammer, null);
    }
    public static async Task<(List<Studieprogram>, Exception?)> GetAllStudieprogrammerRaw(FSApiClient client, CancellationToken cancellationToken = default)
    {
        List<Studieprogram> allStudieprogrammer = new List<Studieprogram>();

        // Iterate pages
        string? after = null;
        bool hasMore = true;

        while (hasMore)
        {
            var qBuilder = RawQueryBuilder.Build(
                QueryPubliseringsklareStudieprogram,
                eierInstitusjonsnummer: "186",
                terminBetegnelse: "HØST",
                aarstall: 2024,
                first: 10,
                after: after
                );

            // Run it
            var result = await client.QueryStudieprogram(qBuilder, cancellationToken);

            // Evaluate
            if (result is null) return (allStudieprogrammer, new Exception("Result from QueryStudieprogram is null"));
            if (FSApiClient.ValidateResult(result) != null) return (allStudieprogrammer, FSApiClient.ValidateResult(result));

            // Next page
            hasMore = result.Data.publiseringsklareStudieprogram.pageInfo.HasNextPage ?? false;
            after = result.Data.publiseringsklareStudieprogram.pageInfo.EndCursor;

            // Extract data
            result.Data.publiseringsklareStudieprogram.nodes.ToList().ForEach(sp => { allStudieprogrammer.Add(sp); });

        }

        return (allStudieprogrammer, null);
    }


    private static string QueryPubliseringsklareStudieprogram = """
        query publiseringAvStudieprograminformasjon {
        publiseringsklareStudieprogram(
            parameter_filtersection
                ) {
            pageInfo {
              endCursor
              hasNextPage
            }
            nodes {
              studieprogram {
                id
                kode
                navnAlleSprak {nb}
                vekting {
                  verdi
                  vektingstype {
                    navnAlleSprak {nb}
                  }
                }
                organisasjonsenhet {
                  studieansvarlig {
                    fakultet {
                      fakultetsnummer
                      navn { nb }
                    }
                  }
                }
                campuser {
                  nodes
                  {
                    campus {
                      kode
                      navnAlleSprak {nb}
                    }
                  }
                }
                sprak {
                  sprak {
                    navn {no}
                    iso6392Kode
                  }
                }
                studieniva {
                  kode
                  navnAlleSprak {nb}
                }
                prosentHeltid
              }
              beskrivelsesavsnitt {
                tekstkategori {
                  kode
                  navnAlleSprak {nb}
                }
                periode {
                  fraTermin {
                    arstall
                    betegnelse { kode }
                  }
                }

                sprak {
                  iso6391Kode
                }
                innhold
              }
            }
          }
        }
        """;

}



public class QryResultStudieprogramHead : GraphQlResponse<QueryData_publiseringsklareStudieprogram> { }

public class QueryData_publiseringsklareStudieprogram
{
    [JsonPropertyName("publiseringsklareStudieprogram")]
    public QryResultpubliseringsklareStudieprogram publiseringsklareStudieprogram { get; set; } = null!;
}

public class QryResultpubliseringsklareStudieprogram
{
    [JsonPropertyName("totalCount")]
    public int totalCount { get; set; } = 0;
    [JsonPropertyName("pageInfo")]
    public PageInfo pageInfo { get; set; } = null!;

    [JsonPropertyName("nodes")]
    public IEnumerable<Studieprogram> nodes { get; set; } = null!;
}