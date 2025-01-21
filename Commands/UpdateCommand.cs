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
            if (command.Length == 0)
            {
                ChatUtils.SendMessage((UnturnedPlayer)caller, "Usage: /update [help|check|confirm]", UnityEngine.Color.yellow);
                return;
            }

            UnturnedPlayer player = (UnturnedPlayer)caller;

            switch (command[0].ToLower())
            {
                case "help":
                    ChatUtils.SendMessage(player, "Update Command Help: /update [help|check|confirm]", UnityEngine.Color.green);
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
                        ChatUtils.SendMessage(player, "No update available to confirm.", UnityEngine.Color.red);
                    }
                    break;
                default:
                    ChatUtils.SendMessage(player, "Invalid option. Use /update help for more information.", UnityEngine.Color.red);
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
                        ChatUtils.SendMessage(player, $"A new version {LatestVersion} is available. Current version: {NansUtilsPlugin.CurrentVersion}. Use /update confirm to update.", UnityEngine.Color.yellow);
                    }
                    else
                    {
                        UpdateAvailable = false;
                        ChatUtils.SendMessage(player, "You are already using the latest version.", UnityEngine.Color.green);
                    }
                }
            }
            catch (Exception ex)
            {
                ChatUtils.SendMessage(player, $"Failed to check for updates: {ex.Message}", UnityEngine.Color.red);
            }
        }

        private async Task UpdatePlugin(UnturnedPlayer player)
        {
            try
            {
                ChatUtils.SendMessage(player, "Downloading update...", UnityEngine.Color.green);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
                    string json = await client.GetStringAsync(GitHubApiUrl);
                    JObject release = JObject.Parse(json);
                    string downloadUrl = release["assets"].First["browser_download_url"].ToString();

                    byte[] data = await client.GetByteArrayAsync(downloadUrl);
                    string pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "NansUtils.dll");

                    File.WriteAllBytes(pluginPath, data);
                }

                ChatUtils.SendMessage(player, "Plugin updated successfully to version " + LatestVersion, UnityEngine.Color.green);
            }
            catch (Exception ex)
            {
                ChatUtils.SendMessage(player, $"Failed to update plugin: {ex.Message}", UnityEngine.Color.red);
            }
        }
    }
} 