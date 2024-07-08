using Microsoft.EntityFrameworkCore;
using NFLDepthCharts.API;
using NFLDepthCharts.API.Data;
using NFLDepthCharts.API.Middleware;
using NFLDepthCharts.API.Models;
using NFLDepthCharts.API.Repositories;
using NFLDepthCharts.API.Services;
using NFLDepthCharts.API.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DepthChartDbContext>(options =>
    options.UseInMemoryDatabase("DepthChartDB"));

builder.Services.AddScoped<IDepthChartDbContext, DepthChartDbContext>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IDepthChartRepository, DepthChartRepository>();
builder.Services.AddScoped<IDepthChartService, DepthChartService>();
builder.Services.AddScoped<IPlayerValidator, PlayerValidator>();
builder.Services.AddScoped<IPositionValidator, PositionValidator>();
builder.Services.AddScoped<IDepthChartEntryValidator, DepthChartEntryValidator>();
builder.Services.AddScoped<IAddPlayerToDepthChartDtoValidator, AddPlayerToDepthChartDtoValidator>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ApiExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seeding Position data, this would be a Migration ordinarily but for speed I'm using an in-memory DB. 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DepthChartDbContext>();
    if (!context.Positions.Any())
    {
        context.Positions.AddRange(
            new Position { Name = "LWR" },
            new Position { Name = "RWR" },
            new Position { Name = "SWR" },
            new Position { Name = "LT" },
            new Position { Name = "LG" },
            new Position { Name = "C" },
            new Position { Name = "RG" },
            new Position { Name = "RT" },
            new Position { Name = "TE" },
            new Position { Name = "QB" },
            new Position { Name = "RB" },
            new Position { Name = "LDE" },
            new Position { Name = "NT" },
            new Position { Name = "RDE" },
            new Position { Name = "LOLB" },
            new Position { Name = "LILB" },
            new Position { Name = "RILB" },
            new Position { Name = "ROLB" },
            new Position { Name = "LCB" },
            new Position { Name = "LS" },
            new Position { Name = "FS" },
            new Position { Name = "RCB" },
            new Position { Name = "NB" },
            new Position { Name = "PT" },
            new Position { Name = "PK" },
            new Position { Name = "LS" },
            new Position { Name = "H" },
            new Position { Name = "KO" },
            new Position { Name = "PR" },
            new Position { Name = "KR" }
            );
        context.SaveChanges();
    }
}

app.Run();
