using RPiRgbLEDMatrix;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = RPiRgbLEDMatrix.Color;
using Image = SixLabors.ImageSharp.Image;
using System.Reflection;


var gifBytes = await LoadGifAsync("neutral");
using var image = Image.Load<Rgb24>(gifBytes);

var options = new RGBLedMatrixOptions
{
    ChainLength=2,
    Parallel = 1,
    Cols = 64,
    HardwareMapping = "adafruit-hat"
};

using var matrix = new RGBLedMatrix(options);
var canvas = matrix.CreateOffscreenCanvas();

image.Mutate(o => o.Resize(canvas.Width, canvas.Height));

var running = true;
Console.CancelKeyPress += (s, e) =>
{
    running = false;
    e.Cancel = true; // don't terminate, we need to dispose
};

var frame = -1;
// preprocess frames to get delays and pixel buffers
var frames = image.Frames
    .Select(f => (
        Pixels: f.DangerousTryGetSinglePixelMemory(out var memory) ? memory : throw new("Could not get pixel buffer"),
        Delay: f.Metadata.GetGifMetadata().FrameDelay * 10
    )).ToArray();

// run until user presses Ctrl+C
while (running)
{
    frame = (frame + 1) % frames.Length;

    var data = MemoryMarshal.Cast<Rgb24, Color>(frames[frame].Pixels.Span);
    canvas.SetPixels(0, 0, canvas.Width, canvas.Height, data);

    matrix.SwapOnVsync(canvas);
    Thread.Sleep(frames[frame].Delay);
}


async Task<byte[]> LoadGifAsync(string emotion, CancellationToken cancellationToken = default)
{
    var assembly = Assembly.GetExecutingAssembly();
    var resourceName = $"Lutra3D.Protogen.Control.Emotions.{emotion}.gif";

    using var stream = assembly.GetManifestResourceStream(resourceName)
        ?? throw new InvalidOperationException($"Could not load emotion {emotion}");
    using var memoryStream = new MemoryStream();
    await stream.CopyToAsync(memoryStream, cancellationToken);
    return memoryStream.ToArray();
}