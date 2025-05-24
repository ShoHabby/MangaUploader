using HanumanInstitute.MvvmDialogs.Avalonia;
using MangaUploader.ViewModels;
using MangaUploader.Views;

namespace MangaUploader;

/// <summary>
/// ViewModel locator
/// </summary>
public class ViewLocator : StrongViewLocator
{
    public ViewLocator() => Register<MainWindowViewModel, MainWindow>();
}
