
using Iot.Device.Ws28xx;
using System;
using System.Device.Spi;
using System.Drawing;

namespace Lutra3D.Protogen.Server.Services;

public class NeoPixelRedrawHostedService(ProtogenManager protogenManager) : BackgroundService()
{
    protected sealed override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield(); //Give App chance to init

        SpiConnectionSettings settings = new(0, 0)
        {
            ClockFrequency = 2_400_000,
            Mode = SpiMode.Mode0,
            DataBitLength = 8
        };
        using SpiDevice spi = SpiDevice.Create(settings);

        var neo = new Ws2812b(spi, 24);

        while (!ct.IsCancellationRequested)
        {
            Rainbow(neo, 24);
            await Task.Delay(200);
        }

        void Rainbow(Ws28xx neo, int count, int iterations = 1)
        {
            var img = neo.Image;
            for (var i = 0; i < 255 * iterations; i++)
            {
                for (var j = 0; j < count; j++)
                {
                    img.SetPixel(j, 0, Wheel((i + j) & 255));
                }

                neo.Update();
            }
        }

        Color Wheel(int WheelPos)
        {
            WheelPos = 255 - WheelPos;
            if (WheelPos < 85)
            {
                return Color.FromArgb(255 - WheelPos * 3, 0, WheelPos * 3);
            }
            if (WheelPos < 170)
            {
                WheelPos -= 85;
                return Color.FromArgb(0, WheelPos * 3, 255 - WheelPos * 3);
            }
            WheelPos -= 170;
            return Color.FromArgb(WheelPos * 3, 255 - WheelPos * 3, 0);
        }
    }
}
