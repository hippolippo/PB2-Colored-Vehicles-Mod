using System;
using System.Collections;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;
using UnityEngine;
using System.Reflection;
using PolyTechFramework;
namespace ColoredVehiclesMod {
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInDependency(PolyTechMain.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
    
    
    public class ColoredVehiclesMod : PolyTechMod {
        
        
        public const string pluginGuid = "polytech.ColoredVehiclesMod";
        public const string pluginName = "Colored Vehicles Mod";
        public const string pluginVersion = "1.0.0";
        
        public static ConfigEntry<bool> mEnabled;
        
        public ConfigDefinition mEnabledDef = new ConfigDefinition(pluginVersion, "Enable/Disable Mod");
        
        
        
        
        public override void enableMod(){
            mEnabled.Value = true;
            this.isEnabled = true;
        }
        public override void disableMod(){
            mEnabled.Value = false;
            this.isEnabled = false;
        }
        public override string getSettings(){
            return "";
        }
        public override void setSettings(string settings){
            
        }
        public ColoredVehiclesMod(){
            
            mEnabled = Config.Bind(mEnabledDef, false, new ConfigDescription("Controls if the mod should be enabled or disabled", null, new ConfigurationManagerAttributes {Order = 0}));
        }
        void Awake(){
            
            this.repositoryUrl = null;
            this.isCheat = true;
            PolyTechMain.registerMod(this);
            Logger.LogInfo("Colored Vehicles Mod Registered");
            Harmony.CreateAndPatchAll(typeof(ColoredVehiclesMod));
            Logger.LogInfo("Colored Vehicles Mod Methods Patched");
        }
        void Update(){
            
        }
    }
}