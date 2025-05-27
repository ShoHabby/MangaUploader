using HanumanInstitute.MvvmDialogs.Avalonia;
using MangaUploader.Core.ViewModels;
using MangaUploader.Views;

namespace MangaUploader;

/// <summary>
/// ViewModel locator
/// </summary>
public sealed class ViewLocator : StrongViewLocator
{
    public ViewLocator() => Register<MainWindowViewModel, MainWindow>();
}
