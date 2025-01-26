using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System;

namespace NansUtils.Commands
{
    public class UpdateCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "update";
        public string Help => "Check for and apply plugin updates.";
        public string Syntax => "/update [check|apply]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.update" };

        private const string GITHUB_API_URL = "https://api.github.com/repos/Nanotech2023/NansUtils/releases/latest";
        private static readonly HttpClient client = new HttpClient();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller is UnturnedPlayer player)
            {
                if (command.Length == 0 || command[0].ToLower() == "check")
                {
                    CheckForUpdates(player);
                }
                else if (command[0].ToLower() == "apply")
                {
                    ApplyUpdate(player);
                }
                else
                {
                    ChatUtils.SendMessage(player, "Usage: /update [check|apply]", Color.yellow);
                }
            }
        }

        private async void CheckForUpdates(UnturnedPlayer player)
        {
            try
            {
                ChatUtils.SendMessage(player, "Checking for updates...", Color.yellow);

                // Get current version
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                string currentVersionStr = $"{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}";

                // Get latest version from GitHub
                client.DefaultRequestHeaders.UserAgent.ParseAdd("NansUtils");
                var response = await client.GetStringAsync(GITHUB_API_URL);
                var json = JObject.Parse(response);
                string latestVersion = json["tag_name"].ToString().TrimStart('v');

                // Parse versions for comparison
                Version latest = Version.Parse(latestVersion);
                
                if (latest > currentVersion)
                {
                    ChatUtils.SendMessage(player, $"New version available: v{latestVersion}", Color.green);
                    ChatUtils.SendMessage(player, "Use /update apply to update the plugin", Color.green);
                }
                else if (latest == currentVersion)
                {
                    ChatUtils.SendMessage(player, $"You are running the latest version (v{currentVersionStr})", Color.green);
                }
                else
                {
                    ChatUtils.SendMessage(player, $"You are running a newer version (v{currentVersionStr}) than the latest release (v{latestVersion})", Color.yellow);
                }
            }
            catch (Exception ex)
            {
                ChatUtils.SendMessage(player, "Failed to check for updates. Check logs for details.", Color.red);
                Rocket.Core.Logging.Logger.LogError($"Update check failed: {ex.Message}");
            }
        }

        private async void ApplyUpdate(UnturnedPlayer player)
        {
            try
            {
                ChatUtils.SendMessage(player, "Checking for updates...", Color.yellow);

                // Get current version
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                string currentVersionStr = $"{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}";

                // Get latest version info from GitHub
                client.DefaultRequestHeaders.UserAgent.ParseAdd("NansUtils");
                var response = await client.GetStringAsync(GITHUB_API_URL);
                var json = JObject.Parse(response);
                string latestVersion = json["tag_name"].ToString().TrimStart('v');
                string downloadUrl = json["assets"][0]["browser_download_url"].ToString();

                // Parse versions for comparison
                Version latest = Version.Parse(latestVersion);

                if (latest <= currentVersion)
                {
                    ChatUtils.SendMessage(player, $"You are already running the latest version (v{currentVersionStr})", Color.green);
                    return;
                }

                // Download and apply update
                ChatUtils.SendMessage(player, $"Downloading update v{latestVersion}...", Color.yellow);
                
                string pluginDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string backupPath = Path.Combine(pluginDir, "NansUtils.dll.bak");
                string currentPath = Path.Combine(pluginDir, "NansUtils.dll");
                string downloadPath = Path.Combine(pluginDir, "NansUtils.dll.new");

                // Download new version
                byte[] newVersion = await client.GetByteArrayAsync(downloadUrl);
                File.WriteAllBytes(downloadPath, newVersion);

                // Backup current version
                if (File.Exists(currentPath))
                {
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);
                    File.Move(currentPath, backupPath);
                }

                // Apply new version
                File.Move(downloadPath, currentPath);

                ChatUtils.SendMessage(player, $"Update downloaded successfully. Restart the server to apply the update.", Color.green);
            }
            catch (Exception ex)
            {
                ChatUtils.SendMessage(player, "Failed to apply update. Check logs for details.", Color.red);
                Rocket.Core.Logging.Logger.LogError($"Update failed: {ex.Message}");
            }
        }
    }
} 