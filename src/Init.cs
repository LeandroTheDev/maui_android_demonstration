using Camera.MAUI;

namespace Android_Native_Demonstration;

public static class Init
{
    public static MauiApp CriarAplicacao()
    {
        var builder = MauiApp.CreateBuilder();
        builder = Builder_Construct(builder);

        return builder.Build();
    }

    /// <summary>
    ///  Construct the builder to initialize the application with the necessary plugins
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static MauiAppBuilder Builder_Construct(MauiAppBuilder builder)
    {
        // Add the plugins and necessary resources here
        builder
                // Maui Framework
                .UseMauiApp<App>()
                // Camera Plugin
                .UseMauiCameraView()
                // Fonts Declaration
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
        return builder;
    }
}