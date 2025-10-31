using BlazorFurniture.Client.Enums;
using BlazorFurniture.Client.Services.Interfaces;
using MudBlazor;

namespace BlazorFurniture.Client.Services;

public sealed class ThemeService : IThemeService
{
    private bool _isDarkMode = false;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
                OnThemeChanged?.Invoke();
            }
        }
    }

    public event Action? OnThemeChanged;

    public MudTheme GetTheme( ThemeType type = ThemeType.Default )
    {
        return type switch
        {
            ThemeType.Public => GetPublicTheme(),
            ThemeType.User => GetUserTheme(),
            ThemeType.Admin => GetAdminTheme(),
            _ => GetDefaultTheme()
        };
    }

    private static MudTheme GetDefaultTheme() => new()
    {
        PaletteLight = new PaletteLight()
        {
            Black = "#110e2d",
            AppbarText = "#424242",
            AppbarBackground = "rgba(255,255,255,1)",
            DrawerBackground = "#ffffff",
            GrayLight = "#e8e8e8",
            GrayLighter = "#f9f9f9",
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#7e6fff",
            Surface = "#1e1e2d",
            Background = "#1a1a27",
            BackgroundGray = "#151521",
            AppbarText = "#92929f",
            AppbarBackground = "rgba(26,26,39,1)",
            DrawerBackground = "#1a1a27",
            ActionDefault = "#74718e",
            ActionDisabled = "#9999994d",
            ActionDisabledBackground = "#605f6d4d",
            TextPrimary = "#b2b0bf",
            TextSecondary = "#92929f",
            TextDisabled = "#ffffff33",
            DrawerIcon = "#92929f",
            DrawerText = "#92929f",
            GrayLight = "#2a2833",
            GrayLighter = "#1e1e2d",
            Info = "#4a86ff",
            Success = "#3dcb6c",
            Warning = "#ffb545",
            Error = "#ff3f5f",
            LinesDefault = "#33323e",
            TableLines = "#33323e",
            Divider = "#292838",
            OverlayLight = "#1e1e2d80",
        }
    };

    private static MudTheme GetPublicTheme() => new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#2196F3", // Blue for public
            AppbarText = "#424242",
            AppbarBackground = "rgba(255,255,255,1)",
            DrawerBackground = "#ffffff",
            Background = "#f5f5f5",
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#42A5F5", // Lighter blue
            Surface = "#1e1e2d",
            Background = "#121212",
            AppbarBackground = "rgba(30,30,45,1)",
        }
    };

    private static MudTheme GetUserTheme() => new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#4CAF50", // Green for users
            AppbarText = "#424242",
            AppbarBackground = "rgba(255,255,255,1)",
            DrawerBackground = "#ffffff",
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#7e6fff",
            Surface = "#1e1e2d",
            Background = "#1a1a27",
            BackgroundGray = "#151521",
            AppbarText = "#92929f",
            AppbarBackground = "rgba(26,26,39,1)",
            DrawerBackground = "#1a1a27",
            ActionDefault = "#74718e",
            ActionDisabled = "#9999994d",
            ActionDisabledBackground = "#605f6d4d",
            TextPrimary = "#b2b0bf",
            TextSecondary = "#92929f",
            TextDisabled = "#ffffff33",
            DrawerIcon = "#92929f",
            DrawerText = "#92929f",
            GrayLight = "#2a2833",
            GrayLighter = "#1e1e2d",
            Info = "#4a86ff",
            Success = "#3dcb6c",
            Warning = "#ffb545",
            Error = "#ff3f5f",
            LinesDefault = "#33323e",
            TableLines = "#33323e",
            Divider = "#292838",
            OverlayLight = "#1e1e2d80",
        }
    };

    private static MudTheme GetAdminTheme() => new()
    {
        PaletteLight = new PaletteLight()
        {
            White = "#FFFFFF",
            Black = "#110e2d",
            AppbarText = "#424242",
            AppbarBackground = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            GrayLight = "#e8e8e8",
            GrayLighter = "#f9f9f9",
            Background = "#F6F6F9",
            Surface = "#FFFFFF"
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#EF5350",
            Surface = "#2B2B40",
            Background = "#1a1a27",
            BackgroundGray = "#151521",
            AppbarText = "#92929f",
            AppbarBackground = "rgba(26,26,39,1)",
            DrawerBackground = "#1a1a27",
            ActionDefault = "#74718e",
            ActionDisabled = "#9999994d",
            ActionDisabledBackground = "#605f6d4d",
            TextPrimary = "#b2b0bf",
            TextSecondary = "#92929f",
            TextDisabled = "#ffffff33",
            DrawerIcon = "#92929f",
            DrawerText = "#92929f",
            GrayLight = "#2a2833",
            GrayLighter = "#1e1e2d",
            Info = "#4a86ff",
            Success = "#3dcb6c",
            Warning = "#ffb545",
            Error = "#ff3f5f",
            LinesDefault = "#33323e",
            TableLines = "#33323e",
            Divider = "#292838",
            OverlayLight = "#1e1e2d80",
        }
    };

    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
    }
}
