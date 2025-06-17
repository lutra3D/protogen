using System.Device.Pwm;

namespace Lutra3D.Protogen.Server.Services;

public class FanSpeedService(ProtogenManager protogenManager) : BackgroundService()
{
    protected sealed override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Yield(); //Give App chance to init

        using PwmChannel pwmChannel = PwmChannel.Create(0, 19, frequency: 25000, dutyCyclePercentage: 0.5);

        // Start the PWM signal
        pwmChannel.Start();

        Console.WriteLine("PWM signal started. Press any key to change brightness gradually...");
        Console.ReadKey();

        while (!ct.IsCancellationRequested)
        {
            pwmChannel.DutyCycle = await protogenManager.GetFanSpeedFractionAsync(ct);
            await Task.Delay(200, ct);
        }

    }
}
