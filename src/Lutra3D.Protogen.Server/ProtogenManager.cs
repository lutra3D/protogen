using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;

namespace Lutra3D.Protogen.Server;

public record AnimationFrame(Memory<Rgb24> Pixels, int Delay);

public class ProtogenManager : IDisposable
{
    private static readonly SemaphoreSlim ConcurencySemaphore = new(1, 1);

    public Image<Rgb24>? CurrentImage { get; private set; }

    public async Task ChangeEmotionAsync(string emotion, CancellationToken cancellationToken)
    {
        await ConcurencySemaphore.WaitAsync(cancellationToken);
        try
        {
            var gifBytes = await LoadGifAsync(emotion);
            CurrentImage = Image.Load<Rgb24>(gifBytes);
        }
        finally
        {
            ConcurencySemaphore.Release();
        }
    }

    public async Task<AnimationFrame[]> GetFramesAsync(CancellationToken cancellationToken)
    {
        if(CurrentImage is null) { return []; }

        await ConcurencySemaphore.WaitAsync(cancellationToken);
        try
        {
            return CurrentImage.Frames
           .Select(f => new AnimationFrame(
               f.DangerousTryGetSinglePixelMemory(out var memory) ? memory : throw new("Could not get pixel buffer"),
               f.Metadata.GetGifMetadata().FrameDelay * 10
           )).ToArray();
        }
        finally
        {
            ConcurencySemaphore.Release();
        }
    }

    private async Task<byte[]> LoadGifAsync(string emotion, CancellationToken cancellationToken = default)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Lutra3D.Protogen.Control.Emotions.{emotion}.gif";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Could not load emotion {emotion}");
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    public void Dispose()
    {
        CurrentImage?.Dispose();
    }
}

