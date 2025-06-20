using Microsoft.Extensions.Hosting;
using System.Drawing;
using ws281x.Net;

namespace Lutra3D.Protogen.Server.Services;

public class NeoPixelRedrawHostedService(ProtogenManager protogenManager, Neopixel neopixel) : BackgroundService()
{
    protected async override Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield(); //Give App chance to init

        while (!ct.IsCancellationRequested)
        {
            // Set color of all LEDs to red
            var pixel = 0;
            neopixel.SetPixelColor(pixel++, System.Drawing.Color.Red);
            neopixel.Show();
            pixel %= neopixel.GetNumberOfPixels();
            await Task.Delay(200);
        }
    }
}
