using Microsoft.Extensions.Hosting;
using rpi_ws281x;
using System.Drawing;

namespace Lutra3D.Protogen.Server.Services;

public class NeoPixelRedrawHostedService(WS281x neopixels, ProtogenManager protogenManager) : BackgroundService()
{
    protected async sealed override Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield();

        var pixel = 0;
        while (!ct.IsCancellationRequested)
        {
            neopixels.SetLEDColor(0, pixel++, Color.Blue);
            neopixels.SetLEDColor(0, pixel++, Color.Red);
            neopixels.Render();
            pixel %= 25;
        }

    }
}
