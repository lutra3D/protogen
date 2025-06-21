using Lutra3D.Protogen.Server;
using Lutra3D.Protogen.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rpi_ws281x;
using RPiRgbLEDMatrix;

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
        HardwareMapping = "adafruit-hat"
    };

    return new RGBLedMatrix(options);
});

var settings = Settings.CreateDefaultSettings();
settings.Channels[0] = new Channel(24, 18, 128, false, StripType.WS2812_STRIP);
var neopixel = new WS281x(settings);

builder.Services.AddSingleton(sp => neopixel);

builder.Services.AddSingleton<ProtogenManager>();
//builder.Services.AddHostedService<LedMatrixRedrawHostedService>();
builder.Services.AddHostedService<NeoPixelRedrawHostedService>();
//builder.Services.AddHostedService<FanSpeedService>();

builder.Services.AddControllers();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers().WithOpenApi();

app.Run();