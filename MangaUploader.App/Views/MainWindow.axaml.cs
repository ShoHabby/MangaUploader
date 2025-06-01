using Avalonia.Controls;
using MangaUploader.Core.Models;

namespace MangaUploader.Views;

/// <summary>
/// Main window view
/// </summary>
public sealed partial class MainWindow : Window
{
    #region Constructors
    /// <summary>
    /// Window Initialization
    /// </summary>
    public MainWindow() => InitializeComponent();
    #endregion

    #region Overrides
    /// <inheritdoc />
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        this.Width  = App.Settings.WindowWidth;
        this.Height = App.Settings.WindowHeight;
    }

    /// <inheritdoc />
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        App.Settings.WindowWidth       = (int)Math.Round(this.Width);
        App.Settings.WindowHeight      = (int)Math.Round(this.Height);
        App.Settings.SavedRepositoryID = this.RepositoriesAutoCompleteBox.SelectedItem is RepositoryInfo repository ? repository.ID : null;
        base.OnClosing(e);
    }
    #endregion
}
