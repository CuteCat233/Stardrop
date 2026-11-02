using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stardrop.Utilities
{
    /// <summary>
    /// Thin wrappers over Avalonia's storage provider, which replaced the per-platform dialog classes
    /// (OpenFileDialog, SaveFileDialog, OpenFolderDialog) in Avalonia 11.
    /// </summary>
    internal static class FilePickers
    {
        public static async Task<IReadOnlyList<string>> OpenFilesAsync(Visual owner, string title, bool allowMultiple, params FilePickerFileType[] filters)
        {
            var storageProvider = GetStorageProvider(owner);
            if (storageProvider is null)
            {
                return Array.Empty<string>();
            }

            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = allowMultiple,
                FileTypeFilter = filters
            });

            return ToLocalPaths(files);
        }

        public static async Task<string?> OpenFileAsync(Visual owner, string title, params FilePickerFileType[] filters)
        {
            return (await OpenFilesAsync(owner, title, false, filters)).FirstOrDefault();
        }

        public static async Task<string?> OpenFolderAsync(Visual owner, string title, string? startingPath = null)
        {
            var storageProvider = GetStorageProvider(owner);
            if (storageProvider is null)
            {
                return null;
            }

            IStorageFolder? suggestedStartLocation = null;
            if (String.IsNullOrEmpty(startingPath) is false && System.IO.Directory.Exists(startingPath))
            {
                suggestedStartLocation = await storageProvider.TryGetFolderFromPathAsync(startingPath);
            }

            var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                SuggestedStartLocation = suggestedStartLocation
            });

            return ToLocalPaths(folders).FirstOrDefault();
        }

        public static async Task<string?> SaveFileAsync(Visual owner, string title, string suggestedFileName, FilePickerFileType filter)
        {
            var storageProvider = GetStorageProvider(owner);
            if (storageProvider is null)
            {
                return null;
            }

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = title,
                SuggestedFileName = suggestedFileName,
                FileTypeChoices = new[] { filter },
                ShowOverwritePrompt = true
            });

            return file?.TryGetLocalPath();
        }

        private static IStorageProvider? GetStorageProvider(Visual owner)
        {
            return TopLevel.GetTopLevel(owner)?.StorageProvider;
        }

        private static IReadOnlyList<string> ToLocalPaths(IEnumerable<IStorageItem> items)
        {
            return items.Select(item => item.TryGetLocalPath())
                        .Where(path => String.IsNullOrEmpty(path) is false)
                        .Cast<string>()
                        .ToList();
        }
    }
}
