using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace EngiShotgun
{
	[BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(new string[]
    {
        nameof(LoadoutAPI),
        nameof(ContentAddition),
        nameof(LanguageAPI)
    })]
    public class EngineerShotgunPlugin : BepInEx.BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource ModLogger;
        public const string PluginName = "Active Engineer EX";
        public const string PluginGUID = "com.macawesone.EngiShotgun";
        public const string PluginVersion = "3.0.0";

        public static AssetBundle MainAssets;

        private void LoadAssetBundle()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Assets.engineerassets"))
            {
                MainAssets = AssetBundle.LoadFromStream(stream);
            }
        }
        private void EnableSkills()
        {
            EngiShotgunDef.Init(Config);
            PlasmaGrenadesDef.Init(Config);
        }
        public void Awake()
        {
            ModLogger = Logger;
            LoadAssetBundle();
            EnableSkills();
        }
    }
}
