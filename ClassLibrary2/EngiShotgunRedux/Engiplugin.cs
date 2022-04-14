using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using EntityStates;
using EntityStates.Engi.EngiWeapon.Rux1;
using EntityStates.Engi.EngiWeapon.Rux2;
using EntityStates.Engi.EngiWeapon.Reload;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine.Networking;




namespace EngiShotgu
{
	// Token: 0x02000007 RID: 7
	[BepInDependency("com.bepis.r2api")]
	[BepInPlugin("com.macawesone.EngiShotgun", "EngiGamingREDUX", "1.0.2")]
	[R2APISubmoduleDependency(new string[]
	{
		"PrefabAPI",
		"SurvivorAPI",
		"LanguageAPI",
		"LoadoutAPI",
		"ItemAPI",
		"DifficultyAPI",
		"BuffAPI",
		"ProjectileAPI",
		"EffectAPI"
	})]
	
	public class Engiplugin : BaseUnityPlugin
	{

		public static AssetBundle assetBundle = null;
		private bool load = true;
		public static PluginInfo PInfo { get; private set; }
		public Sprite gaussShotgunIconS;
		public Sprite plasmaGrenadeIconS;
		private void LoadAssetBundle()
		{
			PInfo = Info;
			if (assetBundle == null)
			{

				string dir = System.IO.Path.GetDirectoryName(Info.Location);
				assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(PInfo.Location, "..", "AssetBundleStaging", "icons"));
				load = assetBundle;
				Logger.LogMessage("loaded");
			}
			if (!load)
			{
				Logger.LogMessage("load failed");
				return; 
			}
			gaussShotgunIconS = assetBundle.LoadAsset<Sprite>("Assets/Icons/engishotgunicon.png");
			plasmaGrenadeIconS = assetBundle.LoadAsset<Sprite>("Assets/Icons/grenade1.png");
		}

		// Token: 0x04000027 RID: 39
		

		// Token: 0x04000028 RID: 40
		//public static Sprite gaussShotgunIconS = Assets.TexToSprite(Engiplugin.gaussShotgunIcon);

		// Token: 0x04000029 RID: 41
		

