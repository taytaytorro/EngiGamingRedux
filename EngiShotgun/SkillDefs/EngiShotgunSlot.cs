using System;
using BepInEx;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using weapon = EntityStates.Engi.EngiWeapon;
using BepInEx.Configuration;
using static EngiShotgun.EngineerShotgunPlugin;
using ShotgunengiREDUX.SkillDefs;

namespace EngiShotgun
{
    public class EngiShotgunSlot
	{
		static GaussShotgun_SkillDef gaussSkillDef;
		public static UnityEngine.Sprite skillSprite => MainAssets.LoadAsset<UnityEngine.Sprite>("GaussShotgun");

		public static string configPrefix = "Gauss Shotgun";

		private static void AddLanguage()
		{
			LanguageAPI.Add("ENGIPLUS_ENGISHOTGUN_NAME", "EngiShotgun");
			LanguageAPI.Add("ENGIPLUS_ENGISHOTGUN_DESCRIPTION", $"Fire a close-range blast of pellets, dealing <style=cIsDamage>8x{damageCoeff.Value}% damage</style>. Holds up to {shotgunStock.Value} total rounds.");
			LanguageAPI.Add("ENGIPLUS_ENGISHOTGUN_NAMETOKEN", "Gauss Scatter-rifle");
		}


		public static ConfigEntry<int> shotgunStock;
		public static ConfigEntry<int> damageCoeff;
		public static ConfigEntry<int> maxVisibleStocks;
		public static ConfigEntry<bool> clickSpam;

		private static void AddConfig(ConfigFile config)
		{
			damageCoeff = config.Bind(configPrefix, "Projectile Damage%", 60, "The base damage of each shotgun projectile, in %");
			shotgunStock = config.Bind<int>(configPrefix, "Max Ammo", 8, "The maximum amount of ammo in a shotgun magazine.");
			maxVisibleStocks = config.Bind(configPrefix, "Max Visible Stock", 10, "The maximum amount of shotgun stock that can appear in your HUD.");
			clickSpam = config.Bind(configPrefix, "Spammable", false, "Whether the Gauss Shotgun is auto or semi-auto.");
		}
		private static void AddDef()
		{
			var GaussShotgunState = R2API.ContentAddition.AddEntityState<weapon.GaussShotgunState>(out _);
			gaussSkillDef = UnityEngine.ScriptableObject.CreateInstance<GaussShotgun_SkillDef>();
			gaussSkillDef.activationState = GaussShotgunState;
			gaussSkillDef.baseMaxStock = shotgunStock.Value;
			gaussSkillDef.rechargeStock = 0;
			gaussSkillDef.skillDescriptionToken = "ENGIPLUS_ENGISHOTGUN_DESCRIPTION";
			gaussSkillDef.skillName = "ENGIPLUS_ENGISHOTGUN_NAME";
			gaussSkillDef.skillNameToken = "Gauss Scatter-rifle";
			gaussSkillDef.activationStateMachineName = "Weapon";
			gaussSkillDef.beginSkillCooldownOnSkillEnd = false;
			gaussSkillDef.fullRestockOnAssign = false;
			gaussSkillDef.interruptPriority = EntityStates.InterruptPriority.Skill;
			gaussSkillDef.isCombatSkill = true;
			gaussSkillDef.cancelSprintingOnActivation = true;
			gaussSkillDef.canceledFromSprinting = false;
			gaussSkillDef.mustKeyPress = !clickSpam.Value;
			gaussSkillDef.icon = skillSprite;
			gaussSkillDef.requiredStock = 1;
			gaussSkillDef.stockToConsume = 1;
			gaussSkillDef.reloadInterruptPriority = InterruptPriority.Any;
			gaussSkillDef.reloadState = new SerializableEntityStateType(typeof(weapon.ReloadState));
			gaussSkillDef.graceDuration = 0.5f;
			GaussShotgun_SkillDef.instance = gaussSkillDef;
			ContentAddition.AddSkillDef(gaussSkillDef);
		}
		public static void Init(ConfigFile config)
        {
			AddConfig(config);
			AddLanguage();
			weapon.ReloadState.Init(config);
			weapon.GaussShotgunState.Init(config);
			AddDef();
			UpdateSkillFamily();
        }
		private static void UpdateSkillFamily()
        {
			SkillLocator component = UnityEngine.Resources.Load<UnityEngine.GameObject>("prefabs/characterbodies/EngiBody").GetComponent<SkillLocator>();
			var skillFamily = component.primary.skillFamily;
			Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
			skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
			{
				skillDef = gaussSkillDef,
				viewableNode = new ViewablesCatalog.Node(gaussSkillDef.skillNameToken, false, null)
			};
		}
		
		
    }
}
