using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace Lutra3D.Protogen.Server.Api;

public class EmotionChangeEndpoint(ILogger<EmotionChangeEndpoint> logger, ProtogenManager protogenManager) : EndpointBaseAsync.WithRequest<string>.WithoutResult
{

    [HttpPut("api/emotion-change")]
    public override async Task HandleAsync([FromBody] string emotionName, CancellationToken cancellationToken = default)
    {
        await protogenManager.ChangeEmotionAsync(emotionName, cancellationToken);
        logger.LogInformation($"Emotion changed to {emotionName}");
    }
}

