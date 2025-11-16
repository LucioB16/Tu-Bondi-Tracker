using System.Globalization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TuBondi.Presentation.WebApi.Services;

namespace TuBondi.Presentation.WebApi.TagHelpers;

/// <summary>
/// Renderiza el contenedor visual del tablero LED.
/// </summary>
[HtmlTargetElement("led-board")]
public sealed class LedBoardTagHelper : TagHelper
{
    private readonly IBoardClientConfigProvider _configProvider;

    public LedBoardTagHelper(IBoardClientConfigProvider configProvider)
    {
        _configProvider = configProvider;
    }

    public string Endpoint { get; set; } = "/api/arrivals";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var config = _configProvider.GetConfig();
        output.TagName = "section";
        output.Attributes.SetAttribute("class", "led-board");
        output.Attributes.SetAttribute("data-endpoint", Endpoint);
        output.Attributes.SetAttribute("data-lines", string.Join(',', config.Lines));
        output.Attributes.SetAttribute("data-stop", config.StopCode);
        output.Attributes.SetAttribute("data-stop-description", config.StopDescription);
        output.Attributes.SetAttribute("data-refresh-ms", config.UpdateInterval.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
        output.Content.SetHtmlContent(@"<header class='led-board__header'><div class='led-board__title'></div><button class='led-board__contrast' aria-pressed='false'>Alto contraste</button></header><div class='led-board__body' aria-live='polite'></div><footer class='led-board__footer'><div class='led-board__clock'></div><div class='led-board__status'></div></footer>");
    }
}
