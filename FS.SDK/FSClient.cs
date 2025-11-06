using FS.SDK.GraphQL.Model;
using FS.SDK.qryPubliseringAvStudieprograminformasjon;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FS.SDK;

public class FSClient(
    FSApiClient fsApiClient, 
    ILogger<FSClient> logger, 
    CancellationToken cancellationToken = default)
{
    private const string eierInstitusjonsnummer = "186";
    private const int pageSize = 50;
    private const int delay = 10;   // milliseconds

    public async Task<List<Studieprogram>> GetAll_Studieprogrammer(int year, string terminbetegnelse)
    {
        (List<Studieprogram> Studieprogrammer, Exception? exc) = await StudieprogramInformasjon.GetAllStudieprogrammerRaw(fsApiClient, eierInstitusjonsnummer, year, terminbetegnelse, pageSize, delay, cancellationToken);
        if (exc is not null)
        {
            logger.LogError(exc, "Error fetching Studieprogrammer for year {Year} and terminbetegnelse {Terminbetegnelse}", year, terminbetegnelse);
            throw exc;
        }
             
        return Studieprogrammer;
    }
}
