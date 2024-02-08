using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PrimalEditor.Dictionaries;

public partial class ControlTemplates : ResourceDictionary
{
    private void OnTextBox_KeyDown(Object sender, KeyEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        
        BindingExpression exp = textBox!.GetBindingExpression(TextBox.TextProperty);
        if (exp == null)
            return;

        if (e.Key == Key.Enter)
        {
            if (textBox!.Tag is ICommand command && command.CanExecute(textBox.Text))
                command.Execute(textBox.Text);
            else
                exp.UpdateSource();
            
            Keyboard.ClearFocus();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            exp.UpdateTarget();
            Keyboard.ClearFocus();
        }
    }

    private void OnCloseButton_Click(Object sender, RoutedEventArgs e)
    {
        Window window = (Window)((FrameworkElement)sender).TemplatedParent;
        window.Close();
    }

    private void OnMaximizeRestoreButton_Click(Object sender, RoutedEventArgs e)
    {
        Window window = (Window)((FrameworkElement)sender).TemplatedParent;
        window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
    }

    private void OnMinimizeButton_Click(Object sender, RoutedEventArgs e)
    {
        Window window = (Window)((FrameworkElement)sender).TemplatedParent;
        window.WindowState = WindowState.Minimized;
    }

    private void OnTextBoxRename_KeyDown(Object sender, KeyEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        
        BindingExpression exp = textBox!.GetBindingExpression(TextBox.TextProperty);
        if (exp == null)
            return;

        if (e.Key == Key.Enter)
        {
            if (textBox!.Tag is ICommand command && command.CanExecute(textBox.Text))
                command.Execute(textBox.Text);
            else
                exp.UpdateSource();

            textBox.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            exp.UpdateTarget();
            textBox.Visibility = Visibility.Collapsed;
        }
    }

    private void OnTextBoxRename_LostFocus(Object sender, RoutedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (!textBox!.IsVisible)
            return;
        
        BindingExpression exp = textBox!.GetBindingExpression(TextBox.TextProperty);

        if (exp != null)
        {
            exp.UpdateTarget();
            textBox.Visibility = Visibility.Collapsed;
        }
    }
}