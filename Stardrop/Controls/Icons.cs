using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace Stardrop.Controls
{
    /// <summary>
    /// Renders the Material Design icons the application uses. The Projektanker Icons.Avalonia package is
    /// built against Avalonia 11 (it relies on the removed TemplateBinding.ProvideValue/IBinding API), so the
    /// handful of glyphs used by the views are drawn here instead.
    /// </summary>
    public class IconGlyph : Control
    {
        public static readonly StyledProperty<string?> ValueProperty = AvaloniaProperty.Register<IconGlyph, string?>(nameof(Value));
        public static readonly StyledProperty<IBrush?> FillProperty = AvaloniaProperty.Register<IconGlyph, IBrush?>(nameof(Fill));

        // Path data from the Material Design Icons set (https://pictogrammers.com/library/mdi/), 24x24 view box
        private static readonly Dictionary<string, string> _paths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["mdi-check"] = "M21,7L9,19L3.5,13.5L4.91,12.09L9,16.17L19.59,5.59L21,7Z",
            ["mdi-check-outline"] = "M19.78,2.2L24,6.42L8.44,22L0,13.55L4.22,9.33L8.44,13.55L19.78,2.2M19.78,5L8.44,16.36L4.22,12.19L2.81,13.55L8.44,19.17L21.19,6.42L19.78,5Z",
            ["mdi-close"] = "M19,6.41L17.59,5L12,10.59L6.41,5L5,6.41L10.59,12L5,17.59L6.41,19L12,13.41L17.59,19L19,17.59L13.41,12L19,6.41Z",
            ["mdi-cancel"] = "M12 2C17.5 2 22 6.5 22 12S17.5 22 12 22 2 17.5 2 12 6.5 2 12 2M12 4C10.1 4 8.4 4.6 7.1 5.7L18.3 16.9C19.3 15.5 20 13.8 20 12C20 7.6 16.4 4 12 4M16.9 18.3L5.7 7.1C4.6 8.4 4 10.1 4 12C4 16.4 7.6 20 12 20C13.9 20 15.6 19.4 16.9 18.3Z",
            ["mdi-content-copy"] = "M19,21H8V7H19M19,5H8A2,2 0 0,0 6,7V21A2,2 0 0,0 8,23H19A2,2 0 0,0 21,21V7A2,2 0 0,0 19,5M16,1H4A2,2 0 0,0 2,3V17H4V3H16V1Z",
            ["mdi-delete"] = "M19,4H15.5L14.5,3H9.5L8.5,4H5V6H19M6,19A2,2 0 0,0 8,21H16A2,2 0 0,0 18,19V7H6V19Z",
            ["mdi-download"] = "M5,20H19V18H5M19,9H15V3H9V9H5L12,16L19,9Z",
            ["mdi-export-variant"] = "M12,1L8,5H11V14H13V5H16M18,23H6C4.89,23 4,22.1 4,21V9A2,2 0 0,1 6,7H9V9H6V21H18V9H15V7H18A2,2 0 0,1 20,9V21A2,2 0 0,1 18,23Z",
            ["mdi-folder"] = "M10,4H4C2.89,4 2,4.89 2,6V18A2,2 0 0,0 4,20H20A2,2 0 0,0 22,18V8C22,6.89 21.1,6 20,6H12L10,4Z",
            ["mdi-import"] = "M14,12L10,8V11H2V13H10V16M20,18V6C20,4.89 19.1,4 18,4H6A2,2 0 0,0 4,6V9H6V6H18V18H6V15H4V18A2,2 0 0,0 6,20H18A2,2 0 0,0 20,18Z",
            ["mdi-minus"] = "M19,13H5V11H19V13Z",
            ["mdi-package-variant-closed"] = "M21,16.5C21,16.88 20.79,17.21 20.47,17.38L12.57,21.82C12.41,21.94 12.21,22 12,22C11.79,22 11.59,21.94 11.43,21.82L3.53,17.38C3.21,17.21 3,16.88 3,16.5V7.5C3,7.12 3.21,6.79 3.53,6.62L11.43,2.18C11.59,2.06 11.79,2 12,2C12.21,2 12.41,2.06 12.57,2.18L20.47,6.62C20.79,6.79 21,7.12 21,7.5V16.5M12,4.15L10.11,5.22L16,8.61L17.96,7.5L12,4.15M6.04,7.5L12,10.85L13.96,9.75L8.08,6.35L6.04,7.5M5,15.91L11,19.29V12.58L5,9.21V15.91M19,15.91V9.21L13,12.58V19.29L19,15.91Z",
            ["mdi-package-variant-closed-plus"] = "M13 19.3V12.6L19 9.2V13C19.7 13 20.4 13.1 21 13.4V7.5C21 7.1 20.8 6.8 20.5 6.6L12.6 2.2C12.4 2.1 12.2 2 12 2S11.6 2.1 11.4 2.2L3.5 6.6C3.2 6.8 3 7.1 3 7.5V16.5C3 16.9 3.2 17.2 3.5 17.4L11.4 21.8C11.6 21.9 11.8 22 12 22S12.4 21.9 12.6 21.8L13.5 21.3C13.2 20.7 13.1 20 13 19.3M12 4.2L18 7.5L16 8.6L10.1 5.2L12 4.2M11 19.3L5 15.9V9.2L11 12.6V19.3M12 10.8L6 7.5L8 6.3L14 9.8L12 10.8M20 15V18H23V20H20V23H18V20H15V18H18V15H20Z",
            ["mdi-pencil"] = "M20.71,7.04C21.1,6.65 21.1,6 20.71,5.63L18.37,3.29C18,2.9 17.35,2.9 16.96,3.29L15.12,5.12L18.87,8.87M3,17.25V21H6.75L17.81,9.93L14.06,6.18L3,17.25Z",
            ["mdi-playlist-edit"] = "M3 6V8H14V6H3M3 10V12H14V10H3M20 10.1C19.9 10.1 19.7 10.2 19.6 10.3L18.6 11.3L20.7 13.4L21.7 12.4C21.9 12.2 21.9 11.8 21.7 11.6L20.4 10.3C20.3 10.2 20.2 10.1 20 10.1M18.1 11.9L12 17.9V20H14.1L20.2 13.9L18.1 11.9M3 14V16H10V14H3Z",
            ["mdi-plus"] = "M19,13H13V19H11V13H5V11H11V5H13V11H19V13Z",
            ["mdi-refresh"] = "M17.65,6.35C16.2,4.9 14.21,4 12,4A8,8 0 0,0 4,12A8,8 0 0,0 12,20C15.73,20 18.84,17.45 19.73,14H17.65C16.83,16.33 14.61,18 12,18A6,6 0 0,1 6,12A6,6 0 0,1 12,6C13.66,6 15.14,6.69 16.22,7.78L13,11H20V4L17.65,6.35Z",
            ["mdi-thumb-up"] = "M23,10C23,8.89 22.1,8 21,8H14.68L15.64,3.43C15.66,3.33 15.67,3.22 15.67,3.11C15.67,2.7 15.5,2.32 15.23,2.05L14.17,1L7.59,7.58C7.22,7.95 7,8.45 7,9V19A2,2 0 0,0 9,21H18C18.83,21 19.54,20.5 19.84,19.78L22.86,12.73C22.95,12.5 23,12.26 23,12V10M1,21H5V9H1V21Z",
            ["mdi-thumb-up-outline"] = "M5,9V21H1V9H5M9,21A2,2 0 0,1 7,19V9C7,8.45 7.22,7.95 7.59,7.59L14.17,1L15.23,2.06C15.5,2.33 15.67,2.7 15.67,3.11L15.64,3.43L14.69,8H21C22.11,8 23,8.9 23,10V12C23,12.26 22.95,12.5 22.86,12.73L19.84,19.78C19.54,20.5 18.83,21 18,21H9M9,19H18.03L21,12V10H12.21L13.34,4.68L9,9.03V19Z",
            ["mdi-window-maximize"] = "M4,4H20V20H4V4M6,8V18H18V8H6Z",
            ["mdi-window-minimize"] = "M20,14H4V10H20"
        };

        private static readonly Dictionary<string, Geometry> _geometries = new Dictionary<string, Geometry>(StringComparer.OrdinalIgnoreCase);

        static IconGlyph()
        {
            AffectsRender<IconGlyph>(ValueProperty, FillProperty);
        }

        public IconGlyph()
        {
            Width = 20;
            Height = 20;
        }

        public string? Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public IBrush? Fill
        {
            get => GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            var geometry = GetGeometry(Value);
            if (geometry is null || Bounds.Width <= 0 || Bounds.Height <= 0)
            {
                return;
            }

            var bounds = geometry.Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            var scale = Math.Min(Bounds.Width / bounds.Width, Bounds.Height / bounds.Height);
            var offsetX = ((Bounds.Width - (bounds.Width * scale)) / 2) - (bounds.X * scale);
            var offsetY = ((Bounds.Height - (bounds.Height * scale)) / 2) - (bounds.Y * scale);

            using (context.PushTransform(Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(offsetX, offsetY)))
            {
                context.DrawGeometry(Fill ?? Brushes.Black, null, geometry);
            }
        }

        private static Geometry? GetGeometry(string? value)
        {
            if (String.IsNullOrEmpty(value) || _paths.TryGetValue(value, out var pathData) is false)
            {
                return null;
            }

            if (_geometries.TryGetValue(value, out var geometry) is false)
            {
                geometry = Geometry.Parse(pathData);
                _geometries[value] = geometry;
            }

            return geometry;
        }
    }

    /// <summary>
    /// Attached properties matching the shapes the Projektanker package exposed (i:Attached.Icon and
    /// i:MenuItem.Icon), so the views only need a different xmlns.
    /// </summary>
    public sealed class Attached
    {
        private Attached()
        {
        }

        public static readonly AttachedProperty<string?> IconProperty =
            AvaloniaProperty.RegisterAttached<Attached, Control, string?>("Icon");

        static Attached()
        {
            IconProperty.Changed.AddClassHandler<Control>(OnIconChanged);
        }

        public static string? GetIcon(Control control) => control.GetValue(IconProperty);

        public static void SetIcon(Control control, string? value) => control.SetValue(IconProperty, value);

        private static void OnIconChanged(Control control, AvaloniaPropertyChangedEventArgs e)
        {
            var iconName = e.NewValue as string;

            // The button is expected to carry the icon as its content, which is how the previous package behaved
            if (control is ContentControl contentControl)
            {
                if (String.IsNullOrEmpty(iconName))
                {
                    if (contentControl.Content is IconGlyph)
                    {
                        contentControl.Content = null;
                    }

                    return;
                }

                contentControl.Content = CreateGlyph(contentControl, iconName);
            }
        }

        internal static IconGlyph CreateGlyph(Control host, string iconName)
        {
            var glyph = new IconGlyph { Value = iconName };

            // Icons follow the foreground of the control that hosts them
            glyph.Bind(IconGlyph.FillProperty, new Binding("Foreground") { Source = host });

            return glyph;
        }
    }

    public sealed class MenuItem
    {
        private MenuItem()
        {
        }

        public static readonly AttachedProperty<string?> IconProperty =
            AvaloniaProperty.RegisterAttached<MenuItem, Control, string?>("Icon");

        static MenuItem()
        {
            IconProperty.Changed.AddClassHandler<Control>(OnIconChanged);
        }

        public static string? GetIcon(Control control) => control.GetValue(IconProperty);

        public static void SetIcon(Control control, string? value) => control.SetValue(IconProperty, value);

        private static void OnIconChanged(Control control, AvaloniaPropertyChangedEventArgs e)
        {
            if (control is not Avalonia.Controls.MenuItem menuItem)
            {
                return;
            }

            var iconName = e.NewValue as string;
            if (String.IsNullOrEmpty(iconName))
            {
                if (menuItem.Icon is IconGlyph)
                {
                    menuItem.Icon = null;
                }

                return;
            }

            menuItem.Icon = Attached.CreateGlyph(menuItem, iconName);
        }
    }
}
