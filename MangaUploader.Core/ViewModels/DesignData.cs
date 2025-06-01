#if DEBUG
using CommunityToolkit.Mvvm.DependencyInjection;

namespace MangaUploader.Core.ViewModels;

/// <summary>
/// Previewer data
/// </summary>
public static class DesignData
{
    public static MainWindowViewModel MainWindowViewModel { get; } = Ioc.Default.GetRequiredService<MainWindowViewModel>();
}
#endif
