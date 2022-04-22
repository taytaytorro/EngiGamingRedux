using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using EngiShotgun;
using EntityStates.EngiTurret.EngiTurretWeapon;
using EntityStates.Toolbot;
using R2API;
using RoR2;
using RoR2.Projectile;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000003 RID: 3
	public class PlasmaGrenades : AimThrowableBase
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000025B0 File Offset: 0x000007B0
		public static void AddConfig(ConfigFile config)
		{
			maxDist = config.Bind(configPrefix, "Max Distance", 60, "The maximum distance a grenade can travel.");
			damageCoeff = config.Bind(configPrefix, "Projectile Damage%", 500, "The base damage of each plasma grenade, in %");
			procCoefficient = config.Bind(configPrefix, "Proc Coefficient", 1f, "The multiplier for plasma grenade procs.");
			poolLifetime = config.Bind(configPrefix, "Plasma Lifetime", 5f, "The lifetime, in seconds,of each grenade's plasma pool.");
			glowIntensity = config.Bind(configPrefix, "Plasma Glow", 10f, "The glowing radius of a plasma pool");
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002656 File Offset: 0x00000856
		public static void Init(ConfigFile config)
		{
			AddConfig(config);
			GenerateProjectile();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002668 File Offset: 0x00000868
		private static void GenerateProjectile()
		{
			plasmaGrenadeObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/banditgrenadeprojectile"), "PlasmaGrenade", true);
			plasmaGhostObject = PrefabAPI.InstantiateClone(plasmaGrenadeObject.GetComponent<ProjectileController>().ghostPrefab, "PlasmaGrenadeGhost", false);
			GameObject gameObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocoleapacid"), "BasedPlasmaPuddle", true);
			GameObject gameObject2 = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion"), "PlasmaGrenadeBoomEffect", false);
			GameObject gameObject3 = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion");
			ProjectileDotZone component = gameObject.GetComponent<ProjectileDotZone>();
			ProjectileIntervalOverlapAttack projectileIntervalOverlapAttack = gameObject.AddComponent<ProjectileIntervalOverlapAttack>();
			ProjectileDamage component2 = gameObject.GetComponent<ProjectileDamage>();
			ProjectileController component3 = plasmaGrenadeObject.GetComponent<ProjectileController>();
			Destroy(plasmaGrenadeObject.GetComponent<ProjectileStickOnImpact>());
			projectileIntervalOverlapAttack.hitBoxGroup = gameObject.GetComponent<HitBoxGroup>();
			Light componentInChildren = gameObject.GetComponentInChildren<Light>();
			EffectComponent component4 = gameObject2.GetComponent<EffectComponent>();
			ProjectileImpactExplosion component5 = plasmaGrenadeObject.GetComponent<ProjectileImpactExplosion>();
			plasmaGrenadeObject.GetComponent<ProjectileDamage>().damageType = DamageType.SlowOnHit;
			plasmaGrenadeObject.GetComponentInChildren<Rigidbody>().useGravity = true;
			component3.ghostPrefab = plasmaGhostObject;
			component2.damageType = DamageType.SlowOnHit;
			component2.damage = (float)damageCoeff.Value * 0.1f;
			component2.force = 30f;
			projectileIntervalOverlapAttack.damageCoefficient = 0.2f;
			projectileIntervalOverlapAttack.interval = 0.5f;
			componentInChildren.color = Color.cyan;
			componentInChildren.range = glowIntensity.Value;
			gameObject.GetComponentInChildren<Decal>().Material.SetColor("_Color", Color.cyan);
			component.attackerFiltering = 0;
			component.overlapProcCoefficient = 0.15f;
			component.fireFrequency = 0.2f;
			component.lifetime = poolLifetime.Value;
			component.damageCoefficient = (float)(damageCoeff.Value / 2500);
			component5.blastRadius = 5f;
			component5.blastProcCoefficient = procCoefficient.Value;
			component5.falloffModel = BlastAttack.FalloffModel.Linear;
			component5.lifetime = 20f;
			component5.explosionEffect = gameObject3;
			component5.timerAfterImpact = false;
			component5.lifetimeAfterImpact = 0f;
			component5.destroyOnWorld = true;
			component5.destroyOnEnemy = true;
			component5.fireChildren = true;
			component5.childrenCount = 1;
			component5.childrenDamageCoefficient = 1f;
			component5.childrenProjectilePrefab = gameObject;
			projectilePrefabs.Add(plasmaGrenadeObject);
			projectilePrefabs.Add(gameObject);
			gameObject.AddComponent<NetworkBehaviour>();
			plasmaGrenadeObject.AddComponent<NetworkBehaviour>();
			gameObject3.AddComponent<NetworkIdentity>();
			plasmaGhostObject.AddComponent<NetworkIdentity>();
			plasmaGhostObject.AddComponent<NetworkBehaviour>();
			ContentAddition.AddProjectile(plasmaGrenadeObject);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002908 File Offset: 0x00000B08
		public override void OnEnter()
		{
			bool flag = goodState == null;
			bool flag2 = flag;
			if (flag2)
			{
				goodState = new AimStunDrone();
			}
			maxDistance = (float)maxDist.Value;
			rayRadius = 8.51f;
			arcVisualizerPrefab = goodState.arcVisualizerPrefab;
			projectilePrefab = plasmaGrenadeObject;
			endpointVisualizerPrefab = goodState.endpointVisualizerPrefab;
			endpointVisualizerRadiusScale = 4f;
			setFuse = false;
			damageCoefficient = (float)PlasmaGrenades.damageCoeff.Value / 100f;
			useGravity = true;
			baseMinimumDuration = 0.1f;
			projectileBaseSpeed = 35f;
			base.OnEnter();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000029C8 File Offset: 0x00000BC8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			characterBody.SetAimTimer(0.25f);
			fixedAge += Time.fixedDeltaTime;
			bool flag = false;
			bool flag2 = isAuthority && !KeyIsDown() && fixedAge >= minimumDuration;
			bool flag3 = flag2;
			bool flag4 = flag3;
			if (flag4)
			{
				flag = true;
			}
			bool flag5 = characterBody && characterBody.isSprinting;
			bool flag6 = flag5;
			bool flag7 = flag6;
			if (flag7)
			{
				flag = true;
			}
			bool flag8 = flag;
			bool flag9 = flag8;
			bool flag10 = flag9;
			if (flag10)
			{
				outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002A80 File Offset: 0x00000C80
		public override void OnExit()
		{
			base.OnExit();
			PlayCrossfade("Gesture, Additive", "FireMineRight", "FireMine.playbackRate", 0.3f, 0.05f);
			AddRecoil(0f, 0f, 0f, 0f);
			characterBody.AddSpreadBloom(1.75f);
			Util.PlaySound("Play_commando_M2_grenade_throw", gameObject);
			EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, gameObject, PlasmaGrenades.muzzleString, false);
		}

		// Token: 0x04000011 RID: 17
		public static ConfigEntry<int> maxDist;

		// Token: 0x04000012 RID: 18
		public static ConfigEntry<int> damageCoeff;

		// Token: 0x04000013 RID: 19
		public static ConfigEntry<float> glowIntensity;

		// Token: 0x04000014 RID: 20
		private static string configPrefix = PlasmaGrenadesDef.configPrefix;

		// Token: 0x04000015 RID: 21
		private static GameObject plasmaGrenadeObject;

		// Token: 0x04000016 RID: 22
		private static GameObject plasmaGhostObject;

		// Token: 0x04000017 RID: 23
		public static List<GameObject> projectilePrefabs = new List<GameObject>();

		// Token: 0x04000018 RID: 24
		public static List<EffectDef> effectDefs = new List<EffectDef>();

		// Token: 0x04000019 RID: 25
		public static ConfigEntry<float> procCoefficient;

		// Token: 0x0400001A RID: 26
		public static ConfigEntry<float> poolLifetime;

		// Token: 0x0400001B RID: 27
		private AimStunDrone goodState;

		// Token: 0x0400001C RID: 28
		public static string muzzleString = "MuzzleCenter";
	}
}
