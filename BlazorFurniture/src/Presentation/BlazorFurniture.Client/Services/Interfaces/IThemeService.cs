using BlazorFurniture.Client.Enums;
using MudBlazor;

namespace BlazorFurniture.Client.Services.Interfaces;

public interface IThemeService
{
    MudTheme GetTheme( ThemeType type = ThemeType.Default );
    bool IsDarkMode { get; set; }
    event Action? OnThemeChanged;
    void ToggleDarkMode();
}
