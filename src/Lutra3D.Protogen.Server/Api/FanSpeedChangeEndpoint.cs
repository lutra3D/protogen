using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lutra3D.Protogen.Server.Api;

public class FanSpeedChangeEndpoint(ILogger<EmotionChangeEndpoint> logger, ProtogenManager protogenManager) : EndpointBaseAsync.WithRequest<double>.WithoutResult
{

    [HttpPut("api/fan-change")]
    public override async Task HandleAsync([FromBody] double fraction, CancellationToken cancellationToken = default)
    {
        await protogenManager.SetFanSpeedFractionAsync(fraction, cancellationToken);
        logger.LogInformation($"Fan speed changed to {fraction}");
    }
}

