using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;

namespace Lutra3D.Protogen.Server;

public record AnimationFrame(Memory<Rgb24> Pixels, int Delay);
public record PixelImage(int Width, int Height, Rgb24[] Pixels);

public class ProtogenManager : IDisposable
{
    private static readonly SemaphoreSlim ConcurencySemaphore = new(1, 1);

    public Image<Rgb24>? CurrentVisorImage { get; private set; }
    public Image<Rgb24>? CurrentSidesImage { get; private set; }

    public double FanSpeedFraction { get; private set; } = 0.5;

    public async Task<double> GetFanSpeedFractionAsync(CancellationToken cancellationToken) 
    {
        await ConcurencySemaphore.WaitAsync(cancellationToken);
        try
        {
            return FanSpeedFraction;
        }
        finally
        {
            ConcurencySemaphore.Release();
        }
    }

    public async Task SetFanSpeedFractionAsync(double fraction, CancellationToken cancellationToken)
    {
        await ConcurencySemaphore.WaitAsync(cancellationToken);
        try
        {
            FanSpeedFraction = fraction;
        }
        finally
        {
            ConcurencySemaphore.Release();
        }
    }

    public async Task ChangeEmotionAsync(string emotion, CancellationToken cancellationToken)
    {
        await ConcurencySemaphore.WaitAsync(cancellationToken);
        try
        {
            CurrentVisorImage = await LoadImageAsync($"{emotion}.gif");
            CurrentSidesImage = await LoadImageAsync($"{emotion}.bmp");
        }
        finally
        {
            ConcurencySemaphore.Release();
        }
    }

    public async Task<PixelImage> GetSidesPixelAsync(CancellationToken cancellationToken)
    {
        if (CurrentSidesImage is null) { return new PixelImage(0, 0, []); }

        await ConcurencySemaphore.WaitAsync(cancellationToken);
        try
        {
            var pixels = new Span<Rgb24>();
            CurrentSidesImage.CopyPixelDataTo(pixels);
            return new PixelImage(CurrentSidesImage.Width, CurrentSidesImage.Height, [.. pixels]);
        }
        finally
        {
            ConcurencySemaphore.Release();
        }
    }

    public async Task<AnimationFrame[]> GetVisorFramesAsync(CancellationToken cancellationToken)
    {
        if(CurrentVisorImage is null) { return []; }

        await ConcurencySemaphore.WaitAsync(cancellationToken);
        try
        {
            return CurrentVisorImage.Frames
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
    
    private async Task<Image<Rgb24>> LoadImageAsync(string fileName)
    {
        var gifBytes = await LoadResourceAsync(fileName);
        return Image.Load<Rgb24>(gifBytes);
    }


    private async Task<byte[]> LoadResourceAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Lutra3D.Protogen.Server.Emotions.{fileName}";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Could not load emotion {fileName}");
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    public void Dispose()
    {
        CurrentVisorImage?.Dispose();
    }
}

