using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using MangaUploader.ViewModels;
using MangaUploader.Views;

namespace MangaUploader;

/// <summary>
/// Application root
/// </summary>
public class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (this.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }

        // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
        // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
        DisableAvaloniaDataAnnotationValidation();

        desktop.MainWindow = new MainWindow
        {
            DataContext = new MainWindowViewModel(),
        };

        base.OnFrameworkInitializationCompleted();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        DataAnnotationsValidationPlugin[] toRemove = BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();
        // Remove each entry found
        Array.ForEach(toRemove, plugin => BindingPlugins.DataValidators.Remove(plugin));
    }
}
