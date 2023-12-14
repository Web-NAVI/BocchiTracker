using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace BocchiTracker.MAUIBlazor.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();
            var userConfigRepo = new ConfigRepository<UserConfig>(new FileSystem());
            userConfigRepo.SetLoadFilename(ConfigModule.GetUserConfigFilePath());
            builder.Services.AddSingleton(p => new CachedConfigRepository<UserConfig>(userConfigRepo));
            builder.Services.AddSingleton(p => new CachedConfigRepository<ProjectConfig>(new ConfigRepository<ProjectConfig>(new FileSystem())));
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
