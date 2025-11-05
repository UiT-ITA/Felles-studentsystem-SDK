using FS.SDK.GraphQL.Model;
using GraphQlClientGenerator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FS.SDK.qryPubliseringAvStudieprograminformasjon;

internal static class StudieprogramInformasjon
{
    internal static async Task<(List<Studieprogram>, Exception?)> GetAllStudieprogrammerRaw(FSApiClient client, CancellationToken cancellationToken = default)
    {
        List<Studieprogram> allStudieprogrammer = [];

        // Iterate pages
        string? after = null;
        bool hasMore = true;

        while (hasMore)
        {
            string qryParams = string.Format(queryStudieprogramParam, "186", "HØST", 2025, 10, (after is null? "null": "\""+after+"\""));
            string qry = "query publiseringAvStudieprograminformasjon { " + qryParams + queryStudieprogram + "}";

            // Run it
            var result = await client.QueryStudieprogram(qry, cancellationToken);

            // Evaluate
            if (result is null) return (allStudieprogrammer, new Exception("Result from QueryStudieprogram is null"));
            if (FSApiClient.ValidateResult(result) != null) return (allStudieprogrammer, FSApiClient.ValidateResult(result));

            // Next page
            hasMore = result.Data.PubliseringsklareStudieprogram.PageInfo.HasNextPage ?? false;
            after = result.Data.PubliseringsklareStudieprogram.PageInfo.EndCursor;

            // Extract data
            result.Data.PubliseringsklareStudieprogram.Nodes.ToList().ForEach(sp => { allStudieprogrammer.Add(sp); });
        }

        return (allStudieprogrammer, null);
    }



    private static readonly string queryStudieprogramParam = """
        publiseringsklareStudieprogram(
            filter: {{ 
                    eierInstitusjonsnummer: "{0}"
                    terminer: 
                        {{ 
                            terminbetegnelse: "{1}"
                            arstall: {2} 
                        }} 
                    }}
            first: {3}
            after: {4}
            )
        """;

    private static readonly string queryStudieprogram = """
        {
          pageInfo {
            endCursor
            hasNextPage
          }
          nodes {
            studieprogram {
              id
              kode
              navnAlleSprak {
                nb
              }
              vekting {
                verdi
                vektingstype {
                  navnAlleSprak {
                    nb
                  }
                }
              }
              organisasjonsenhet {
                studieansvarlig {
                  fakultet {
                    fakultetsnummer
                    navn {
                      nb
                    }
                  }
                }
              }

              campuser {
                nodes { 
                  campus {
                    kode
                    navnAlleSprak {nb}
                  }
                }
              }
              sprak {
                sprak {
                  navn {
                    no
                  }
                  iso6392Kode
                }
              }
              studieniva {
                kode
                navnAlleSprak {
                  nb
                }
              }
              prosentHeltid
            }

            beskrivelsesavsnitt 
            {
                tekstkategori {
                kode
                navnAlleSprak {nb}
                }
                periode {
                fraTermin {
                    arstall
                    betegnelse {
                    kode
                    }
                }
                }

                sprak {
                iso6391Kode
                }
                innhold
            }            
          }
        }
        """;


}

