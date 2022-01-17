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

        public static int[] ids = {
            0,
            1,
            0,
            0,
            0,
            1,
            0,
            0,
            1,
            9,
            3,
            5,
            1,
            1,
            4,
            4,
            2,
            4,
            0
        };

        public static string[] colorSettingNames = {
            "Vespa Color",
            "Chopper Color",
            "Compact Car Color",
            "Buggy Color",
            "Sports Car Color",
            "Taxi Color",
            "Model T Color",
            "Pickup Truck Color",
            "Limo Color",
            "Van Color",
            "Tow Truck Color",
            "Steam Car Color",
            "Big Truck Cab Color",
            "Monster Truck Color",
            "Ambulance Color",
            "School Bus Color",
            "Fire Truck Color",
            "Bulldozer Color",
            "Dump Truck Color"
        };
        /*
                    "Biplane Color",
            "Seaplane Color",
            "Show Jet Color",
            "Blimp Color",
            "Hydrofoil Color",
            "Speedboat Color",
            "Sailboat Color",
            "Submarine Color",
            "Pirate Ship Color",
            "Steamboat Color",
            "Cruise Ship Color"*/

        public static GameObject[] prefabs;

        public static int[] materialIndices;
        
        public static ConfigEntry<bool> mEnabled;

        public static ConfigEntry<int> num;
        public static ConfigEntry<bool> LoadFromFile;
        public static ConfigEntry<string> FilePath;
        public static ConfigEntry<string>[] vehicleColors = new ConfigEntry<string>[colorSettingNames.Length];
        public const string generalText = "General Settings";
        public const string vehicleText = "Vehicle Colors";

        
        public ConfigDefinition mEnabledDef = new ConfigDefinition(generalText, "Enable/Disable Mod");
        public ConfigDefinition numDef = new ConfigDefinition(generalText, "num");
        public ConfigDefinition LoadFromFileDef = new ConfigDefinition(generalText, "Load Settings From File");
        public ConfigDefinition FilePathDef = new ConfigDefinition(generalText, "Settings Filepath");
        public ConfigDefinition[] VehicleColorsDef = new ConfigDefinition[colorSettingNames.Length];

        public string[] currentHexes = new string[colorSettingNames.Length];
        public string[] lastHexes = new string[colorSettingNames.Length];


        
        
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
            int order = 0;
            //mEnabled = Config.Bind(mEnabledDef, false, new ConfigDescription("Controls if the mod should be enabled or disabled", null, new ConfigurationManagerAttributes {Order = order--}));
            //num = Config.Bind(numDef,9,new ConfigDescription("", null, new ConfigurationManagerAttributes {Order = order --}));
            //LoadFromFile = Config.Bind(LoadFromFileDef, false, new ConfigDescription("Should the colors load from a file instead of from the settings", null, new ConfigurationManagerAttributes {Order = order--}));
            //FilePath = Config.Bind(FilePathDef, "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Poly Bridge 2\\BepInEx\\config\\VehicleColors.json", new ConfigDescription("Filepath for the settings to load", null, new ConfigurationManagerAttributes {Order = order--}));
            for(int i = 0; i < colorSettingNames.Length; i++){
                VehicleColorsDef[i] = new ConfigDefinition(vehicleText, colorSettingNames[i]);
                vehicleColors[i] = Config.Bind(VehicleColorsDef[i], "None", new ConfigDescription("Accepts Hexadecimal Colors (#AABBCC)", null, new ConfigurationManagerAttributes {Order = order--}));
            }
        }
        void Awake(){

            this.repositoryUrl = null;
            this.isCheat = false;
            PolyTechMain.registerMod(this);
            Logger.LogInfo("Colored Vehicles Mod Registered");
            Harmony.CreateAndPatchAll(typeof(ColoredVehiclesMod));
            Logger.LogInfo("Colored Vehicles Mod Methods Patched");
        }
        void Update(){
            prefabs = new GameObject[]{
                Prefabs.m_Instance.m_Vespa,
                Prefabs.m_Instance.m_Chopper,
                Prefabs.m_Instance.m_CompactCar,
                Prefabs.m_Instance.m_DuneBuggy,
                Prefabs.m_Instance.m_SportsCar,
                Prefabs.m_Instance.m_Taxi,
                Prefabs.m_Instance.m_ModelT,
                Prefabs.m_Instance.m_PickupTruck,
                Prefabs.m_Instance.m_Limo,
                Prefabs.m_Instance.m_Van,
                Prefabs.m_Instance.m_TowTruck,
                Prefabs.m_Instance.m_SteamCar,
                Prefabs.m_Instance.m_Truck,
                Prefabs.m_Instance.m_MonsterTruck,
                Prefabs.m_Instance.m_Ambulance,
                Prefabs.m_Instance.m_SchoolBus,
                Prefabs.m_Instance.m_FireTruck,
                Prefabs.m_Instance.m_Bulldozer,
                Prefabs.m_Instance.m_DumpTruck
            };
            /*
                            Prefabs.m_Instance.m_BiPlane,
                Prefabs.m_Instance.m_SeaPlane,
                Prefabs.m_Instance.m_ShowJet,
                Prefabs.m_Instance.m_Blimp,
                Prefabs.m_Instance.m_Hydrofoil,
                Prefabs.m_Instance.m_SpeedBoat,
                Prefabs.m_Instance.m_Sailboat,
                Prefabs.m_Instance.m_Submarine,
                Prefabs.m_Instance.m_PirateShip,
                Prefabs.m_Instance.m_Steamboat,
                Prefabs.m_Instance.m_CruiseShip*/
            for(int i = 0; i < vehicleColors.Length; i++){
                currentHexes[i] = vehicleColors[i].Value;
            }
            for(int i = 0; i < currentHexes.Length; i++){
                if (currentHexes[i] != lastHexes[i] && currentHexes[i].Length == 7 && currentHexes[i][0] == '#'){
                    Color color;
                    ColorUtility.TryParseHtmlString(vehicleColors[i].Value, out color);
                    prefabs[i].GetComponent<Vehicle>().m_MeshRenderer.materials[ids[i]].SetColor("_Color", color);
                }
                lastHexes[i] = currentHexes[i];
            }
        }
    }
}