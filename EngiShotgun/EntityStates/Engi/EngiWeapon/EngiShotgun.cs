using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.EngiTurret.EngiTurretWeapon;
using BepInEx.Configuration;
using EngiShotgun;
using RoR2.UI;
using UnityEngine.UI;
using static RoR2.UI.CrosshairController;
using static EngiShotgun.EngineerShotgunPlugin;

namespace EntityStates.Engi.EngiWeapon
{
	public class EngiShotgun : BaseState
	{
		static readonly string configPrefix = EngiShotgunSlot.configPrefix;
		public static ConfigEntry<float> minSpread;
		public static ConfigEntry<float> maxSpread;
		public static ConfigEntry<float> procCoefficient;
		public static ConfigEntry<uint> bulletCount;
		public static ConfigEntry<float> shotgunBloom;

		public static uint bulletCountLoad;
		
		public static void AddConfig(ConfigFile config)
        {
			minSpread = config.Bind(configPrefix, "Min Spread", 1.2f, "The smallest possible angle between a shotgun projectile and the crosshair.");
			maxSpread = config.Bind(configPrefix, "Max Spread", 2.2f, "The largest possible angle between a shotgun projectile and the crosshair.");
			procCoefficient = config.Bind(configPrefix, "Proc Coeffient%", 0.45f, "The proc coefficient multiplier of each shotgun projectile.");
			shotgunBloom = config.Bind(configPrefix, "Shotgun Bloom", 0.5f, "The temporary inaccuracy added by each round fired.");
			bulletCount = config.Bind(configPrefix, "Bullet Count", 8U, "The bullet count of each shotgun blast.");
            {
				bulletCountLoad = (uint) Mathf.CeilToInt(bulletCount.Value / 2);
            }

		}
		public static void Init(ConfigFile config)
        {
			AddConfig(config);
        }

		public override void OnEnter()
		{
			base.OnEnter();

			maxDuration = baseMaxDuration / attackSpeedStat;
			minDuration = baseMaxDuration / attackSpeedStat;

			Util.PlaySound(FireGauss.attackSoundString, gameObject);

			characterBody.skillLocator.primary.rechargeStopwatch = 0f;


			muzzleString = "MuzzleRight";
			muzzleString2 = "MuzzleLeft";


			PlayAnimation("Gesture Right Cannon, Additive", "FireGrenadeRight", "FireGrenadeRight.playbackRate", maxDuration);
			PlayAnimation("Gesture Left Cannon, Additive", "FireGrenadeLeft", "FireGrenadeLeft.playbackRate", maxDuration);

			characterBody.AddSpreadBloom(shotgunBloom.Value);

			if (muzzleEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, gameObject, muzzleString, false);
				EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, gameObject, muzzleString2, false);
			}
			if (isAuthority)
			{
				Ray aimRay = GetAimRay();
				AddRecoil(-4.5f, -1f, -1f, 4f);
				var attack = new BulletAttack
				{
					queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
					bulletCount = bulletCountLoad,
					aimVector = aimRay.direction,
					origin = aimRay.origin,
					damage = EngiShotgunSlot.damageCoeff.Value * damageStat * 0.01f,

					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,

					force = 9f,
					hitMask = LayerIndex.CommonMasks.bullet,

					minSpread = minSpread.Value,
					maxSpread = maxSpread.Value,

					isCrit = Util.CheckRoll(critStat, characterBody.master),
					owner = characterBody.gameObject,
					smartCollision = false,
					procChainMask = default(ProcChainMask),
					procCoefficient = procCoefficient.Value,
					radius = 0.53f,
					sniper = false,	
					stopperMask = LayerIndex.CommonMasks.bullet,

					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					hitEffectPrefab = FireGauss.hitEffectPrefab
				};
				attack.muzzleName = muzzleString;
				attack.Fire();
				attack.muzzleName = muzzleString2;
				attack.Fire();
				shotfired = true;
				
			}
		}
		public override void OnExit()
		{
			if (!buttonReleased && characterBody)
			{
				characterBody.SetSpreadBloom(0f, false);
			}
			base.OnExit();
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (shotfired == true)
            {
				//skillLocator.primary.DeductStock(1);
				shotfired = false;
			}
			//base.FixedUpdate();
			buttonReleased |= !inputBank.skill1.down;
			if (fixedAge >= maxDuration && isAuthority)
			{
				outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (buttonReleased && fixedAge >= minDuration)
			{
				return InterruptPriority.Any;
			}
			return InterruptPriority.PrioritySkill;
		}

		public static GameObject muzzleEffectPrefab;
		public static GameObject tracerEffectPrefab;
		public static GameObject impactEffectPrefab;
		public static float bulletRadius = 0.4f;
		public static float baseMaxDuration = 0.1f;
		public static float baseMinDuration = 0.1f;
        private float maxDuration;
		private float minDuration;
		private bool buttonReleased;
		private bool shotfired;
		private string muzzleString;
		private string muzzleString2;
    }

}




