using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

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
        
        private void Awake()
        {
            
            
            
            
            harmony.PatchAll();
            Log.LogInfo($"Plugin {PluginName} loaded");
        }
    }
}
