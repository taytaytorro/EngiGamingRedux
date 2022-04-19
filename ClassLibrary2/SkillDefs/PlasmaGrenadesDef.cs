using System;
using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using weapon = EntityStates.Engi.EngiWeapon;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using static EngiShotgun.EngineerShotgunPlugin;

namespace EngiShotgun
{
    public class PlasmaGrenadesDef
    {
		public static SkillDef skillDef;
		public static UnityEngine.Sprite skillSprite = MainAssets.LoadAsset<UnityEngine.Sprite>("PlasmaGrenades");

		public static ConfigEntry<int> stockSize;
		public static ConfigEntry<float> cooldown;
		public static void AddConfig(ConfigFile config)
        {
			stockSize = config.Bind<int>("Plasma Grenade", "Stock Size", 2, "The default number of grenades which can be held.");
			cooldown = config.Bind<float>("Plasma Grenade", "Ability Cooldown", 8f, "Seconds before a single grenade recharges.");
        }
		private static void AddLanguage()
		{
			LanguageAPI.Add("ENGIPLUS_PLASMAGRENADES_NAME", "PlasmaGrenade");
			LanguageAPI.Add("ENGIPLUS_PLASMAGRENADES_DESCRIPTION", $"Take aim and throw a plasma grenade that deals <style=cIsDamage>{weapon.PlasmaGrenades.damageCoeff.Value}% damage</style> on impact, and leaves a lingering pool of <style=cIsUtility>slowing</style> plasma that deals <style=cIsDamage>100% damage per second.</style> Can hold up to {stockSize.Value}.");
			LanguageAPI.Add("ENGIPLUS_PLASMAGRENADES_NAMETOKEN", "Plasma Grenade");
			LanguageAPI.Add("ENGIPLUS_KEYWORD_SLOWING", "Slows enemies caught in the effect radius.");

		}
		public static void Init(ConfigFile config)
		{
			weapon.PlasmaGrenades.Init(config);
			AddConfig(config);
			AddLanguage();
			AddDef();
			UpdateSkillFamily();
        }
		private static void AddDef()
		{
			R2API.ContentAddition.AddEntityState<weapon.PlasmaGrenades>(out _);
			skillDef = UnityEngine.ScriptableObject.CreateInstance<SkillDef>();
			skillDef.activationState = new SerializableEntityStateType(typeof(weapon.PlasmaGrenades));
			skillDef.activationStateMachineName = "Weapon";
			skillDef.baseMaxStock = stockSize.Value;
			skillDef.baseRechargeInterval = cooldown.Value;
			skillDef.beginSkillCooldownOnSkillEnd = true;
			skillDef.canceledFromSprinting = true;
			skillDef.fullRestockOnAssign = false;
			skillDef.interruptPriority = InterruptPriority.Skill;
			skillDef.isCombatSkill = true;
			skillDef.mustKeyPress = false;
			skillDef.cancelSprintingOnActivation = false;
			skillDef.forceSprintDuringState = false;
			skillDef.rechargeStock = 1;
			skillDef.requiredStock = 1;
			skillDef.stockToConsume = 1;
			skillDef.icon = skillSprite;
			skillDef.skillDescriptionToken = "ENGIPLUS_PLASMAGRENADES_DESCRIPTION";
			skillDef.skillName = "ENGIPLUS_PLASMAGRENADES_NAME";
			skillDef.skillNameToken = "ENGIPLUS_PLASMAGRENADES_NAMETOKEN";
			skillDef.keywordTokens = new string[] { "ENGIPLUS_KEYWORD_SLOWING" };
			ContentAddition.AddSkillDef(skillDef);
		}
		private static void UpdateSkillFamily()
        {
			SkillLocator component = UnityEngine.Resources.Load<UnityEngine.GameObject>("prefabs/characterbodies/EngiBody").GetComponent<SkillLocator>();
			var skillFamily = component.secondary.skillFamily;
			Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
			skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
			{
				skillDef = skillDef,
				viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
			};
		}
		
		
    }
}
