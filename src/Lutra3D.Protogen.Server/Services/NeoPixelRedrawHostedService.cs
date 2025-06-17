using System.Drawing;

namespace Lutra3D.Protogen.Server.Services;

public class NeoPixelRedrawHostedService(ProtogenManager protogenManager) : BackgroundService()
{
    protected sealed override async Task ExecuteAsync(CancellationToken ct)
    {
        var neopixel = new ws281x.Net.Neopixel(ledCount: 256, pin: 25);

        neopixel.Begin();

        var frame = -1;
        while (!ct.IsCancellationRequested)
        {
            var image = await protogenManager.GetSidesPixelAsync(ct);

            frame = frame++;
            frame %= image.Height;

            for (var pixelIndex = 0; pixelIndex < neopixel.GetNumberOfPixels(); pixelIndex++)
            {
                var pixel = image.Pixels.ElementAtOrDefault(pixelIndex + frame * image.Width);

                neopixel.SetPixelColor(pixelIndex, Color.FromArgb(pixel.R, pixel.G, pixel.B));
            }

            neopixel.Show();
            await Task.Delay(200, ct);
        }

        neopixel.Dispose();
    }
}
