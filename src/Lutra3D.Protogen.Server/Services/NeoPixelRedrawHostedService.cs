using Microsoft.Extensions.Hosting;
using rpi_ws281x;
using System.Drawing;

namespace Lutra3D.Protogen.Server.Services;

public class NeoPixelRedrawHostedService(ProtogenManager protogenManager, WS281x neopixel) : BackgroundService()
{
    public const int LedChannel = 1;
    protected async override Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield(); //Give App chance to init

        var frame = -1;
        while (!ct.IsCancellationRequested)
        {
            var image = await protogenManager.GetSidesPixelAsync(ct);
            frame++;
            frame %= image.Width;

            for (var pixelIndex = 0; pixelIndex < neopixel.Settings.Channels[LedChannel].LEDCount; pixelIndex++)
            {
                var pixel = image.Pixels.ElementAtOrDefault(pixelIndex + frame * image.Width);

                neopixel.SetLEDColor(LedChannel, pixelIndex, Color.FromArgb(pixel.R, pixel.G, pixel.B));
            }

            await Task.Delay(200, ct);
        }
    }
}

