using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;

namespace NansUtils.Utils
{
    public static class ChatUtils
    {
        public static void SendMessage(UnturnedPlayer player, string message, Color color)
        {
            UnturnedChat.Say(player, message, color);
        }
    }
} 