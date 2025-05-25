using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace MangaUploader.Behaviours;

/// <summary>
/// Behaviour that restricts the AutoCompleteBox to only select items from it's dropdown menu
/// </summary>
public class DropdownItemsOnlyBehaviour : Behavior<AutoCompleteBox>
{
    #region Constants
    /// <summary>
    /// Reflection binding flags
    /// </summary>
    private const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;
    /// <summary>
    /// AutoCompleteBox type
    /// </summary>
    private static readonly Type AutoCompleteBoxType = typeof(AutoCompleteBox);
    #endregion

    #region Overrides
    /// <inheritdoc />
    protected override void OnAttached()
    {
        if (this.AssociatedObject is not null)
        {
            this.AssociatedObject.KeyUp           += OnKeyUp;
            this.AssociatedObject.GotFocus        += OnGotFocus;
            this.AssociatedObject.LostFocus       += OnLostFocus;
            this.AssociatedObject.DropDownOpening += OnDropDownOpening;
        }

        base.OnAttached();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (this.AssociatedObject is not null)
        {
            this.AssociatedObject.KeyUp           -= OnKeyUp;
            this.AssociatedObject.GotFocus        -= OnGotFocus;
            this.AssociatedObject.LostFocus       -= OnLostFocus;
            this.AssociatedObject.DropDownOpening -= OnDropDownOpening;
        }

        base.OnDetaching();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Show the AutoCompleteBox dropdown
    /// </summary>
    private void ShowDropdown()
    {
        if (this.AssociatedObject is null or { IsDropDownOpen: true }) return;

        AutoCompleteBoxType.GetMethod("PopulateDropDown", FLAGS)?.Invoke(this.AssociatedObject, [this.AssociatedObject, EventArgs.Empty]);
        AutoCompleteBoxType.GetMethod("OpeningDropDown", FLAGS)?.Invoke(this.AssociatedObject, [false]);

        if (this.AssociatedObject.IsDropDownOpen) return;

        // We *must* set the field and not the property as we need to avoid the changed event being raised (which prevents the dropdown opening).
        FieldInfo? ipc = AutoCompleteBoxType.GetField("_ignorePropertyChange", FLAGS);

        if (ipc?.GetValue(this.AssociatedObject) is false)
        {
            ipc.SetValue(this.AssociatedObject, true);
        }

        this.AssociatedObject.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, true);
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
        if (this.AssociatedObject is null or { Text: null } or { ItemsSource: null }) return;

        foreach (object item in this.AssociatedObject.ItemsSource)
        {
            if (this.AssociatedObject.Text == item.ToString()) return;
        }

        this.AssociatedObject.Text = string.Empty;
    }

    /// <summary>
    /// DropDownOpening event handler
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event args</param>
    private void OnDropDownOpening(object? sender, CancelEventArgs e)
    {
        PropertyInfo? prop = this.AssociatedObject?.GetType().GetProperty("TextBox", FLAGS);
        object? value = prop?.GetValue(this.AssociatedObject);
        if (value is TextBox { IsReadOnly: true })
        {
            e.Cancel = true;
        }
    }
    #endregion
}