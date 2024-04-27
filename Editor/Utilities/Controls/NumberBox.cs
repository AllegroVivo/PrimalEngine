using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Editor.Utilities.Controls;

[TemplatePart(Name = "PART_textBlock", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_textBox", Type = typeof(TextBox))]
class NumberBox : Control
{
    private Double _originalValue;
    private Double _mouseXStart;
    private Double _multiplier;
    
    private Boolean _captured = false;
    private Boolean _valueChanged = false;
    
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(String), typeof(NumberBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public String Value
    {
        get => (String)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public static readonly DependencyProperty MultiplierProperty =
        DependencyProperty.Register(nameof(Multiplier), typeof(Double), typeof(NumberBox),
            new PropertyMetadata(1.0));

    public Double Multiplier
    {
        get => (Double)GetValue(MultiplierProperty);
        set => SetValue(MultiplierProperty, value);
    }
    
    static NumberBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), new FrameworkPropertyMetadata(typeof(NumberBox)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_textBlock") is TextBlock textblock)
        {
            textblock.MouseLeftButtonDown += OnTextBlock_MouseLBD;
            textblock.MouseLeftButtonUp += OnTextBlock_MouseLBU;
            textblock.MouseMove += OnTextBlock_MouseMove;
        }
    }

    private void OnTextBlock_MouseLBD(Object sender, MouseButtonEventArgs e)
    {
        Double.TryParse(Value, out _originalValue);
        Mouse.Capture(sender as UIElement);
        
        _captured = true;
        _valueChanged = false;
        e.Handled = true;

        _mouseXStart = e.GetPosition(this).X;
    }

    private void OnTextBlock_MouseLBU(Object sender, MouseButtonEventArgs e)
    {
        if (_captured)
        {
            Mouse.Capture(null);
            _captured = false;
            e.Handled = true;

            if (!_valueChanged && GetTemplateChild("PART_textBox") is TextBox textBox)
            {
                textBox.Visibility = Visibility.Visible;
                textBox.Focus();
                textBox.SelectAll();
            }
        }
    }

    private void OnTextBlock_MouseMove(Object sender, MouseEventArgs e)
    {
        if (_captured)
        {
            Double mouseX = e.GetPosition(this).X;
            Double d = mouseX - _mouseXStart;
            
            if (Math.Abs(d) > SystemParameters.MinimumHorizontalDragDistance)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    _multiplier = 0.001;
                else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    _multiplier = 0.1;
                else
                    _multiplier = 0.01;
                
                var newValue = _originalValue + d * _multiplier * Multiplier;
                Value = newValue.ToString("0.#####");
                _valueChanged = true;
            }
        }
    }
}