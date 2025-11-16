using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TuBondi.Application.Extensions;
using TuBondi.Application.Queries.GetArrivalsForBoard;
using TuBondi.Infrastructure.DependencyInjection;
using TuBondi.Presentation.WebApi.Models;
using TuBondi.Presentation.WebApi.Options;
using TuBondi.Presentation.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
builder.Services.Configure<TrackerOptions>(builder.Configuration.GetSection("Tracker"));
builder.Services.AddSingleton<IBoardClientConfigProvider, BoardClientConfigProvider>();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddResponseCompression();

var app = builder.Build();

app.UseResponseCompression();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/api/arrivals", async (
        [FromQuery] string? stop,
        [FromQuery] string? lines,
        IGetArrivalsForBoardQueryHandler handler,
        IBoardClientConfigProvider configProvider,
        HttpContext http,
        CancellationToken ct) =>
    {
        var defaults = configProvider.GetConfig();
        var targetStop = string.IsNullOrWhiteSpace(stop) ? defaults.StopCode : stop.Trim();
        IEnumerable<string> requestedLines = !string.IsNullOrWhiteSpace(lines)
            ? lines.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : defaults.Lines;
        if (!requestedLines.Any())
        {
            requestedLines = defaults.Lines;
        }

        var query = new GetArrivalsForBoardQuery(targetStop, requestedLines.ToArray());
        var result = await handler.HandleAsync(query, ct);
        var payload = result.Arrivals
            .Select(arrival => new ArrivalApiModel(
                arrival.Line,
                arrival.EtaMinutes,
                arrival.DistanceKm,
                arrival.Route,
                arrival.Direction,
                arrival.Operator,
                arrival.ColorHex,
                arrival.Notification,
                arrival.RawMessage))
            .ToList();

        http.Response.Headers["X-Stop-Code"] = result.Stop.Code;
        http.Response.Headers["X-Stop-Description"] = result.Stop.Description;
        http.Response.Headers["X-Board-GeneratedAt"] = result.GeneratedAt.ToOffset(TimeSpan.FromHours(-3)).ToString("O");
        if (result.Notifications.Count > 0)
        {
            http.Response.Headers["X-Board-Notifications"] = string.Join('|', result.Notifications.Select(n => n.Message));
        }

        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
        {
            http.Response.Headers["X-Board-Error"] = result.ErrorMessage;
        }

        return Results.Ok(payload);
    })
    .WithName("GetArrivals")
    .Produces<List<ArrivalApiModel>>(StatusCodes.Status200OK);

app.MapRazorPages();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapFallbackToPage("/Index");

app.Run();
