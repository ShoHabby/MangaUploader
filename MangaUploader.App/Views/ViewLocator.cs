using HanumanInstitute.MvvmDialogs.Avalonia;
using MangaUploader.Core.ViewModels;

namespace MangaUploader.Views;

/// <summary>
/// ViewModel locator
/// </summary>
internal sealed class ViewLocator : StrongViewLocator
{
    public ViewLocator() => Register<MainWindowViewModel, MainWindow>();
}
