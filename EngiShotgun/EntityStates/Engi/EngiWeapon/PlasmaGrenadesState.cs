using System;
using EntityStates.EngiTurret.EngiTurretWeapon;
using EntityStates.Toolbot;
using RoR2;
using UnityEngine;
using BepInEx.Configuration;
using R2API;
using RoR2.Projectile;
using UnityEngine.Networking;
using System.Collections.Generic;
using EngiShotgun;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000005 RID: 5
	public class PlasmaGrenadesState : AimThrowableBase
	{
		public static ConfigEntry<int> maxDist;
		public static ConfigEntry<int> damageCoeff;
		public static ConfigEntry<float> glowAmount;
		static string configPrefix = PlasmaGrenadesSlot.configPrefix;
		public static void AddConfig(ConfigFile config)
        {
			maxDist = config.Bind<int>(configPrefix, "Max Distance", 60, "The maximum distance a grenade can travel.");
			damageCoeff = config.Bind<int>(configPrefix, "Projectile Damage%", 500, "The base damage of each plasma grenade, in %");
			procCoefficient = config.Bind<float>(configPrefix, "Proc Coefficient", 1, "The multiplier for plasma grenade procs.");
			poolLifetime = config.Bind<float>(configPrefix, "Plasma Lifetime", 5f, "The lifetime, in seconds,of each grenade's plasma pool.");
			glowAmount = config.Bind(configPrefix, "Plasma Glow", 15f, "The power of the Plasma Pool's glow.");
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
			//instantiated clones
			plasmaGrenadeObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/banditgrenadeprojectile"), "PlasmaGrenade", true);
			plasmaGhostObject = PrefabAPI.InstantiateClone(plasmaGrenadeObject.GetComponent<ProjectileController>().ghostPrefab, "PlasmaGrenadeGhost", false);
			GameObject poolObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocoleapacid"), "BasedPlasmaPuddle", true);
			GameObject effectObject = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion"), "PlasmaGrenadeBoomEffect", false);

			//add projectile prefab to list
			projectilePrefabs.Add(plasmaGrenadeObject);
			projectilePrefabs.Add(poolObject);

			//get components
			plasmaGrenadeObject.GetComponent<ProjectileController>().ghostPrefab = plasmaGhostObject;
			plasmaGrenadeObject.GetComponent<Rigidbody>().useGravity = true;
			ProjectileDamage damageComponent = poolObject.GetComponent<ProjectileDamage>();
			ProjectileImpactExplosion plasmaBlast = plasmaGrenadeObject.GetComponent<ProjectileImpactExplosion>();

			//reassign DOT to network-compatible 
			var oldDOT = poolObject.GetComponent<ProjectileDotZone>();
			PlasmaDOTZone plasmaPool = poolObject.AddComponent<PlasmaDOTZone>();

			//value copying
			plasmaPool.impactEffect = effectObject;
			plasmaPool.onEnd = oldDOT.onEnd;
			plasmaPool.attackerFiltering = 0;
			plasmaPool.overlapProcCoefficient = 0.15f * procCoefficient.Value;
			plasmaPool.fireFrequency = 5f;
			plasmaPool.resetFrequency = 20f;
			plasmaPool.lifetime = poolLifetime.Value;
			plasmaPool.damageCoefficient = damageCoeff.Value * 0.02f / (plasmaPool.fireFrequency * plasmaPool.resetFrequency);

			//destroy network-incompat DOT
			Destroy(oldDOT);


			//damage values
			plasmaGrenadeObject.GetComponent<ProjectileDamage>().damageType = DamageType.SlowOnHit;
			damageComponent.damageType = DamageType.SlowOnHit;
			damageComponent.damage = damageCoeff.Value / 100f;
			damageComponent.force = 30f;

			//plasma pool values
			var light = poolObject.GetComponentInChildren<Light>();
			light.color = Color.cyan;
			light.intensity = light.range = glowAmount.Value;
			//decal values
			var decal = poolObject.GetComponentInChildren<ThreeEyedGames.Decal>();
			decal.Material.SetColor("_Color", Color.cyan);

			//plasma blast values
			plasmaBlast.blastRadius = 5.00f;
			plasmaBlast.blastProcCoefficient = procCoefficient.Value;
			plasmaBlast.falloffModel = 0;
			plasmaBlast.lifetime = 20f;
			plasmaBlast.timerAfterImpact = false;
			plasmaBlast.lifetimeAfterImpact = 0f;
			plasmaBlast.destroyOnWorld = true;
			plasmaBlast.destroyOnEnemy = true;
			plasmaBlast.fireChildren = true;
			plasmaBlast.childrenCount = 1;
			plasmaBlast.childrenDamageCoefficient = 1f;
			plasmaBlast.childrenProjectilePrefab = poolObject;

			//destroy sticky impact
			UnityEngine.Object.Destroy(plasmaGrenadeObject.GetComponent<ProjectileStickOnImpact>());

			//add network comps
			plasmaGrenadeObject.AddComponent<NetworkBehaviour>();
			plasmaGhostObject.AddComponent<NetworkIdentity>();
			plasmaGhostObject.AddComponent<NetworkBehaviour>();
			poolObject.AddComponent<NetworkBehaviour>();
			ContentAddition.AddProjectile(plasmaGrenadeObject);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000024F0 File Offset: 0x000006F0
		public override void OnEnter()
		{

			bool flag = goodState == null;
			if (flag)
			{
				goodState = new AimStunDrone();
			}
			setFuse = true;
			maxDistance = maxDist.Value;
			rayRadius = 8.51f;
			arcVisualizerPrefab = goodState.arcVisualizerPrefab;
			projectilePrefab = plasmaGrenadeObject;
			endpointVisualizerPrefab = goodState.endpointVisualizerPrefab;
			endpointVisualizerRadiusScale = 4f;
			setFuse = false;
			damageCoefficient = damageCoeff.Value / 100f;
			useGravity = true;
			baseMinimumDuration = 0.1f;
			projectileBaseSpeed = 35f;
			base.OnEnter();
		}

        public override void FireProjectile()
        {
            base.FireProjectile();
        }
        public override void FixedUpdate()
		{
			base.FixedUpdate();
			if(isAuthority)
            {
				UpdateTrajectoryInfo(out currentTrajectoryInfo);
            }
			characterBody.SetAimTimer(0.25f);
			fixedAge += Time.fixedDeltaTime;
			bool flag = false;
			bool flag2 = isAuthority && !KeyIsDown() && fixedAge >= minimumDuration;
			if (flag2)
			{
				flag = true;
			}
			bool flag4 = characterBody && characterBody.isSprinting;
			if (flag4)
			{
				flag = true;
			}
			if (flag)
			{
				outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000263C File Offset: 0x0000083C
		public override void OnExit()
		{
			base.OnExit();
			PlayCrossfade("Gesture, Additive", "FireMineRight", "FireMine.playbackRate", 0.3f, 0.05f);
			AddRecoil(0f, 0f, 0f, 0f);
			characterBody.AddSpreadBloom(1.75f);
			Util.PlaySound("Play_commando_M2_grenade_throw", gameObject);
			EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, gameObject, muzzleString, false);
		}

		// Token: 0x04000017 RID: 23
		private AimStunDrone goodState;

		// Token: 0x04000018 RID: 24
		public static string muzzleString = "MuzzleCenter";
	}
}
