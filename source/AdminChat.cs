namespace AdminChat;

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
[MinimumApiVersion(71)]
public class AdminChat : BasePlugin
{
    public override string ModuleName => "AdminChat";
    public override string ModuleVersion => "0.1";
    public override string ModuleAuthor => "Johnoclock, WidovV";
    public override string ModuleDescription => "Admin chat";

    public bool[] g_btoggleasay = new bool[Server.MaxPlayers + 1];

    public override void Load(bool hotReload)
    {
        AddCommandListener("say", OnPlayerChat);
        AddCommandListener("say_team", OnPlayerChatTeam);

        RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnect);

        new Cfg().CheckConfig(ModuleDirectory);

        Console.WriteLine("AdminChat loaded");
    }


    public void OnClientDisconnect(int playerSlot)
    {
        CCSPlayerController player = Utilities.GetPlayerFromSlot(playerSlot);

        if (player.IsBot || !player.IsValid)
        {
            return;
        }

        g_btoggleasay[playerSlot] = false;
    }

    [ConsoleCommand("css_asay", "adminsay command")]
    [ConsoleCommand("css_adminsay", "adminsay command")]
    [RequiresPermissions("@css/chat")]
    public void OnCommandAsay(CCSPlayerController? player, CommandInfo command)
    {
        if (player is null)
        {
            if (command.ArgCount < 2)
            {
                Console.WriteLine("Usage: css_adminsay <message>");
                return;
            }

            foreach (CCSPlayerController currentPlayer in Utilities.GetPlayers())
            {
                if (!ValidClient(currentPlayer)) continue;

                currentPlayer.PrintToChat($"{Cfg.Config.ChatPrefix} {Cfg.Config.ChatHighlight}Console{Cfg.Config.ChatText}: {GetArgs(command, 1)}");
            }
            return;
        }

        if (!ValidClient(player)) return;

        if (command.ArgCount < 2)
        {
            player.PrintToChat("Usage: css_adminsay <message>");
            return;
        }

        List<CCSPlayerController> players = Utilities.GetPlayers();

        foreach (CCSPlayerController currentPlayer in players)
        {
            if (!ValidClient(player)) continue;

            if (AdminManager.PlayerHasPermissions(currentPlayer, "@css/chat"))
            {
                currentPlayer.PrintToChat($"{Cfg.Config.ChatPrefix} {Cfg.Config.ChatHighlight}{player.PlayerName}{Cfg.Config.ChatText}: {GetArgs(command, 1)}");
            }
        }
        Console.WriteLine($"{Cfg.Config.ChatPrefix} {Cfg.Config.ChatHighlight}{player.PlayerName}{Cfg.Config.ChatText}: {GetArgs(command, 1)}");
    }

    [ConsoleCommand("css_toggleadminsay", "adminsay toggle admin chat command")]
    [ConsoleCommand("css_tasay", "adminsay toggle admin chat command")]
    [RequiresPermissions("@css/chat")]
    public void OnCommandTAsay(CCSPlayerController? player, CommandInfo command)
    {
        if (!ValidClient(player!)) return;

        if (!g_btoggleasay[player.Slot])
        {
            player.PrintToChat($"{Cfg.Config.ChatPrefix} Admin chat is now toggle on");
            g_btoggleasay[player.Slot] = true;

        }
        else
        {
            player.PrintToChat($"{Cfg.Config.ChatPrefix} Admin chat is now toggle off");
            g_btoggleasay[player.Slot] = false;
        }
    }

    private HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info)
    {
        if (player is null)
        {
            foreach (CCSPlayerController currentPlayer in Utilities.GetPlayers())
            {
                if (!ValidClient(currentPlayer)) continue;

                currentPlayer.PrintToChat($"{Cfg.Config.ChatPrefix} {Cfg.Config.ChatHighlight}Console{Cfg.Config.ChatText}: {GetArgs(info, 1)}");
            }
            return HookResult.Handled;
        }

        if (!g_btoggleasay[player.Slot] == true || !player.IsValid) return HookResult.Continue;

        char firstChar = info.GetArg(1)[0];
        if (firstChar == '!' || firstChar == '/')
        {
            return HookResult.Continue;
        }
        else
        {
            List<CCSPlayerController> players = Utilities.GetPlayers();

            foreach (CCSPlayerController currentPlayer in players)
            {
                if (!AdminManager.PlayerHasPermissions(currentPlayer, "@css/chat"))
                {
                    continue;
                }

                currentPlayer.PrintToChat($"{Cfg.Config.ChatPrefix} {Cfg.Config.ChatHighlight}{player.PlayerName}{Cfg.Config.ChatText}: {GetArgs(info, 1)}");
            }
            Console.WriteLine($"{Cfg.Config.ChatPrefix} {Cfg.Config.ChatHighlight}{player.PlayerName}{Cfg.Config.ChatText}: {GetArgs(info, 1)}");
            return HookResult.Handled;
        }
    }


    private HookResult OnPlayerChatTeam(CCSPlayerController? player, CommandInfo info)
    {
        if (player is null)
        {
            foreach (CCSPlayerController currentPlayer in Utilities.GetPlayers())
            {
                if (!ValidClient(currentPlayer)) continue;

                currentPlayer.PrintToChat($"{Cfg.Config.ChatPrefix} {Cfg.Config.ChatHighlight}Console{Cfg.Config.ChatText}: {GetArgs(info, 1)}");
            }
            return HookResult.Handled;
        }

        if (!g_btoggleasay[player.Slot] == true || !player.IsValid) return HookResult.Continue;

        char firstChar = info.GetArg(1)[0];
        if (firstChar == '!' || firstChar == '/')
        {
            return HookResult.Continue;
        }
        else
        {
            List<CCSPlayerController> players = Utilities.GetPlayers();

            foreach (CCSPlayerController currentPlayer in players)
            {
                if (!AdminManager.PlayerHasPermissions(currentPlayer, "@css/chat"))
                {
                    continue;
                }

                currentPlayer.PrintToChat($"{Cfg.Config.ChatPrefix} {Cfg.Config.ChatHighlight}{player.PlayerName}{Cfg.Config.ChatText}: {GetArgs(info, 1)}");
            }
            Console.WriteLine($"{Cfg.Config.ChatPrefix} {Cfg.Config.ChatHighlight}{player.PlayerName}{Cfg.Config.ChatText}: {GetArgs(info, 1)}");
            return HookResult.Handled;
        }
    }

    private bool ValidClient(CCSPlayerController player)
    {
        if (!player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsHLTV || !player.PlayerPawn.IsValid || player.UserId == -1 || player.IsBot) return false;
        return true;
    }


    private string GetArgs(CommandInfo info, int start, int end = -1)
    {
        // Get the number of arguments to copy
        int stop = end == -1 ? info.ArgCount : end;

        // Create a new array to store the arguments
        string[] args = new string[stop - start];

        // Loop through the arguments
        for (int i = start; i < stop; i++)
        {
            // Copy the argument to the new array
            args[i - start] = info.GetArg(i);
        }
        // Join the arguments into a string and return it
        return string.Join(" ", args);
    }
}