using Microsoft.Extensions.Hosting;
using rpi_ws281x;

namespace Lutra3D.Protogen.Server.Services;

public class NeoPixelRedrawHostedService(ProtogenManager protogenManager, WS281x neopixel) : BackgroundService()
{
    protected async override Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield(); //Give App chance to init

        var pixel = 0;
        while (!ct.IsCancellationRequested)
        {
            // Set color of all LEDs to red
            neopixel.SetLEDColor(0, pixel++, System.Drawing.Color.Red);
            neopixel.Render();
            pixel %= 25;
            await Task.Delay(200);
        }
    }
}

