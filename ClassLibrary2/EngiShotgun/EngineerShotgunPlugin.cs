using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using R2API.Utils;
using UnityEngine;

namespace EngiShotgun
{
	// Token: 0x02000005 RID: 5
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin("com.macawesone.EngiShotgun", "Active Engineer EX", "3.0.0")]
	[R2APISubmoduleDependency(new string[]
	{
		"LoadoutAPI",
		"ContentAddition",
		"LanguageAPI",
		"PrefabAPI"
	})]
	public class EngineerShotgunPlugin : BaseUnityPlugin
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00002D1C File Offset: 0x00000F1C
		private void LoadAssetBundle()
		{
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EngineerShotgunREDUX.engishotgunassets"))
			{
				MainAssets = AssetBundle.LoadFromStream(manifestResourceStream);
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002D64 File Offset: 0x00000F64
		private void EnableSkills()
		{
			PlasmaGrenadesDef.Init(Config);
			GaussShotgun.Init(Config);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002D7F File Offset: 0x00000F7F
		public void Awake()
		{
			EngineerShotgunPlugin.ModLogger = base.Logger;
			LoadAssetBundle();
			EnableSkills();
		}

		// Token: 0x04000025 RID: 37
		public static ManualLogSource ModLogger;

		// Token: 0x04000026 RID: 38
		public const string PluginName = "Active Engineer EX";

		// Token: 0x04000027 RID: 39
		public const string PluginGUID = "com.macawesone.EngiShotgun";

		// Token: 0x04000028 RID: 40
		public const string PluginVersion = "3.0.0";

		// Token: 0x04000029 RID: 41
		public static AssetBundle MainAssets;
	}
}
