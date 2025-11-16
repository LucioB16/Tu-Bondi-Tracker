using Microsoft.AspNetCore.Mvc.RazorPages;
using TuBondi.Presentation.WebApi.Services;

namespace TuBondi.Presentation.WebApi.Pages;

/// <summary>
/// Página principal que aloja el tablero LED.
/// </summary>
public sealed class IndexModel : PageModel
{
    private readonly IBoardClientConfigProvider _configProvider;

    public string StopCode { get; private set; } = string.Empty;
    public string StopDescription { get; private set; } = string.Empty;

    public IndexModel(IBoardClientConfigProvider configProvider)
    {
        _configProvider = configProvider;
    }

    public void OnGet()
    {
        var config = _configProvider.GetConfig();
        StopCode = config.StopCode;
        StopDescription = config.StopDescription;
        ViewData["Title"] = $"Transit Tracker - {StopDescription}";
    }
}
