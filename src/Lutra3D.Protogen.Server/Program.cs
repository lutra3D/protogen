using Lutra3D.Protogen.Server;
using Lutra3D.Protogen.Server.Services;
using RPiRgbLEDMatrix;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddSingleton<ProtogenManager>();
builder.Services.AddHostedService<LedMatrixRedrawHostedService>();

builder.Services.AddControllers();

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