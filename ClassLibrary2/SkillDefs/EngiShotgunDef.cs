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

namespace EngiShotgun
{
    public class EngiShotgunDef
	{
		static ReloadSkillDef reloadSkillDef;
		public static UnityEngine.Sprite skillSprite = MainAssets.LoadAsset<UnityEngine.Sprite>("GaussShotgun");

		private static void AddLanguage()
		{
			LanguageAPI.Add("ENGIPLUS_ENGISHOTGUN_NAME", "EngiShotgun");
			LanguageAPI.Add("ENGIPLUS_ENGISHOTGUN_DESCRIPTION", $"Fire a close-range blast of pellets, dealing <style=cIsDamage>8x{weapon.EngiShotgun.damageCoeff.Value}% damage</style>. Holds up to {shotgunStock.Value} total rounds.");
			LanguageAPI.Add("ENGIPLUS_ENGISHOTGUN_NAMETOKEN", "Gauss Scatter-rifle");
		}

		public static ConfigEntry<int> shotgunStock;

		private static void AddConfig(ConfigFile config)
        {
			shotgunStock = config.Bind<int>("Gauss Shotgun", "Max Ammo", 8, "The maximum amount of ammo in a shotgun magazine.");
        }
		private static void AddDef()
        {
			reloadSkillDef = UnityEngine.ScriptableObject.CreateInstance<ReloadSkillDef>();
			reloadSkillDef.activationState = new SerializableEntityStateType(typeof(weapon.EngiShotgun));
			reloadSkillDef.baseMaxStock = shotgunStock.Value;
			reloadSkillDef.rechargeStock = 0;
			reloadSkillDef.skillDescriptionToken = "ENGIPLUS_ENGISHOTGUN_DESCRIPTION";
			reloadSkillDef.skillName = "ENGIPLUS_ENGISHOTGUN_NAME";
			reloadSkillDef.skillNameToken = "Gauss Scatter-rifle";
			reloadSkillDef.activationStateMachineName = "Weapon";
			reloadSkillDef.beginSkillCooldownOnSkillEnd = false;
			reloadSkillDef.fullRestockOnAssign = false;
			reloadSkillDef.interruptPriority = EntityStates.InterruptPriority.Skill;
			reloadSkillDef.isCombatSkill = true;
			reloadSkillDef.cancelSprintingOnActivation = true;
			reloadSkillDef.canceledFromSprinting = false;
			reloadSkillDef.mustKeyPress = false;
			reloadSkillDef.icon = skillSprite;
			reloadSkillDef.requiredStock = 1;
			reloadSkillDef.stockToConsume = 1;
			reloadSkillDef.reloadInterruptPriority = InterruptPriority.Any;
			reloadSkillDef.reloadState = new SerializableEntityStateType(typeof(weapon.Reload));
			reloadSkillDef.graceDuration = 0.5f;
		}
		public static void Init(ConfigFile config)
        {
			UnityEngine.Debug.Log("Loading Engineer Shotgun");
			weapon.Reload.Init(config);
			weapon.EngiShotgun.Init(config);
			AddConfig(config);
			AddLanguage();
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
				skillDef = reloadSkillDef,
				viewableNode = new ViewablesCatalog.Node(reloadSkillDef.skillNameToken, false, null)
			};
		}
		
		
    }
}
