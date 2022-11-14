using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Groups;
using HarmonyLib;
using UnityEngine;

namespace GroupsGUI
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("org.bepinex.plugins.groups", BepInDependency.DependencyFlags.SoftDependency)]
    public class GroupsGUI : BaseUnityPlugin
    {
        private const string PluginGuid = "com.comoyi.valheim.GroupsGUI";
        private const string PluginName = "GroupsGUI";
        private const string PluginVersion = "1.0.1";

        private readonly Harmony harmony = new Harmony(PluginGuid);

        private static ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("GroupsGUI");

        private ConfigEntry<int> configPositionX;
        private ConfigEntry<int> configPositionY;
        private static Texture2D groupIcon;

        private static bool isGameStarted = false;
        private bool isOpen = false;

        private void Awake()
        {
            configPositionX = Config.Bind("UI", "X", 50, "Position X");
            configPositionY = Config.Bind("UI", "Y", 300, "Position Y");

            groupIcon = GetGroupIcon();

            harmony.PatchAll();
            Log.LogInfo($"Plugin {PluginName} loaded");
        }

        private void OnGUI()
        {
            if (!isGameStarted)
            {
                return;
            }

            int x = configPositionX.Value;
            int y = configPositionY.Value;
            int w = 70;
            int h = 30;
            int offsetY = 35;
            if (GUI.Button(new Rect(x, y, w, h), "队伍"))
            {
                isOpen = !isOpen;
            }

            y += offsetY;
            if (!isOpen)
            {
                return;
            }

            if (GUI.Button(new Rect(x, y, w, h), "创建队伍"))
            {
                Groups.API.CreateNewGroup();
                Log.LogInfo("Create New Group");
            }

            x += 70;

            if (GUI.Button(new Rect(x, y, w, h), "离开队伍"))
            {
                Groups.API.LeaveGroup();
                Log.LogInfo("Leave Group");
            }

            y += offsetY;
            
            List<PlayerReference> groupPlayers = Groups.API.GroupPlayers();
            foreach (var groupPlayer in groupPlayers)
            {
                x = configPositionX.Value;

                string playerName = groupPlayer.name;
                long targetId = groupPlayer.peerId;
                if (targetId == 0)
                {
                    continue;
                }

                GUI.Label(new Rect(x, y, 100, h), groupPlayer.name);
                x += 100;

                if (GUI.Button(new Rect(x, y, 70, h), "成为队长"))
                {
                    Groups.API.PromoteToLeader(Groups.PlayerReference.fromPlayerId(targetId));
                    Log.LogInfo($"Promote to leader, playerName: {playerName}, targetId: {targetId}");
                }

                y += offsetY;
            }

            List<ZNet.PlayerInfo> players = ZNet.instance.GetPlayerList();
            foreach (var player in players)
            {
                x = configPositionX.Value;
                string playerName = player.m_name;
                long targetId = ZNet.instance.GetPlayerList().FirstOrDefault(p => string.Compare(playerName, p.m_name, StringComparison.OrdinalIgnoreCase) == 0).m_characterID.userID;
                if (targetId == 0)
                {
                    continue;
                }

                GUI.Label(new Rect(x, y, w, h), playerName);
                x += 70;

                if (GUI.Button(new Rect(x, y, 50, h), "加入"))
                {
                    Groups.API.JoinGroup(Groups.PlayerReference.fromPlayerId(targetId));
                    Log.LogInfo($"Join Group, playerName: {playerName}, targetId: {targetId}");
                }

                x += 70;

                if (GUI.Button(new Rect(x, y, 50, h), "邀请"))
                {
                    Groups.API.ForcePlayerIntoOwnGroup(Groups.PlayerReference.fromPlayerId(targetId));
                    Log.LogInfo($"Force Invite to Group, playerName: {playerName}, targetId: {targetId}");
                }

                y += offsetY;
            }
        }

        [HarmonyPatch(typeof(Game), "Start")]
        private class CheckGameStart
        {
            [HarmonyPostfix]
            static void Postfix()
            {
                isGameStarted = true;
            }
        }

        private Texture2D GetGroupIcon()
        {
            string relativeFilePath = "BepInEx\\plugins\\GroupsGUI\\group.png";
            Texture2D tex = LoadTexture(relativeFilePath);
            if (tex == null)
            {
                tex = GetDefaultGroupIcon();
                Log.LogInfo("Loaded default group icon");
            }
            else
            {
                Log.LogInfo($"Loaded sleep icon from {relativeFilePath}");
            }

            return tex;
        }

        private Texture2D GetDefaultGroupIcon()
        {
            Texture2D tex = new Texture2D(10, 10);
            byte[] pngBytes = new byte[]
            {
            };
            tex.LoadImage(pngBytes);
            return tex;
        }

        private static Texture2D LoadTexture(string relativeFilePath)
        {
            string filePath = System.IO.Path.GetFullPath(relativeFilePath);
            Texture2D tex2D;
            byte[] fileData;

            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    fileData = System.IO.File.ReadAllBytes(filePath);
                }
                catch (Exception e)
                {
                    Log.LogInfo("" + e.Message);
                    return null;
                }

                tex2D = new Texture2D(10, 10);
                if (tex2D.LoadImage(fileData))
                {
                    return tex2D;
                }
            }

            return null;
        }
    }
}