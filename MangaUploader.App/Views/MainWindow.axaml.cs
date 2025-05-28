using System;
using Avalonia.Controls;

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

        this.Width  = Settings.Default.Width;
        this.Height = Settings.Default.Height;
    }

    /// <inheritdoc />
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Settings.Default.Width  = (int)Math.Round(this.Width);
        Settings.Default.Height = (int)Math.Round(this.Height);

        if (this.RepositoriesAutoCompleteBox.SelectedItem is not null)
        {
            Settings.Default.SelectedRepository = this.RepositoriesAutoCompleteBox.Text;
        }

        Settings.Default.Save();

        base.OnClosing(e);
    }
    #endregion
}
