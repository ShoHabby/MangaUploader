using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

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

    #region Overrides
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        this.AutoCompleteBox.KeyUp           += OnKeyUp;
        this.AutoCompleteBox.GotFocus        += OnGotFocus;
        this.AutoCompleteBox.LostFocus       += OnLostFocus;
        this.AutoCompleteBox.DropDownOpening += OnDropDownOpening;
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        this.AutoCompleteBox.KeyUp           -= OnKeyUp;
        this.AutoCompleteBox.GotFocus        -= OnGotFocus;
        this.AutoCompleteBox.LostFocus       -= OnLostFocus;
        this.AutoCompleteBox.DropDownOpening -= OnDropDownOpening;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Show the AutoCompleteBox dropdown
    /// </summary>
    private void ShowDropdown()
    {
        if (this.AutoCompleteBox is { IsDropDownOpen: true } or { SelectedItem: not null }) return;

        this.AutoCompleteBox.PopulateDropDown(this.AutoCompleteBox, EventArgs.Empty);
        this.AutoCompleteBox.OpeningDropDown(false);

        if (this.AutoCompleteBox.IsDropDownOpen) return;

        // We *must* set the field and not the property as we need to avoid the changed event being raised (which prevents the dropdown opening).
        this.AutoCompleteBox._ignorePropertyChange = true;
        this.AutoCompleteBox.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, true);
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// KeyUp event handler
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event args</param>
    private void OnKeyUp(object? sender, KeyEventArgs e) => ShowDropdown();

    /// <summary>
    /// GotFocus event handler
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event args</param>
    private void OnGotFocus(object? sender, GotFocusEventArgs e) => ShowDropdown();

    /// <summary>
    /// LostFocus event handler
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event args</param>
    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (this.AutoCompleteBox.SelectedItem is null)
        {
            this.AutoCompleteBox.SetValue(AutoCompleteBox.TextProperty, string.Empty);
        }
    }

    /// <summary>
    /// DropDownOpening event handler
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event args</param>
    private void OnDropDownOpening(object? sender, CancelEventArgs e)
    {
        if (this.AutoCompleteBox.TextBox is { IsReadOnly: true })
        {
            e.Cancel = true;
        }
    }
    #endregion
}

