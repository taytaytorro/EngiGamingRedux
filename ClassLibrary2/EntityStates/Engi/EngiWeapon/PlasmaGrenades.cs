using System;
using EntityStates.EngiTurret.EngiTurretWeapon;
using EntityStates.Toolbot;
using RoR2;
using UnityEngine;
using EngiPlugin = EngiShotgun.EngineerShotgunPlugin;
using BepInEx.Configuration;
using R2API;
using RoR2.Projectile;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000005 RID: 5
	public class PlasmaGrenades : AimThrowableBase
	{
		public static ConfigEntry<int> maxDist;
		public static ConfigEntry<int> damageCoeff;
		public static void AddConfig(ConfigFile config)
        {
			maxDist = config.Bind<int>("Plasma Grenade", "Max Distance", 60, "The maximum distance a grenade can travel.");
			damageCoeff = config.Bind<int>("Plasma Grenade", "Projectile Damage%", 500, "The base damage of each plasma grenade, in %");
			procCoefficient = config.Bind<float>("Plasma Grenade", "Proc Coefficient", 1, "The multiplier for plasma grenade procs.");
			poolLifetime = config.Bind<float>("Plasma Grenade", "Plasma Lifetime", 5f, "The lifetime, in seconds,of each grenade's plasma pool.");
		}
		public static void Init(ConfigFile config)
        {
			AddConfig(config);
			GenerateProjectile();
        }


		private static GameObject plasmaGrenadeObject;
		private static GameObject plasmaGhostObject;
		// Token: 0x0400002D RID: 45
		public static List<GameObject> projectilePrefabs = new List<GameObject>();

		// Token: 0x0400002E RID: 46
		public static List<EffectDef> effectDefs = new List<EffectDef>();

		public static ConfigEntry<float> procCoefficient;
		public static ConfigEntry<float> poolLifetime;

		private static void GenerateProjectile()
        {
			plasmaGrenadeObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/banditgrenadeprojectile"), "PlasmaGrenade", true);
			plasmaGhostObject = PrefabAPI.InstantiateClone(plasmaGrenadeObject.GetComponent<ProjectileController>().ghostPrefab, "PlasmaGrenadeGhost", false);
			projectilePrefabs.Add(plasmaGrenadeObject);
			plasmaGrenadeObject.GetComponent<ProjectileController>().ghostPrefab = plasmaGhostObject;
			plasmaGrenadeObject.AddComponent<NetworkBehaviour>();
			GameObject poolObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocoleapacid"), "BasedPlasmaPuddle", true);
			projectilePrefabs.Add(poolObject);
			poolObject.AddComponent<NetworkIdentity>();
			poolObject.AddComponent<NetworkBehaviour>();
			ProjectileDamage damageComponent = poolObject.GetComponent<ProjectileDamage>();
			damageComponent.damageType = DamageType.SlowOnHit;
			damageComponent.damage = damageCoeff.Value / 100f;
			damageComponent.force = 30f;
			ProjectileDotZone plasmaPool = poolObject.GetComponent<ProjectileDotZone>();
			plasmaPool.attackerFiltering = 0;
			plasmaPool.overlapProcCoefficient = 0.15f * procCoefficient.Value;
			plasmaPool.fireFrequency = 0.2f;
			plasmaPool.lifetime = poolLifetime.Value;
			plasmaPool.damageCoefficient = damageCoeff.Value /2500;
			GameObject effectObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion"), "PlasmaGrenadeBoomEffect", false);
			EffectComponent component3 = effectObject.GetComponent<EffectComponent>();
			ProjectileImpactExplosion plasmaBlast = plasmaGrenadeObject.GetComponent<ProjectileImpactExplosion>();
			plasmaBlast.blastRadius = 5.00f;
			plasmaGrenadeObject.GetComponent<ProjectileDamage>().damageType = DamageType.SlowOnHit;
			plasmaBlast.blastProcCoefficient = procCoefficient.Value;
			plasmaBlast.falloffModel = 0;
			plasmaBlast.lifetime = 20f;
			var impact = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion");

			//TODO REWRITE THIS AS A NETWORKED SOUND COMPONENT
			plasmaBlast.explosionSoundString = "Play_engi_M2_explo";
			//TODO REWRITE THIS AS A NETWORKED SOUND COMPONENT

			plasmaBlast.timerAfterImpact = false;
			plasmaBlast.lifetimeAfterImpact = 0f;
			plasmaBlast.destroyOnWorld = true;
			plasmaBlast.destroyOnEnemy = true;
			plasmaBlast.fireChildren = true;
			plasmaBlast.childrenCount = 1;
			plasmaBlast.childrenDamageCoefficient = 1f;
			plasmaBlast.childrenProjectilePrefab = poolObject;
			UnityEngine.Object.Destroy(plasmaGrenadeObject.GetComponent<ProjectileStickOnImpact>());
			plasmaGhostObject.AddComponent<NetworkIdentity>();
			plasmaGhostObject.AddComponent<NetworkBehaviour>();
			ContentAddition.AddProjectile(plasmaGrenadeObject);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000024F0 File Offset: 0x000006F0
		public override void OnEnter()
		{
			bool flag = this.goodState == null;
			if (flag)
			{
				this.goodState = new AimStunDrone();
			}
			maxDistance = maxDist.Value;
			rayRadius = 8.51f;
			arcVisualizerPrefab = this.goodState.arcVisualizerPrefab;
			projectilePrefab = plasmaGrenadeObject;
			endpointVisualizerPrefab = this.goodState.endpointVisualizerPrefab;
			endpointVisualizerRadiusScale = 4f;
			setFuse = false;
			damageCoefficient = damageCoeff.Value / 100f;
			useGravity = true;
			baseMinimumDuration = 0.1f;
			projectileBaseSpeed = 35f;
			base.OnEnter();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002598 File Offset: 0x00000798
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			base.characterBody.SetAimTimer(0.25f);
			base.fixedAge += Time.fixedDeltaTime;
			bool flag = false;
			bool flag2 = base.isAuthority && !this.KeyIsDown() && base.fixedAge >= this.minimumDuration;
			bool flag3 = flag2;
			if (flag3)
			{
				flag = true;
			}
			bool flag4 = base.characterBody && base.characterBody.isSprinting;
			bool flag5 = flag4;
			if (flag5)
			{
				flag = true;
			}
			bool flag6 = flag;
			bool flag7 = flag6;
			if (flag7)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000263C File Offset: 0x0000083C
		public override void OnExit()
		{
			base.OnExit();
			base.PlayCrossfade("Gesture, Additive", "FireMineRight", "FireMine.playbackRate", 0.3f, 0.05f);
			base.AddRecoil(0f, 0f, 0f, 0f);
			base.characterBody.AddSpreadBloom(1.75f);
			Util.PlaySound("Play_commando_M2_grenade_throw", base.gameObject);
			EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, base.gameObject, PlasmaGrenades.muzzleString, false);
		}

		// Token: 0x04000017 RID: 23
		private AimStunDrone goodState;

		// Token: 0x04000018 RID: 24
		public static string muzzleString = "MuzzleCenter";
	}
}
