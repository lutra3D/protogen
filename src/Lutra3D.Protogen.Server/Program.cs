using Lutra3D.Protogen.Server;
using Lutra3D.Protogen.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rpi_ws281x;
using RPiRgbLEDMatrix;
using System.Device.Pwm;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton((sp) =>
{
    var options = new RGBLedMatrixOptions
    {
        ChainLength = 2,
        Parallel = 1,
        Cols = 64,
        HardwareMapping = "adafruit-hat",
        LimitRefreshRateHz = 100
    };

    return new RGBLedMatrix(options);
});

var settings = Settings.CreateDefaultSettings();
settings.AddController(24, Pin.Gpio10, StripType.WS2812_STRIP, ControllerType.SPI,  128, false);
var neopixel = new WS281x(settings);

using var pwmChannel = PwmChannel.Create(0, 0, frequency: 25000, dutyCyclePercentage: 0.5);

builder.Services.AddSingleton(sp => neopixel);
builder.Services.AddSingleton(sp => pwmChannel);

builder.Services.AddSingleton<ProtogenManager>();
builder.Services.AddHostedService<LedMatrixRedrawHostedService>();
builder.Services.AddHostedService<NeoPixelRedrawHostedService>();
builder.Services.AddHostedService<FanSpeedService>();

builder.Services.AddControllers();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapControllers().WithOpenApi();

app.Run();