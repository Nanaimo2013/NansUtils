using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using NansUtils.Utils;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace NansUtils.Commands
{
    public class UpdateCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "update";
        public string Help => "Checks for updates to the NansUtils plugin.";
        public string Syntax => "/update [help|check|confirm]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "nansutils.update" };

        private static string LatestVersion { get; set; }
        private static bool UpdateAvailable { get; set; }
        private const string GitHubApiUrl = "https://api.github.com/repos/Nanaimo2013/NansUtils/releases/latest";
        private const string UserAgent = "NansUtilsUpdater";

        public async void Execute(IRocketPlayer caller, string[] command)
        {
            // Set invariant culture to avoid culture-related issues
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (command.Length == 0)
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Usage: /update [help|check|confirm]", Color.yellow);
                return;
            }

            UnturnedPlayer player = (UnturnedPlayer)caller;

            switch (command[0].ToLower())
            {
                case "help":
                    ChatUtils.SendMessage(player, "Update Command Help: /update [help|check|confirm]", Color.green);
                    break;
                case "check":
                    await CheckForUpdates(player);
                    break;
                case "confirm":
                    if (UpdateAvailable)
                    {
                        await UpdatePlugin(player);
                    }
                    else
                    {
                        ChatUtils.SendMessage(player, "No update available to confirm.", Color.red);
                    }
                    break;
                default:
                    ChatUtils.SendMessage(player, "Invalid option. Use /update help for more information.", Color.red);
                    break;
            }
        }

        private async Task CheckForUpdates(UnturnedPlayer player)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                    string json = await client.GetStringAsync(GitHubApiUrl);
                    JObject release = JObject.Parse(json);
                    string latestVersion = release["tag_name"].ToString().Trim();

                    if (latestVersion != NansUtilsPlugin.CurrentVersion)
                    {
                        UpdateAvailable = true;
                        LatestVersion = latestVersion;
                        ChatUtils.SendMessage(player, $"A new version {LatestVersion} is available. Current version: {NansUtilsPlugin.CurrentVersion}. Use /update confirm to update.", Color.yellow);
                    }
                    else
                    {
                        UpdateAvailable = false;
                        ChatUtils.SendMessage(player, "You are already using the latest version.", Color.green);
                    }
                }
            }
            catch (Exception ex)
            {
                ChatUtils.SendMessage(player, $"Failed to check for updates: {ex.Message}", Color.red);
                Rocket.Core.Logging.Logger.LogError($"Update check error: {ex}");
            }
        }

        private async Task UpdatePlugin(UnturnedPlayer player)
        {
            try
            {
                ChatUtils.SendMessage(player, "Downloading update...", Color.green);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                    string json = await client.GetStringAsync(GitHubApiUrl);
                    JObject release = JObject.Parse(json);
                    string downloadUrl = release["assets"].First["browser_download_url"].ToString();

                    // Create a backup of the current plugin
                    string pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "NansUtils.dll");
                    string backupPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "NansUtils.dll.backup");
                    if (File.Exists(pluginPath))
                    {
                        if (File.Exists(backupPath))
                        {
                            File.Delete(backupPath);
                        }
                        File.Copy(pluginPath, backupPath);
                    }

                    // Download and save the new version
                    byte[] data = await client.GetByteArrayAsync(downloadUrl);
                    File.WriteAllBytes(pluginPath, data);

                    // Update the current version
                    NansUtilsPlugin.CurrentVersion = LatestVersion;

                    ChatUtils.SendMessage(player, $"Plugin updated successfully to version {LatestVersion}", Color.green);
                    ChatUtils.SendMessage(player, "Please restart the server to apply the update.", Color.yellow);
                }
            }
            catch (Exception ex)
            {
                ChatUtils.SendMessage(player, $"Failed to update plugin: {ex.Message}", Color.red);
                Rocket.Core.Logging.Logger.LogError($"Update error: {ex}");

                // Try to restore backup if update failed
                string pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "NansUtils.dll");
                string backupPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "NansUtils.dll.backup");
                if (File.Exists(backupPath))
                {
                    try
                    {
                        File.Copy(backupPath, pluginPath, true);
                        ChatUtils.SendMessage(player, "Restored previous version from backup.", Color.yellow);
                    }
                    catch (Exception backupEx)
                    {
                        Rocket.Core.Logging.Logger.LogError($"Backup restoration error: {backupEx}");
                    }
                }
            }
        }
    }
} 