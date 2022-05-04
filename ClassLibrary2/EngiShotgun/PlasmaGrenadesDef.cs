using System;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace EngiShotgun
{
	// Token: 0x02000008 RID: 8
	public class PlasmaGrenadesDef
	{
		// Token: 0x0600002B RID: 43 RVA: 0x000030B2 File Offset: 0x000012B2
		public static void AddConfig(ConfigFile config)
		{
			stockSize = config.Bind("Plasma Grenade", "Stock Size", 2, "The default number of grenades which can be held.");
			cooldown = config.Bind("Plasma Grenade", "Ability Cooldown", 8f, "Seconds before a single grenade recharges.");
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000030F0 File Offset: 0x000012F0
		private static void AddLanguage()
		{
			bool flag = !LanguageAPI.Loaded;
			if (flag)
			{
				Debug.LogWarning("Skipping Language Init");
			}
			else
			{
				LanguageAPI.Add("ENGIPLUS_PLASMAGRENADES_NAME", "PlasmaGrenade");
				LanguageAPI.Add("ENGIPLUS_PLASMAGRENADES_DESCRIPTION", string.Format("Take aim and throw a plasma grenade that deals <style=cIsDamage>{0}% damage</style> on impact, and leaves a lingering pool of <style=cIsUtility>slowing</style> plasma that deals <style=cIsDamage>100% damage per second.</style> Can hold up to {1}.", PlasmaGrenades.damageCoeff.Value, PlasmaGrenadesDef.stockSize.Value));
				LanguageAPI.Add("ENGIPLUS_PLASMAGRENADES_NAMETOKEN", "Plasma Grenade");
				LanguageAPI.Add("ENGIPLUS_KEYWORD_SLOWING", "<style=cKeywordName>Slowing</style><style=cIsUtility>Slows enemies caught in the effect radius.</style>");
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000317B File Offset: 0x0000137B
		public static void Init(ConfigFile config)
		{
			PlasmaGrenades.Init(config);
			AddConfig(config);
			AddLanguage();
			AddDef();
			UpdateSkillFamily();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000031A0 File Offset: 0x000013A0
		private static void AddDef()
		{
			bool flag;
			ContentAddition.AddEntityState<PlasmaGrenades>(out flag);
			skillDef = ScriptableObject.CreateInstance<SkillDef>();
			skillDef.activationState = new SerializableEntityStateType(typeof(PlasmaGrenades));
			skillDef.activationStateMachineName = "Weapon";
			skillDef.baseMaxStock = stockSize.Value;
			skillDef.baseRechargeInterval = cooldown.Value;
			skillDef.beginSkillCooldownOnSkillEnd = true;
			skillDef.canceledFromSprinting = true;
			skillDef.fullRestockOnAssign = false;
			skillDef.interruptPriority = InterruptPriority.PrioritySkill;
			skillDef.isCombatSkill = true;
			skillDef.mustKeyPress = true;
			skillDef.cancelSprintingOnActivation = false;
			skillDef.forceSprintDuringState = false;
			skillDef.rechargeStock = 1;
			skillDef.requiredStock = 1;
			skillDef.stockToConsume = 1;
			skillDef.icon = skillSprite;
			skillDef.skillDescriptionToken = "ENGIPLUS_PLASMAGRENADES_DESCRIPTION";
			skillDef.skillName = "ENGIPLUS_PLASMAGRENADES_NAME";
			skillDef.skillNameToken = "ENGIPLUS_PLASMAGRENADES_NAMETOKEN";
			skillDef.keywordTokens = new string[]
			{
				"ENGIPLUS_KEYWORD_SLOWING"
			};
			if(flag)
            {
				ContentAddition.AddSkillDef(skillDef);
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000032E8 File Offset: 0x000014E8
		private static void UpdateSkillFamily()
		{
			SkillFamily skillFamily = skillLocator.secondary.skillFamily;
			Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
			SkillFamily.Variant[] variants = skillFamily.variants;
			int num = skillFamily.variants.Length - 1;
			SkillFamily.Variant variant = default(SkillFamily.Variant);
			variant.skillDef = skillDef;
			variant.viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null);
			variants[num] = variant;
			Debug.LogWarning("Finishing Grenade");
		}

		// Token: 0x0400002F RID: 47
		public static SkillDef skillDef;

		// Token: 0x04000030 RID: 48
		public static Sprite skillSprite = EngineerShotgunPlugin.MainAssets.LoadAsset<Sprite>("PlasmaGrenades");

		// Token: 0x04000031 RID: 49
		public static string configPrefix = "Plasma Grenade";

		// Token: 0x04000032 RID: 50
		private static SkillLocator skillLocator = Resources.Load<GameObject>("prefabs/characterbodies/EngiBody").GetComponent<SkillLocator>();

		// Token: 0x04000033 RID: 51
		public static ConfigEntry<int> stockSize;

		// Token: 0x04000034 RID: 52
		public static ConfigEntry<float> cooldown;
	}
}
