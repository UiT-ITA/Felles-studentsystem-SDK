using System;
using System.Collections.Generic;
using System.Text;

namespace FS.SDK;

internal static class RawQueryBuilder
{
    internal static string Build(string query, string eierInstitusjonsnummer, string terminBetegnelse, int aarstall, int first, string? after = null)
    {
        string qryFilter = """
            filter: { 
                    eierInstitusjonsnummer: "param_eierInstitusjonsnummer", 
                    terminer: 
                        { 
                            terminbetegnelse: "param_terminBetegnelse", 
                            arstall: param_aarstall 
                        } 
                    }
            first: param_first
            after: param_after
            """;

        qryFilter = qryFilter.Replace("param_eierInstitusjonsnummer", eierInstitusjonsnummer);
        qryFilter = qryFilter.Replace("param_terminBetegnelse", terminBetegnelse);
        qryFilter = qryFilter.Replace("param_aarstall", aarstall.ToString());
        qryFilter = qryFilter.Replace("param_first", first.ToString());
        qryFilter = qryFilter.Replace("param_after", after != null ? $"\"{after}\"" : "null");

        return query.Replace("parameter_filtersection", qryFilter);
    }


}
/*
  query publiseringAvStudieprograminformasjon {
  publiseringsklareStudieprogram(
    filter: {eierInstitusjonsnummer: "1234", terminer: {terminbetegnelse: "HØST", arstall: 2023}}
    first: 100
    #after: "legg inn endCursor fra forrige side, dersom den har hasNextPage = true
  ) {
     
    pageInfo {
      endCursor
      hasNextPage
    }
    nodes {
      studieprogram {
        id
        kode
        navnAlleSprak {
          nno
        }
        vekting {
          verdi
          vektingstype {
            navnAlleSprak {
              nob
            }
          }
        }
        organisasjonsenhet {
          studieansvarlig {
            fakultet {
              fakultetsnummer
              navn {
                nob
              }
            }
          }
        }
        campuser {
          campus {
            kode
            navn {
              nob
            }
          }
        }
        sprak {
          sprak {
            navn {
              nor
            }
            iso6392Kode
          }
        }
        studieniva {
          kode
          navnAlleSprak {
            nob
          }
        }
        prosentHeltid
      }
      beskrivelser {
        tekstkategori {
          kode
          navn {
            nob
          }
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
}
*/