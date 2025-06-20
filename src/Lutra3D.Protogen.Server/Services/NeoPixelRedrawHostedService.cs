using System.Drawing;

namespace Lutra3D.Protogen.Server.Services;

public class NeoPixelRedrawHostedService(ProtogenManager protogenManager) : BackgroundService()
{
    protected override Task ExecuteAsync(CancellationToken ct)
    {
        var neopixel = new ws281x.Net.Neopixel(ledCount: 24, pin: 18);

        // Always initialize the wrapper first
        neopixel.Begin();

        // Set color of all LEDs to red
        for (var i = 0; i < neopixel.GetNumberOfPixels(); i++)
        {
            neopixel.SetPixelColor(i, System.Drawing.Color.Red);
        }

        // Apply changes to the led
        neopixel.Show();

        // Dispose after use
        neopixel.Dispose();

        return Task.CompletedTask;
    }
}
