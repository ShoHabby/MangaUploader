using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using MangaUploader.Core.Services;
using MangaUploader.Core.ViewModels;
using MangaUploader.Services;
using MangaUploader.Views;
using Microsoft.Extensions.DependencyInjection;

namespace MangaUploader;

/// <summary>
/// Application root
/// </summary>
public class App : Application
{
    /// <inheritdoc />
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        if (this.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }

        // Cleanup Avalonia plugins
        DisableAvaloniaDataAnnotationValidation();

        //Generate Dialog service
        ServiceCollection services = new();
        services.AddSingleton<IDialogService, DialogService>(provider =>
        {
            IViewLocator locator = new ViewLocator();
            IDialogFactory dialogFactory = new DialogFactory();
            return new DialogService(new DialogManager(locator, dialogFactory), provider.GetRequiredService);
        });

        // Add other services
        services.AddSingleton<IGitHubService, GitHubService>();

        // Add VM transients
        services.AddTransient<MainWindowViewModel>();

        // Start and inject
        ServiceProvider provider = services.BuildServiceProvider();
        desktop.MainWindow = new MainWindow
        {
            DataContext = provider.GetRequiredService<MainWindowViewModel>()
        };

        base.OnFrameworkInitializationCompleted();
    }

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
}
