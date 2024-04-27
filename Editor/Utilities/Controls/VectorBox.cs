using System;
using System.Windows;
using System.Windows.Controls;

namespace Editor.Utilities.Controls;

public enum VectorType
{
    Vector2,
    Vector3,
    Vector4
}

class VectorBox : Control
{
    public static readonly DependencyProperty VectorTypeProperty =
        DependencyProperty.Register(nameof(VectorType), typeof(VectorType), typeof(VectorBox),
            new PropertyMetadata(VectorType.Vector3));

    public VectorType VectorType
    {
        get => (VectorType)GetValue(VectorTypeProperty);
        set => SetValue(VectorTypeProperty, value);
    }
    
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VectorBox),
            new PropertyMetadata(Orientation.Horizontal));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    
    public static readonly DependencyProperty MultiplierProperty =
        DependencyProperty.Register(nameof(Multiplier), typeof(Double), typeof(VectorBox),
            new PropertyMetadata(1.0));

    public Double Multiplier
    {
        get => (Double)GetValue(MultiplierProperty);
        set => SetValue(MultiplierProperty, value);
    }
    
    public static readonly DependencyProperty XProperty =
        DependencyProperty.Register(nameof(X), typeof(String), typeof(VectorBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public String X
    {
        get => (String)GetValue(XProperty);
        set => SetValue(XProperty, value);
    }
    
    public static readonly DependencyProperty YProperty =
        DependencyProperty.Register(nameof(Y), typeof(String), typeof(VectorBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public String Y
    {
        get => (String)GetValue(YProperty);
        set => SetValue(YProperty, value);
    }
    
    public static readonly DependencyProperty ZProperty =
        DependencyProperty.Register(nameof(Z), typeof(String), typeof(VectorBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public String Z
    {
        get => (String)GetValue(ZProperty);
        set => SetValue(ZProperty, value);
    }
    
    public static readonly DependencyProperty WProperty =
        DependencyProperty.Register(nameof(W), typeof(String), typeof(VectorBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public String W
    {
        get => (String)GetValue(WProperty);
        set => SetValue(WProperty, value);
    }
    
    static VectorBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(VectorBox),
            new FrameworkPropertyMetadata(typeof(VectorBox)));
    }
}