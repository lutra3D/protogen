using RPiRgbLEDMatrix;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace Lutra3D.Protogen.Server.Services;

public class LedMatrixRedrawHostedService(RGBLedMatrix matrix, ProtogenManager protogenManager) : BackgroundService()
{
    protected sealed override async Task ExecuteAsync(CancellationToken ct)
    {
        await protogenManager.ChangeEmotionAsync("neutral", ct);

        var canvas = matrix.CreateOffscreenCanvas();

        var frame = -1;

        var frames = await protogenManager.GetFramesAsync(ct);

        while (!ct.IsCancellationRequested)
        {
            frame = (frame + 1) % frames.Length;

            var data = MemoryMarshal.Cast<Rgb24, Color>(frames[frame].Pixels.Span);
            canvas.SetPixels(0, 0, canvas.Width, canvas.Height, data);

            matrix.SwapOnVsync(canvas);
            await Task.Delay(frames[frame].Delay);
        }
    }
}