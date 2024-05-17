﻿using Android_Native_Demonstration.Pages;
using Camera.MAUI;
using Plugin.LocalNotification;

namespace Android_Native_Demonstration;

public static class Init
{
    public static MauiApp CriarAplicacao()
    {
        var builder = MauiApp.CreateBuilder();
        builder = BuilderConstruct(builder);

        return builder.Build();
    }

    /// <summary>
    ///  Construct the builder to initialize the application with the necessary plugins
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static MauiAppBuilder BuilderConstruct(MauiAppBuilder builder)
    {
        // Add the plugins and necessary resources here
        builder
                // Maui Framework
                .UseMauiApp<App>()
                // Camera Plugin
                .UseMauiCameraView()
                // Notification Plugin
                .UseLocalNotification()
                // Fonts Declaration
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

        // Initialize OCR Recognition
        builder.Services.AddLogging();
        builder.Services.AddSingleton<Visualization>();
        return builder;
    }
}