using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Stardrop.Models.Nexus.Web;
using Stardrop.Utilities;
using Stardrop.Utilities.Internal;
using Stardrop.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stardrop
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            // Verify that the helper is instantiated, if it isn't then this code is likely reached by Avalonia's previewer and bypassed Main
            if (Program.helper is null)
            {
                Program.helper = new Helper();
            }

            // Load in translations
            Program.translation.LoadTranslations();

            // Handle adding the themes
            Dictionary<string, IStyle> themes = new Dictionary<string, IStyle>();
            foreach (string fileFullName in ThemeManager.GetThemeFilePaths())
            {
                try
                {
                    var themeName = Path.GetFileNameWithoutExtension(fileFullName);
                    themes[themeName] = ThemeManager.Load(fileFullName);
                    Program.helper.Log($"Loaded theme {Path.GetFileNameWithoutExtension(fileFullName)}", Helper.Status.Debug);
                }
                catch (Exception ex)
                {
                    Program.helper.Log($"Unable to load theme on {Path.GetFileNameWithoutExtension(fileFullName)}: {ex}", Helper.Status.Warning);
                }
            }

            Current.Styles.Insert(ThemeManager.THEME_STYLE_INDEX, !themes.ContainsKey(Program.settings.Theme) ? themes.Values.First() : themes[Program.settings.Theme]);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();
                desktop.MainWindow = mainWindow;

                // Avalonia 12 replaced Application.UrlsOpened with the activatable application lifetime,
                // which reports protocol activations (such as nxm:// links) as ProtocolActivatedEventArgs.
                if (this.TryGetFeature<IActivatableLifetime>() is { } activatableLifetime)
                {
                    activatableLifetime.Activated += (sender, e) =>
                    {
                        if (e is ProtocolActivatedEventArgs protocolActivatedEventArgs && String.Equals(protocolActivatedEventArgs.Uri.Scheme, "nxm", StringComparison.OrdinalIgnoreCase))
                        {
                            _ = OpenNxmLinkAsync(mainWindow, protocolActivatedEventArgs.Uri.AbsoluteUri);
                        }
                    };
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static async Task OpenNxmLinkAsync(MainWindow mainWindow, string url)
        {
            try
            {
                await mainWindow.ProcessNXMLink(new NXM() { Link = url, Timestamp = DateTime.Now });
            }
            catch (Exception ex)
            {
                Program.helper.Log($"Failed to process the opened NXM link [{url}]: {ex}", Helper.Status.Warning);
            }
        }
    }
}
