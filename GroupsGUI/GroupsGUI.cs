using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
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

        private bool isOpen = false;

        private void Awake()
        {
            harmony.PatchAll();
            Log.LogInfo($"Plugin {PluginName} loaded");
        }

        private void OnGUI()
        {
            int x = 0;
            int y = 0;
            if (GUI.Button(new Rect(x, y, 70, 20), "队伍"))
            {
                isOpen = !isOpen;
                Log.LogInfo("clicked Group Button, Toggle");
            }

            y += 20;
            if (!isOpen)
            {
                return;
            }

            if (GUI.Button(new Rect(x, y, 70, 20), "创建队伍"))
            {
                Groups.API.CreateNewGroup();
                Log.LogInfo("clicked Group Button, Create New Group");
            }

            y += 20;

            if (GUI.Button(new Rect(x, y, 70, 20), "离开队伍"))
            {
                Groups.API.LeaveGroup();
                Log.LogInfo("clicked Group Button, Leave Group");
            }

            y += 20;

            List<ZNet.PlayerInfo> players = ZNet.instance.GetPlayerList();
            foreach (var player in players)
            {
                x = 0;
                string playerName = player.m_name;
                long targetId = ZNet.instance.GetPlayerList().FirstOrDefault(p => string.Compare(playerName, p.m_name, StringComparison.OrdinalIgnoreCase) == 0).m_characterID.userID;
                if (targetId == 0)
                {
                    continue;
                }

                GUI.Label(new Rect(x, y, 70, 20), playerName);
                x += 70;

                if (GUI.Button(new Rect(x, y, 70, 20), "申请加入"))
                {
                    Groups.API.JoinGroup(Groups.PlayerReference.fromPlayerId(targetId));
                    Log.LogInfo($"playerName: {playerName}, targetId: {targetId}");
                    Log.LogInfo("clicked Group Button, Join Group");
                }

                x += 70;

                if (GUI.Button(new Rect(x, y, 70, 20), "强制邀请"))
                {
                    Groups.API.ForcePlayerIntoOwnGroup(Groups.PlayerReference.fromPlayerId(targetId));
                    Log.LogInfo($"playerName: {playerName}, targetId: {targetId}");
                    Log.LogInfo("clicked Group Button, Force Invite to Group");
                }

                x += 70;

                if (GUI.Button(new Rect(x, y, 70, 20), "成为队长"))
                {
                    Groups.API.PromoteToLeader(Groups.PlayerReference.fromPlayerId(targetId));
                    Log.LogInfo($"playerName: {playerName}, targetId: {targetId}");
                    Log.LogInfo("clicked Group Button, Promote to leader");
                }

                y += 20;
            }

            List<PlayerReference> groupPlayers = Groups.API.GroupPlayers();
            foreach (var groupPlayer in groupPlayers)
            {
                GUI.Label(new Rect(x, y, 100, 25), groupPlayer.name);
                y += 20;
            }
        }
    }
}