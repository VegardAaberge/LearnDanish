using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace LearnDanish.Controls;

public partial class CircleIcon : ContentView
{
    public CircleIcon()
    {
        InitializeComponent();
    }

    /// <summary>
    /// The Icon Displayed on the Frame
    /// </summary>
    public static readonly BindableProperty IconTextProperty =
        BindableProperty.Create(nameof(IconText), typeof(string), typeof(CircleIcon), default(string));

    public string IconText
    {
        get { return (string)GetValue(IconTextProperty); }
        set { SetValue(IconTextProperty, value); }
    }

    /// <summary>
    /// The label text color
    /// </summary>
    public static readonly BindableProperty LabelTextColorProperty =
        BindableProperty.Create(nameof(LabelTextColor), typeof(Color), typeof(CircleIcon), default(Color));

    public Color LabelTextColor
    {
        get { return (Color)GetValue(LabelTextColorProperty); }
        set { SetValue(LabelTextColorProperty, value); }
    }

    /// <summary>
    /// The label font size
    /// </summary>
    public static readonly BindableProperty LabelFontSizeProperty =
        BindableProperty.Create(nameof(LabelFontSize), typeof(double), typeof(CircleIcon), default(double));

    public double LabelFontSize
    {
        get { return (double)GetValue(LabelFontSizeProperty); }
        set { SetValue(LabelFontSizeProperty, value); }
    }

    /// <summary>
    /// The Color of the frame
    /// </summary>
    public static readonly BindableProperty FrameColorProperty =
        BindableProperty.Create(nameof(FrameColor), typeof(Color), typeof(CircleIcon), default(Color));

    public Color FrameColor
    {
        get { return (Color)GetValue(FrameColorProperty); }
        set { SetValue(FrameColorProperty, value); }
    }

    /// <summary>
    /// The size of frame
    /// </summary>
    public static readonly BindableProperty FrameSizeProperty =
        BindableProperty.Create(nameof(FrameSize), typeof(double), typeof(CircleIcon), default(double));

    public double FrameSize
    {
        get { return (double)GetValue(FrameSizeProperty); }
        set { SetValue(FrameSizeProperty, value); }
    }

    void PressedGestureRecognizer_Pressed(System.Object sender, System.EventArgs e)
    {
    }

    void ReleasedGestureRecognizer_Released(System.Object sender, System.EventArgs e)
    {
    }
}
