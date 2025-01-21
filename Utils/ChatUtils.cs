using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.Core.Logging;
using SDG.Unturned;
using UnityEngine;

namespace NansUtils.Utils
{
    public static class ChatUtils
    {
        public static void SendMessage(UnturnedPlayer player, string message, Color color)
        {
            string formattedMessage = FormatMessage(message);
            UnturnedChat.Say(player, formattedMessage, color);
            Rocket.Core.Logging.Logger.Log($"Message sent to {player.CharacterName}: {formattedMessage}");
        }

        private static string FormatMessage(string message)
        {
            return $"[NansUtils] {message}";
        }

        public static void BroadcastMessage(string message, Color color)
        {
            UnturnedChat.Say(message, color);
            Rocket.Core.Logging.Logger.Log($"Broadcast message: {message}");
        }
    }
}