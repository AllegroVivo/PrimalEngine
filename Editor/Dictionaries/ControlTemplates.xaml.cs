using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Editor.Dictionaries;

public partial class ControlTemplates : ResourceDictionary
{
    private void OnTextBox_Keydown(Object sender, KeyEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        BindingExpression exp = textBox!.GetBindingExpression(TextBox.TextProperty);
        
        if (exp == null)
            return;

        if (e.Key == Key.Enter)
        {
            if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
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
    
    private void OnClose_Button_Click(Object sender, RoutedEventArgs _)
    {
        Window window = (Window)((FrameworkElement)sender).TemplatedParent;
        window.Close();
    }

    private void OnMaximizeRestore_Button_Click(Object sender, RoutedEventArgs _)
    {
        Window window = (Window)((FrameworkElement)sender).TemplatedParent;
        window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
    }

    private void OnMinimize_Button_Click(Object sender, RoutedEventArgs _)
    {
        Window window = (Window)((FrameworkElement)sender).TemplatedParent;
        window.WindowState = WindowState.Minimized;
    }

    private void OnRenameTextBox_Keydown(Object sender, KeyEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        BindingExpression exp = textBox!.GetBindingExpression(TextBox.TextProperty);
        
        if (exp == null)
            return;

        if (e.Key == Key.Enter)
        {
            if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
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

    private void OnRenameTextBox_FocusLost(Object sender, RoutedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        BindingExpression exp = textBox!.GetBindingExpression(TextBox.TextProperty);

        if (exp != null)
        {
            exp.UpdateSource();
            textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            textBox.Visibility = Visibility.Collapsed;
        }
    }
}