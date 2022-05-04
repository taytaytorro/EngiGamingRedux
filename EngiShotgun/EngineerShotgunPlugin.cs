using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using ShotgunengiREDUX.SkillDefs;
using System.Reflection;
using UnityEngine;
using Path = System.IO.Path;

namespace EngiShotgun
{
	[BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(new string[]
    {
        nameof(LoadoutAPI),
        nameof(ContentAddition),
        nameof(LanguageAPI),
        nameof(PrefabAPI),
        nameof(RecalculateStatsAPI)
    })]
    public class EngineerShotgunPlugin : BepInEx.BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource ModLogger;
        public const string PluginName = "Active Engineer EX";
        public const string PluginGUID = "com.macawesone.EngiShotgun";
        public const string PluginVersion = "3.1.0";

        private static AssetBundle _mainAssets;
        public static AssetBundle MainAssets
        {
            get
            {
                return _mainAssets;
            }
            set
            {
                _mainAssets = value;
            }
        }

        private void LoadAssetBundle()
        {
            var path = Path.GetDirectoryName(Info.Location);
            MainAssets = AssetBundle.LoadFromFile(Path.Combine(path, "engineerassets"));
        }
        private void EnableSkills()
        {
            EngiShotgunSlot.Init(Config);
            PlasmaGrenadesSlot.Init(Config);
            JetpackSlot.Init(Config);
        }
        public void Awake()
        {
            ModLogger = Logger;
            LoadAssetBundle();
            EnableSkills();
        }
    }
}
