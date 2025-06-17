namespace Lutra3D.Protogen.Server.Services;

public class NeoPixelRedrawHostedService(ProtogenManager protogenManager) : BackgroundService()
{
    protected sealed override async Task ExecuteAsync(CancellationToken ct)
    {
        var neopixel = new ws281x.Net.Neopixel(ledCount: 256, pin: 25);

        // You can also choose a custom color order
        neopixel = new ws281x.Net.Neopixel(ledCount: 42, pin: 18, stripType: rpi_ws281x.WS2811_STRIP_RBG);

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
    }
}
