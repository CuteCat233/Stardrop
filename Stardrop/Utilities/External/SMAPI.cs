﻿using Stardrop.Models;
using Stardrop.Models.SMAPI;
using Stardrop.Models.SMAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Stardrop.Utilities.External
{
    static class SMAPI
    {
        internal static bool IsRunning = false;
        internal static Process Process;

        public static ProcessStartInfo GetPrepareProcess(bool hideConsole)
        {
            var arguments = String.Empty;
            var smapiInfo = new FileInfo(Pathing.GetSmapiPath());
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                arguments = $"-c \"open -n -a Terminal.app --args SMAPI_MODS_PATH='{Pathing.GetSelectedModsFolderPath()}' '{Pathing.GetSmapiPath().Replace("StardewModdingAPI", "StardewValley")}'\"";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                arguments = $"\"{Pathing.GetSmapiPath().Replace("StardewModdingAPI", "StardewValley")}\"";
            }

            var processInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? smapiInfo.FullName : "/bin/bash",
                Arguments = arguments,
                WorkingDirectory = smapiInfo.DirectoryName,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = hideConsole,
                UseShellExecute = false
            };
            processInfo.EnvironmentVariables["SMAPI_MODS_PATH"] = Pathing.GetSelectedModsFolderPath();

            return processInfo;
        }

        public static string GetProcessName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "StardewModdingA";
            }

            return "StardewModdingAPI";
        }

        public async static Task<List<ModEntry>> GetModUpdateData(GameDetails gameDetails, List<Mod> mods)
        {
            List<ModSearchEntry> searchEntries = new List<ModSearchEntry>();
            foreach (var mod in mods)
            {
                searchEntries.Add(new ModSearchEntry(mod.UniqueId, mod.Version, mod.Manifest.UpdateKeys));
            }
            foreach (var requirementKey in mods.SelectMany(m => m.Requirements))
            {
                searchEntries.Add(new ModSearchEntry() { Id = requirementKey.UniqueID });
            }

            // Create the body to be sent via the POST request
            ModSearchData searchData = new ModSearchData(searchEntries, gameDetails.SmapiVersion, gameDetails.GameVersion, gameDetails.System.ToString(), true);

            // Create a throwaway client
            HttpClient client = new HttpClient();
            var requestPackage = new StringContent(JsonSerializer.Serialize(searchData), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://smapi.io/api/v3.0/mods", requestPackage);

            List<ModEntry> modUpdateData = new List<ModEntry>();
            if (response.Content is not null)
            {
                // In the name of the Nine Divines, why is JsonSerializer.Deserialize case sensitive by default???

                modUpdateData = JsonSerializer.Deserialize<List<ModEntry>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            client.Dispose();

            return modUpdateData;
        }
    }
}
