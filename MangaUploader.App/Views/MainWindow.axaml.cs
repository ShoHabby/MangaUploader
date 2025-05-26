using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace MangaUploader.Views;

/// <summary>
/// Main window view
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Window Initialization
    /// </summary>
    public MainWindow() => InitializeComponent();

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

        Settings.Default.Save();

        base.OnClosing(e);
    }
}
