using rpi_ws281x;
using System.Drawing;

var settings = Settings.CreateDefaultSettings();
settings.Channels[0] = new Channel(24, 18, 128, false, StripType.WS2812_STRIP);

using (var rpi = new WS281x(settings))
{
    //Set the color of the first LED of channel 0 to blue
    rpi.SetLEDColor(0, 0, Color.Blue);
    //Set the color of the second LED of channel 0 to red
    rpi.SetLEDColor(0, 1, Color.Red);

    rpi.Render();
}