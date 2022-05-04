using System;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using EngiShotgun = EntityStates.Engi.EngiWeapon.GaussShotgun;

namespace EngiShotgun
{
	// Token: 0x02000007 RID: 7
	public class GaussShotgun
	{
		// Token: 0x06000024 RID: 36 RVA: 0x00002E04 File Offset: 0x00001004
		private static void AddLanguage()
		{
			bool flag = !LanguageAPI.Loaded;
			if (flag)
			{
				Debug.LogWarning("Skipping Language Init");
			}
			else
			{
				LanguageAPI.Add("ENGIPLUS_ENGISHOTGUN_NAME", "EngiShotgun");
                LanguageAPI.Add("ENGIPLUS_ENGISHOTGUN_DESCRIPTION", string.Format("Fire a close-range blast of pellets, dealing <style=cIsDamage>8x{0}% damage</style>. Holds up to {1} total rounds.", EntityStates.Engi.EngiWeapon.GaussShotgun.damageCoeff.Value, GaussShotgun.shotgunStock.Value));
				LanguageAPI.Add("ENGIPLUS_ENGISHOTGUN_NAMETOKEN", "Gauss Scatter-rifle");
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002E7F File Offset: 0x0000107F
		private static void AddConfig(ConfigFile config)
		{
			GaussShotgun.shotgunStock = config.Bind<int>("Gauss Shotgun", "Max Ammo", 8, "The maximum amount of ammo in a shotgun magazine.");
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002EA0 File Offset: 0x000010A0
		private static void AddDef()
		{
			bool flag;
            ContentAddition.AddEntityState<EntityStates.Engi.EngiWeapon.GaussShotgun>(out flag);
			GaussShotgun.reloadSkillDef = ScriptableObject.CreateInstance<ReloadSkillDef>();
            GaussShotgun.reloadSkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Engi.EngiWeapon.GaussShotgun));
			GaussShotgun.reloadSkillDef.baseMaxStock = GaussShotgun.shotgunStock.Value;
			GaussShotgun.reloadSkillDef.rechargeStock = 0;
			GaussShotgun.reloadSkillDef.skillDescriptionToken = "ENGIPLUS_ENGISHOTGUN_DESCRIPTION";
			GaussShotgun.reloadSkillDef.skillName = "ENGIPLUS_ENGISHOTGUN_NAME";
			GaussShotgun.reloadSkillDef.skillNameToken = "Gauss Scatter-rifle";
			GaussShotgun.reloadSkillDef.activationStateMachineName = "Weapon";
			GaussShotgun.reloadSkillDef.beginSkillCooldownOnSkillEnd = false;
			GaussShotgun.reloadSkillDef.fullRestockOnAssign = false;
			GaussShotgun.reloadSkillDef.interruptPriority = InterruptPriority.PrioritySkill;
			GaussShotgun.reloadSkillDef.isCombatSkill = true;
			GaussShotgun.reloadSkillDef.cancelSprintingOnActivation = true;
			GaussShotgun.reloadSkillDef.canceledFromSprinting = false;
			GaussShotgun.reloadSkillDef.mustKeyPress = false;
			GaussShotgun.reloadSkillDef.icon = GaussShotgun.skillSprite;
			GaussShotgun.reloadSkillDef.requiredStock = 1;
			GaussShotgun.reloadSkillDef.stockToConsume = 1;
			GaussShotgun.reloadSkillDef.reloadInterruptPriority = 0;
			GaussShotgun.reloadSkillDef.reloadState = new SerializableEntityStateType(typeof(Reload));
			GaussShotgun.reloadSkillDef.graceDuration = 0.5f;
			if(flag)
            {
				ContentAddition.AddSkillDef(GaussShotgun.reloadSkillDef);
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002FDC File Offset: 0x000011DC
		public static void Init(ConfigFile config)
		{
			Reload.Init(config);
            EntityStates.Engi.EngiWeapon.GaussShotgun.Init(config);
			GaussShotgun.AddConfig(config);
			GaussShotgun.AddLanguage();
			GaussShotgun.AddDef();
			GaussShotgun.UpdateSkillFamily();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003008 File Offset: 0x00001208
		private static void UpdateSkillFamily()
		{
			SkillFamily skillFamily = GaussShotgun.skillLocator.primary.skillFamily;
			Array.Resize<SkillFamily.Variant>(ref skillFamily.variants, skillFamily.variants.Length + 1);
			SkillFamily.Variant[] variants = skillFamily.variants;
			int num = skillFamily.variants.Length - 1;
			SkillFamily.Variant variant = default(SkillFamily.Variant);
			variant.skillDef = GaussShotgun.reloadSkillDef;
			variant.viewableNode = new ViewablesCatalog.Node(GaussShotgun.reloadSkillDef.skillNameToken, false, null);
			variants[num] = variant;
		}

		// Token: 0x0400002B RID: 43
		private static ReloadSkillDef reloadSkillDef;

		// Token: 0x0400002C RID: 44
		public static Sprite skillSprite = EngineerShotgunPlugin.MainAssets.LoadAsset<Sprite>("GaussShotgun");

		// Token: 0x0400002D RID: 45
		private static SkillLocator skillLocator = Resources.Load<GameObject>("prefabs/characterbodies/EngiBody").GetComponent<SkillLocator>();

		// Token: 0x0400002E RID: 46
		public static ConfigEntry<int> shotgunStock;
	}
}
