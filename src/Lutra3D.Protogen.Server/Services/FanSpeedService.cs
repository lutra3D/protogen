using Microsoft.Extensions.Hosting;
using System.Device.Pwm;

namespace Lutra3D.Protogen.Server.Services;

public class FanSpeedService(ProtogenManager protogenManager, PwmChannel pwmChannel) : BackgroundService()
{
    protected sealed override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield(); //Give App chance to init

        pwmChannel.Start();

        Console.WriteLine("PWM signal started. Press any key to change brightness gradually...");
        Console.ReadKey();

        while (!ct.IsCancellationRequested)
        {
            pwmChannel.DutyCycle = await protogenManager.GetFanSpeedFractionAsync(ct);
            await Task.Delay(200, ct);
        }
        pwmChannel.Stop();
    }
}