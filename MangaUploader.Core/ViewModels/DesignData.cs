#if DEBUG
using CommunityToolkit.Mvvm.DependencyInjection;

namespace MangaUploader.Core.ViewModels;

/// <summary>
/// Previewer data
/// </summary>
public static class DesignData
{
    #region Properties
    /// <summary>
    /// MainWindow view design preview
    /// </summary>
    public static MainWindowViewModel MainWindowViewModel { get; } = Ioc.Default.GetRequiredService<MainWindowViewModel>();
    #endregion
}
#endif
