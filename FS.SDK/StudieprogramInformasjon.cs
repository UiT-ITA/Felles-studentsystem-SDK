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
                eierOrganisasjonskode: "186",
                aarstall: 2025,
                terminBetegnelse: "HØST",
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
          query QueryEmnerV2 
          (
            $eierOrganisasjonskode: String!
            $aarstall: Int!
            $terminbetegnelse: EmneIkkeUtloptITerminTerminbetegnelse!
            $after: String!
            $pagesize : Int!
          )
          {
            emnerV2(
              first: $pagesize
              after : $after

              filter: {
                eierOrganisasjonskode:$eierOrganisasjonskode
                ikkeUtloptITermin:
                {
                    arstall:$aarstall
                    terminbetegnelse:$terminbetegnelse
                }
              }
            ) 




          {
            pageInfo 
            {
              hasPreviousPage
              hasNextPage
              startCursor
              endCursor
            }
            nodes
            {
              id
              kode
              versjonskode
              navnAlleSprak {en,nb}
              navnForkortetAlleSprak {und}
              emnetype      
              tilbysSomFjernundervisning { kode, navn}
              studieniva 
              { 
                id
                kode
                navnAlleSprak {nb, en}
                # eqfnivakode #denne feiler, den er ikke int32 hver gang
                # nkrsyklus {kode, navnAlleSprak {nb, en}, vitnemalsnavnAlleSprak {nb}} # Gir alltid null
                dbhNiva {dbhNivakode, dbhNivaBeskrivelse {und},  dbhNivaNavnAlleSprak {und}}
              }
              # avgifter #Gir alltid null            
              vekting 
              {
                emnevekting 
                {
                  verdi
                  vektingstype 
                  {
                    kode
                    navnAlleSprak {nb, en}
                    erAktiv
                    vektenheterPerSemester
                  }
                }
                vektingsreduksjonsregler
                {          
                  prioritetsnummer
                  regel {
                    kode
                    reduksjon {verdi, vektingstype {kode, erAktiv, vektenheterPerSemester, navnAlleSprak {nb, en}}}
                    periode {
                      fraTermin{betegnelse{kode}, arstall, oppdateringssperrer {undervisningsaktiviteter}}
                      tilTermin{betegnelse{kode}, arstall, oppdateringssperrer {undervisningsaktiviteter}}}}
                   	emne {kode}
                }        
              }
              vurderesIPeriode 
              {
                forsteTermin {betegnelse{kode}, arstall}
                sisteTermin {betegnelse{kode}, arstall}
              }
              undervisesIPeriode
              {
                forsteTermin {betegnelse {kode}, arstall}
                sisteTermin {betegnelse {kode}, arstall}
              }   
              undervisningsoversikt 
              {
                varighet {antall, tidsenhet {kode, navnAlleSprak {nb, en}}}
                undervisningsterminer 
                {
                  id
                  terminnummer 
                  starttermin 
                  gjelderForTermin 
                  arEtterStart
                  # fordypningstimer  # gir alltid null
                }
              }      
              rapporteringsstudieprogram 
              {
                kode, navnAlleSprak {nb, en}, erAktiv
              }
              studieprogramkoblinger 
              {
                studieprogram {kode, navnAlleSprak {nb, en}, erAktiv}
                periode {
                  fraTermin {betegnelse{kode}, arstall}
                  # tilTermin {betegnelse{kode}, arstall} # Gir alltid null
                }
              }
              nusKode      
              studieansvarligOrganisasjonsenhet
              {
                navnAlleSprak {nb, en}
                organisasjon {navnAlleSprak {nb, en}}
                fakultet {navnAlleSprak {nb, en}}
                #institutt {navnAlleSprak {nb}} - Gir alltid null
                bibliotek {navnAlleSprak {und}} 
                erAktiv
                skalEksporteresTilLms
                # Mye mer,....
              }

              undervisningsenheter 
              {
                nodes 
                {
                  id
                   	termin {betegnelse {kode, navnAlleSprak {nb, en}}, arstall}
                  terminnummer
                  emne {kode, versjonskode, navnAlleSprak {nb, en}}
                  #undervisningsaktiviteter
                  #{
                  #  nodes
                  #  {
                  #    kode
                  #    # undervisningsenhet 
                  #    partinummer
                  #    lmsRomkode
                  #    skalEksporteresTilLms
                  #    navnAlleSprak {nb}
                  #  }
                  #}
                  #oppmote 
                  #{
                  #  tidspunkt
                  #  undervisningsaktivitet {kode, partinummer, navnAlleSprak {nb, en}}
                  #  #timeplan
                  #  undervisningsuke {ukenummer, arstall}
                  #  merknad
                  #	harMott
                  #}
                }
              }
              tjenestenummerForLms 
              sprakvalg 
              {
                sprak {id, iso6391Kode, iso6392Kode, navn {no, en}}
              }      
              administrativtAnsvarligOrganisasjonsenhet 
              {
                id, navnAlleSprak {nb, en}
                # Mye mer.... 
              }      
              kanTilbysMedFleksibelFinansiering
              # tilgjengeligHosLanekassenPeriode {datoFra, datoTil}     # Gir alltid null  
              navnAlleSprakHistorikk 
              {
                id
                fraTermin {betegnelse {kode, navnAlleSprak {nb, en}} , arstall}
                tilTermin {betegnelse {kode, navnAlleSprak {nb, en}} , arstall}
                navnAlleSprak {nb,en}
                navnForkortetAlleSprak {und}
              }           
              fagkoblinger 
              {
                viktigsteFag {navnAlleSprak{nb, en}, kode, subjectArea {navn {und}, kode}}
                fag {navnAlleSprak{nb, en}, kode, subjectArea {navn {und} , kode}}
              }      
              tilbysSomEnkeltemne 
           			# samarbeidendeLarested      
           			kreverStudierett       
              # praksistype # Gir alltid null

              url {und}
              #campuser {}      
              #publiseringsklartITerminer {}      
           			antallLovligeVurderingsforsok 		{antallLovligeForsok, gjelderFraTermin {betegnelse {kode, navnAlleSprak {nb, en} }, arstall}}     
              antallLovligeUndervisningsforsok 	{antallLovligeForsok } #gjelderFraTermin {betegnelse {kode, navnAlleSprak {nb} }, arstall}}   # Gir alltid null
                	forkunnskapskrav 
              {
                emne {kode, versjonskode, navnAlleSprak {nb, en}, navnForkortetAlleSprak {und}, emnetype}
                periode {fraTermin {betegnelse {kode, navnAlleSprak {nb, en}}, arstall}}
                pakrevdeEmner {kode, versjonskode, navnAlleSprak {nb, en}, navnForkortetAlleSprak {und}, emnetype}
                # Gir alltid null
                #pakrevdeEmnesamlinger { kode, studieniva {kode, navnAlleSprak {nb}},  vekting {verdi, vektingstype{kode, navnAlleSprak {nb}}}}
                #pakrevdeKravelementer {kode, navnAlleSprak {nb}}
                #pakrevdeVurderingsoppbygningsdeler {kode, navnAlleSprak {nb}}
              }      
              iscedFKode       
              vurderingsoppbygninger 
              {
                vurderingsordning {kode, navnAlleSprak {nb, en}, aktiv}
                erAktiv
                erForhandsvalgt
                periode {fraTermin {betegnelse {kode, navnAlleSprak {nb, en} }, arstall}, tilTermin {betegnelse {kode, navnAlleSprak {nb, en} }, arstall}}
              }      
              studienivaIntervall {studienivaintervallkode }      
              erOppgave 
              # nusklassifikasjon        
              # personroller      
              # vurderingsenheter      
              pameldingsform      
              etteranmeldingsform      
              # vurderingsperiodeinformasjon      
              # anbefaltForkunnskap      
              beskrivelsesavsnitt 
              (
                first:1000
                #filter: {gjelderFraTerminer:{ arstall : 2020, terminbetegnelse:"HØST"}}
              )
              {
                nodes  
                {
                  id
                  tekstkategori {
                    kode
                    rekkefolgenummer
                    navnAlleSprak {nb, en}
                    erEnDefaultTekstkategori
                    kanBrukesTilStudieprogrambeskrivelser
                    kanBrukesTilEmnebeskrivelser
                    kanBrukesTilKursbeskrivelser
                    kanBrukesTilUtvekslingbeskrivelser
                    kanBrukesTilStedbeskrivelser
                    kanPubliseresTilWeb
                    kanPubliseresTilWebapplikasjoner
                  }
                  periode 
                  {
                    fraTermin {betegnelse {kode}, arstall}
                    tilTermin {betegnelse {kode}, arstall}
                  }
                  innhold
                  originalinnhold
                  cdmTag
                  publiseringstag
                  rekkefolgenummer
                  sprak {iso6391Kode, iso6392Kode, navn {no, en}}          
                }    
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