using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input.Platform;

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
        App.Settings.WindowWidth  = (int)Math.Round(this.Width);
        App.Settings.WindowHeight = (int)Math.Round(this.Height);

        if (this.RepositoriesAutoCompleteBox.SelectedItem is not null)
        {
            App.Settings.SavedRepositoryName = this.RepositoriesAutoCompleteBox.Text ?? string.Empty;
        }

        base.OnClosing(e);
    }
    #endregion
}
