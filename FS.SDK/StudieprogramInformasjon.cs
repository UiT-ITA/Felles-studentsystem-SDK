using GraphQlClientGenerator;
using FS.SDK.GraphQL.Model;

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
    public static async Task<FsSdkApiQueryResponse?> GetAllStudieprogrammer(FSApiClient client, CancellationToken cancellationToken = default)
    {

        var filter = new StudieprogramV2FilterInput();
        filter.EierOrganisasjonskode = "186";

        var qBuilder = new QueryQueryBuilder()
            .WithStudieprogramV2(
                new QueryStudieprogramV2ConnectionQueryBuilder()
                    .WithAllScalarFields()
                    .WithNodes(new StudieprogramQueryBuilder()
                        .WithAllScalarFields()
                    ),
                filter: filter,
                first: 50,
                after: null)
            .Build(formatting: FS.SDK.GraphQL.Model.Formatting.Indented);

        //var query = new StudieprogramQueryBuilder()
        //    .WithAllScalarFields()
        //    .Build();

        var result = await client.Query(qBuilder, cancellationToken);
        
        // TODO: Vurder å flytte dette til en samlende funksjon
        if (result is null) return null;
        FSApiClient.ValidateResult(result);
        return result;
    }

}
