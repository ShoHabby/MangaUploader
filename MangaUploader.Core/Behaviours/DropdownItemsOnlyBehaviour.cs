using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace MangaUploader.Core.Behaviours;

/// <summary>
/// Behaviour that restricts the AutoCompleteBox to only select items from it's dropdown menu
/// </summary>
public class DropdownItemsOnlyBehaviour : Behavior<AutoCompleteBox>
{
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
        if (this.AssociatedObject is not { IsDropDownOpen: false }) return;

        this.AssociatedObject.PopulateDropDown(this.AssociatedObject, EventArgs.Empty);
        this.AssociatedObject.OpeningDropDown(false);

        if (this.AssociatedObject.IsDropDownOpen) return;

        // We *must* set the field and not the property as we need to avoid the changed event being raised (which prevents the dropdown opening).
        this.AssociatedObject._ignorePropertyChange = true;
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
        if (this.AssociatedObject is { SelectedItem: null })
        {
            this.AssociatedObject.SetValue(AutoCompleteBox.TextProperty, string.Empty);
        }
    }

    /// <summary>
    /// DropDownOpening event handler
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event args</param>
    private void OnDropDownOpening(object? sender, CancelEventArgs e)
    {
        if (this.AssociatedObject is { TextBox.IsReadOnly: true })
        {
            e.Cancel = true;
        }
    }
    #endregion
}