		// Token: 0x0400002A RID: 42
		//public static Sprite plasmaGrenadeIconS = Assets.TexToSprite(Engiplugin.plasmaGrenadeIcon);
		public void Awake()
		{
			

			LoadAssetBundle();
			this.SetupProjectiles();
			SkillLocator component = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/EngiBody").GetComponent<SkillLocator>();
			SkillFamily skillFamily = component.primary.skillFamily;
			SkillFamily skillFamily2 = component.secondary.skillFamily;

			R2API.ContentAddition.AddEntityState<GaussShotgun>(out _);
			SkillDef skillDef2 = ScriptableObject.CreateInstance<SkillDef>();
			skillDef2.skillDescriptionToken = "Fire a close-range blast of pellets, dealing <style=cIsDamage>8x60% damage</style>. Holds 8 total rounds.";
			skillDef2.skillName = "EngiShotgun";
			skillDef2.skillNameToken = "Gauss Scatter";
			skillDef2.icon = gaussShotgunIconS;
			skillDef2.activationState = new SerializableEntityStateType(typeof(GaussShotgun));
			skillDef2.activationStateMachineName = "Weapon";
			skillDef2.baseMaxStock = 1;
			//skillDef2.baseRechargeInterval = .75f;
			skillDef2.beginSkillCooldownOnSkillEnd = true;
			skillDef2.canceledFromSprinting = false;
			skillDef2.fullRestockOnAssign = true;
			skillDef2.interruptPriority = InterruptPriority.Skill;
			skillDef2.resetCooldownTimerOnUse = true;
			skillDef2.mustKeyPress = false;
			skillDef2.isCombatSkill = true;
			skillDef2.cancelSprintingOnActivation = true;
			skillDef2.forceSprintDuringState = false;
			skillDef2.rechargeStock = 1;
			skillDef2.requiredStock = 1;
			skillDef2.stockToConsume = 0;

			ContentAddition.AddSkillDef(skillDef2);
			//skillDef2.reloadState = new SerializableEntityStateType(typeof(EntityStates.Engi.EngiWeapon.Reload.Reload));
			//skillDef2.graceDuration = .7f;
			//skillDef2.reloadInterruptPriority = InterruptPriority.Any;
			Array.Resize<SkillFamily.Variant>(ref skillFamily.variants, skillFamily.variants.Length + 1);
			SkillFamily.Variant[] variants = skillFamily.variants;
			int num = skillFamily.variants.Length - 1;
			SkillFamily.Variant variant = default(SkillFamily.Variant);
			variant.skillDef = skillDef2;
			variant.unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
			variant.viewableNode = new ViewablesCatalog.Node(skillDef2.skillNameToken, false, null);
			variants[num] = variant;







			R2API.ContentAddition.AddEntityState<PlasmaGrenade>(out _);
			SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
			skillDef.activationState = new SerializableEntityStateType(typeof(PlasmaGrenade));
			skillDef.activationStateMachineName = "Weapon";
			skillDef.baseMaxStock = 2;
			skillDef.baseRechargeInterval = 8f;
			skillDef.beginSkillCooldownOnSkillEnd = true;
			skillDef.canceledFromSprinting = true;
			skillDef.fullRestockOnAssign = false;
			skillDef.interruptPriority = InterruptPriority.Skill;
			skillDef.isCombatSkill = true;
			skillDef.mustKeyPress = true;
			skillDef.cancelSprintingOnActivation = true;
			skillDef.forceSprintDuringState = false;
			skillDef.rechargeStock = 1;
			skillDef.requiredStock = 1;
			skillDef.stockToConsume = 1;
			skillDef.icon = plasmaGrenadeIconS;
			skillDef.skillDescriptionToken = "Take aim and lob a plasma grenade that deals <style=cIsDamage>600% damage</style> on impact and leaves a pool of plasma that deals <style=cIsDamage>30% damage per tick</style> and <style=cIsUtility>slows</style>. Can hold up to 2.";
			skillDef.skillName = "PlasmaGrenade";
			skillDef.skillNameToken = "Plasma Grenade";
			ContentAddition.AddSkillDef(skillDef);
			Array.Resize<SkillFamily.Variant>(ref skillFamily2.variants, skillFamily2.variants.Length + 1);
			SkillFamily.Variant[] variants2 = skillFamily2.variants;
			int num2 = skillFamily2.variants.Length - 1;
			variant = default(SkillFamily.Variant);
			variant.skillDef = skillDef;
			variant.unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
			variant.viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null);
			variants2[num2] = variant;
		}


		// Token: 0x06000024 RID: 36 RVA: 0x00002DC6 File Offset: 0x00000FC6
		private void SetupProjectiles()
		{
			this.SetupPlasmaGrenade();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002DD0 File Offset: 0x00000FD0
		private void SetupPlasmaGrenade()
		{
			Engiplugin.PlasmaGrenadeObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/banditgrenadeprojectile"), "PlasmaGrenade", true);
			this.PlasmaGrenadeGhostObject = PrefabAPI.InstantiateClone(Engiplugin.PlasmaGrenadeObject.GetComponent<ProjectileController>().ghostPrefab, "PlasmaGrenadeGhost", false);
			Engiplugin.projectilePrefabs.Add(Engiplugin.PlasmaGrenadeObject);
			Engiplugin.PlasmaGrenadeObject.GetComponent<ProjectileController>().ghostPrefab = this.PlasmaGrenadeGhostObject;
			Logger.LogMessage("adding1");
			//Engiplugin.PlasmaGrenadeObject.AddComponent<NetworkIdentity>();
			Engiplugin.PlasmaGrenadeObject.AddComponent<NetworkBehaviour>();
			GameObject gameObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocoleapacid"), "BasedPlasmaPuddle", true);
			Engiplugin.projectilePrefabs.Add(gameObject);
			Logger.LogMessage("adding2");
			base.gameObject.AddComponent<NetworkIdentity>();
			Logger.LogMessage("adding3");
			base.gameObject.AddComponent<NetworkBehaviour>();
			ProjectileDamage component = gameObject.GetComponent<ProjectileDamage>();
			component.damageType = DamageType.SlowOnHit;
			component.damage = 6f;
			component.force = 30f;
			ProjectileDotZone component2 = gameObject.GetComponent<ProjectileDotZone>();
			component2.attackerFiltering = 0;
			component2.overlapProcCoefficient = 0.15f;
			component2.lifetime = 5f;
			component2.damageCoefficient = 0.3f;
			GameObject gameObject2 = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion"), "PlasmaGrenadeBoomEffect", false);
			//gameObject2.GetComponent<EffectComponent>().soundName = "Play_acrid_shift_land";
			EffectComponent component3 = gameObject2.GetComponent<EffectComponent>();
			ProjectileImpactExplosion component4 = Engiplugin.PlasmaGrenadeObject.GetComponent<ProjectileImpactExplosion>();
			component4.blastRadius = 8.51f;
			Engiplugin.PlasmaGrenadeObject.GetComponent<ProjectileDamage>().damageType = DamageType.SlowOnHit;
			component4.blastProcCoefficient = 1f;
			component4.falloffModel = 0;
			component4.lifetime = 20f;
			component4.impactEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion");
			component4.explosionSoundString = "Play_engi_M2_explo";
			component4.timerAfterImpact = false;
			component4.lifetimeAfterImpact = 0f;
			component4.destroyOnWorld = true;
			component4.destroyOnEnemy = true;
			component4.fireChildren = true;
			component4.childrenCount = 1;
			component4.childrenDamageCoefficient = 0.3f;
			component4.childrenProjectilePrefab = gameObject;
			UnityEngine.Object.Destroy(Engiplugin.PlasmaGrenadeObject.GetComponent<ProjectileStickOnImpact>());
			Engiplugin.PlasmaGrenadeObject.GetComponent<Rigidbody>().useGravity = true;
			Logger.LogMessage("adding6");
			this.PlasmaGrenadeGhostObject.AddComponent<NetworkIdentity>();
			Logger.LogMessage("adding7");
			this.PlasmaGrenadeGhostObject.AddComponent<NetworkBehaviour>();
			ContentAddition.AddProjectile(Engiplugin.PlasmaGrenadeObject);
		}
		
		public InterruptPriority reloadInterruptPriority = InterruptPriority.Skill;
		public float graceDuration;
		public SerializableEntityStateType reloadState;

		public int stock = 8;



		// Token: 0x0400002B RID: 43
		public static GameObject PlasmaGrenadeObject;

		// Token: 0x0400002C RID: 44
		public GameObject PlasmaGrenadeGhostObject;

		// Token: 0x0400002D RID: 45
		public static List<GameObject> projectilePrefabs = new List<GameObject>();

		// Token: 0x0400002E RID: 46
		public static List<EffectDef> effectDefs = new List<EffectDef>();

		// Token: 0x0400002F RID: 47
		public static GameObject projectilePrefab;



	}
}
