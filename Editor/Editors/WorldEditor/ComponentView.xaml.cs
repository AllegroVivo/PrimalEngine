using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Editor.Editors;

[ContentProperty("ComponentContent")]
public partial class ComponentView : UserControl
{
    public String Header
    {
        get => (String)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header), typeof(String), typeof(ComponentView));

    public FrameworkElement ComponentContent
    {
        get => (FrameworkElement)GetValue(ComponentContentProperty);
        set => SetValue(ComponentContentProperty, value);
    }

    public static readonly DependencyProperty ComponentContentProperty = DependencyProperty.Register(
        nameof(ComponentContent), typeof(FrameworkElement), typeof(ComponentView));

    
    public ComponentView()
    {
        InitializeComponent();
    }
}

