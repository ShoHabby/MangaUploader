using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.DependencyInjection;
using Config.Net;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using MangaUploader.Core;
using MangaUploader.Core.Services;
using MangaUploader.Core.ViewModels;
using MangaUploader.Services;
using MangaUploader.Views;
using Microsoft.Extensions.DependencyInjection;

namespace MangaUploader;

/// <summary>
/// Application root
/// </summary>
public sealed class App : Application
{
    #region Constants
    /// <summary>
    /// Application settings config file name
    /// </summary>
    private const string CONFIG_FILE = "appsettings.json";
    #endregion

    #region Static properties
    /// <summary>
    /// Application settings
    /// </summary>
    internal static IAppSettings Settings { get; private set; } = null!;
    #endregion

    #region Overrides
    /// <inheritdoc />
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        // Ensure proper app lifetime
        if (this.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }

        // Cleanup Avalonia plugins
        DisableAvaloniaDataAnnotationValidation();

        // Create settings
        Settings = new ConfigurationBuilder<IAppSettings>().UseJsonFile(CONFIG_FILE).Build();

        // Create service collection
        ServiceCollection services = new();

        // Add dialog service
        IViewLocator viewLocator     = new ViewLocator();
        IDialogFactory dialogFactory = new DialogFactory();
        DialogManager dialogManager  = new(viewLocator, dialogFactory);
        DialogService dialogService  = new(dialogManager, t => Ioc.Default.GetService(t));
        services.AddSingleton<IDialogService>(dialogService);

        // Add other services
        services.AddSingleton<IGitHubService, GitHubService>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddSingleton<ICubariService, CubariService>();
        services.AddSingleton<IAppSettingsService, AppSettingsService>();

        // Add ViewModels
        services.AddTransient<MainWindowViewModel>();

        // Start and inject
        Ioc.Default.ConfigureServices(services.BuildServiceProvider());
        desktop.MainWindow = new MainWindow
        {
            DataContext = Ioc.Default.GetRequiredService<MainWindowViewModel>()
        };

        base.OnFrameworkInitializationCompleted();
    }
    #endregion

    #region Static Methods
    /// <summary>
    /// Gets the current top level application
    /// </summary>
    /// <returns>Top level app</returns>
    /// <exception cref="InvalidOperationException">If the application lifetime does not allow getting a top level</exception>
    public static TopLevel GetTopLevel() => Current!.ApplicationLifetime switch
    {
        IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow!,
        ISingleViewApplicationLifetime viewApp          => viewApp.MainView?.GetVisualRoot() as TopLevel ?? throw new InvalidOperationException("Unable to correctly get TopLevel from current lifetime"),
        _                                               => throw new InvalidOperationException("Current application lifetime does not support getting TopLevel")
    };

    /// <summary>
    /// Avoid duplicate validations from both Avalonia and the CommunityToolkit.
    /// </summary>
    /// <remarks>More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins</remarks>
    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        DataAnnotationsValidationPlugin[] toRemove = BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();
        // Remove each entry found
        Array.ForEach(toRemove, plugin => BindingPlugins.DataValidators.Remove(plugin));
    }
    #endregion
}
