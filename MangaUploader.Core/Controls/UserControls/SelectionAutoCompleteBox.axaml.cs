using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace MangaUploader.Core.Controls.UserControls;

/// <summary>
/// Selection-limited <see cref="AutoCompleteBox"/>
/// </summary>
public sealed partial class SelectionAutoCompleteBox : UserControl
{
    #region Styled Properties
    /// <summary>
    /// Defines the <see cref="ItemsSource"/> property
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty.Register<SelectionAutoCompleteBox, IEnumerable?>(nameof(ItemsSource));
    /// <summary>
    /// Gets/sets the AutoCompleteBox's items
    /// </summary>
    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="SelectedItem"/> property
    /// </summary>
    public static readonly StyledProperty<object?> SelectedItemProperty = AvaloniaProperty.Register<SelectionAutoCompleteBox, object?>(nameof(SelectedItem));
    /// <summary>
    /// Gets/sets the AutoCompleteBox's selected item
    /// </summary>
    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="SelectCommand"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand?> SelectCommandProperty = AvaloniaProperty.Register<SelectionAutoCompleteBox, ICommand?>(nameof(SelectCommand));
    /// <summary>
    /// Gets/sets the inner button's command
    /// </summary>
    public ICommand? SelectCommand
    {
        get => GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="SelectButtonContent"/> property
    /// </summary>
    public static readonly StyledProperty<object?> SelectButtonContentProperty = AvaloniaProperty.Register<SelectionAutoCompleteBox, object?>(nameof(SelectButtonContent));
    /// <summary>
    /// Gets/sets the inner button's content
    /// </summary>
    public object? SelectButtonContent
    {
        get => GetValue(SelectButtonContentProperty);
        set => SetValue(SelectButtonContentProperty, value);
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Control constructor
    /// </summary>
    public SelectionAutoCompleteBox() => InitializeComponent();
    #endregion
}

