using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using PolyTechFramework;
using System;
using System.Reflection;
using Color = UnityEngine.Color;

namespace WaterColorChanger
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(PolyTechFramework.PolyTechMain.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("Poly Bridge 2.exe")]
    public class PluginMain : PolyTechMod
    {
        public const String PluginGuid = "polytech.watercolorchanger";
        public const String PluginName = "Water Color Changer";
        public const String PluginVersion = "1.0.0.0";

        public static bool IsEnabled
        {
            get
            {
                return Enabled.Value && PolyTechFramework.PolyTechMain.modEnabled.Value;
            }
        }

        public static ConfigEntry<float> WaterR;
        public static ConfigEntry<float> WaterG;
        public static ConfigEntry<float> WaterB;
        public static ConfigEntry<float> WaterA;
        public static ConfigEntry<bool> Enabled;
        public static Color waterColor;
        public static Color origColor;
        void Awake()
        {
            var harmony = new Harmony(PluginGuid);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            Enabled = Config.Bind("Settings", "Enabled", true, new ConfigDescription("Turn this off to stop the water change", null, new ConfigurationManagerAttributes { Order = 5 }));

            WaterR = Config.Bind("Settings", "Water R", 1f, new ConfigDescription("Red value for the water", null, new ConfigurationManagerAttributes { Order = 4 }));
            WaterR.SettingChanged += UpdateWaterColor;

            WaterG = Config.Bind("Settings", "Water G", 1f, new ConfigDescription("Green value for the water", null, new ConfigurationManagerAttributes { Order = 3 }));
            WaterG.SettingChanged += UpdateWaterColor;
            
            WaterB = Config.Bind("Settings", "Water B", 1f, new ConfigDescription("Blue value for the water", null, new ConfigurationManagerAttributes { Order = 2 }));
            WaterB.SettingChanged += UpdateWaterColor;

            WaterA = Config.Bind("Settings", "Water A", 1f, new ConfigDescription("Alpha value for the water", null, new ConfigurationManagerAttributes { Order = 1 }));
            WaterA.SettingChanged += UpdateWaterColor;

            this.isEnabled = true;
            this.isCheat = false;
            PolyTechFramework.PolyTechMain.registerMod(this);

            waterColor = new Color(WaterR.Value, WaterG.Value, WaterB.Value, WaterA.Value);
        }

        private void UpdateWaterColor(object sender, EventArgs e)
        {
            waterColor = new Color(WaterR.Value, WaterG.Value, WaterB.Value, WaterA.Value);
            Logger.LogInfo("Set water color to: " + waterColor.ToString());
        }
    }
    [HarmonyPatch(typeof(WaterBlock), "Update")]
    public static class patchWaterBlock
    {
        [HarmonyPostfix]
        static void Postfix(ref WaterBlock __instance)
        {
            if (PluginMain.IsEnabled)
            {
                __instance.m_MeshRenderer.material.color = PluginMain.waterColor;
            }
            else
            {
                __instance.m_MeshRenderer.material.color = PluginMain.origColor;
            }
        }
    }
    [HarmonyPatch(typeof(WaterBlock), "Start")]
    public static class grabWaterBlockOrigColor
    {
        [HarmonyPostfix]
        static void Postfix(ref WaterBlock __instance)
        {
            PluginMain.origColor = __instance.m_MeshRenderer.material.color;
        }
    }
}
