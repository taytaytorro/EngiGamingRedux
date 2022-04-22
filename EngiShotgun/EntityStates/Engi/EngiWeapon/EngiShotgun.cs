using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.EngiTurret.EngiTurretWeapon;
using BepInEx.Configuration;

namespace EntityStates.Engi.EngiWeapon
{
	public class EngiShotgun : BaseState
	{
		public static ConfigEntry<float> minSpread;
		public static ConfigEntry<float> maxSpread;
		public static ConfigEntry<int> damageCoeff;
		public static ConfigEntry<float> procCoefficient;
		public static void AddConfig(ConfigFile config)
        {
			minSpread = config.Bind<float>("Gauss Shotgun", "Min Spread", 1.2f, "The smallest possible angle between a shotgun projectile and the crosshair.");
			maxSpread = config.Bind<float>("Gauss Shotgun", "Max Spread", 2.2f, "The largest possible angle between a shotgun projectile and the crosshair.");
			damageCoeff = config.Bind<int>("Gauss Shotgun", "Projectile Damage%", 60, "The base damage of each shotgun projectile, in %");
			procCoefficient = config.Bind<float>("Gauss Shotgun", "Proc Coeffient%", .45f, "The proc coefficient multiplier of each shotgun projectile.");
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
			PlayAnimation("Gesture Right, Additive", "FireGrenadeRight", "FireGrenadeRight.playbackRate", maxDuration);
			PlayAnimation("Gesture Left, Additive", "FireGrenadeLeft", "FireGrenadeLeft.playbackRate", maxDuration);
			characterBody.AddSpreadBloom(0.5f);

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
					bulletCount = 4U,
					aimVector = aimRay.direction,
					origin = aimRay.origin,
					damage = damageCoeff.Value * damageStat / 100f,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					force = 9f,
					hitMask = LayerIndex.CommonMasks.bullet,
					minSpread = minSpread.Value,
					maxSpread = maxSpread.Value,
					isCrit = Util.CheckRoll(critStat, characterBody.master),
					owner = gameObject,
					muzzleName = muzzleString2,
					smartCollision = false,
					procChainMask = default(ProcChainMask),
					procCoefficient = procCoefficient.Value,
					radius = 0.53f,
					sniper = false,	
					stopperMask = LayerIndex.CommonMasks.bullet,
					weapon = null,
					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					hitEffectPrefab = FireGauss.hitEffectPrefab
				};
				attack.Fire();
				Ray aimRay2 = GetAimRay();
				base.AddRecoil(-4.5f, -1f, -1f, 4f);
				attack = new BulletAttack
				{
					queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
					bulletCount = 4U,
					aimVector = aimRay2.direction,
					origin = aimRay2.origin,
					damage = damageCoeff.Value * damageStat / 100f,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					force = 9f,
					hitMask = LayerIndex.CommonMasks.bullet,
					minSpread = 2.2f,
					maxSpread = 2.2f,
					isCrit = Util.CheckRoll(critStat, characterBody.master),
					owner = gameObject,
					muzzleName = muzzleString,
					smartCollision = false,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0.45f,
					radius = 0.53f,
					sniper = false,
					stopperMask = LayerIndex.CommonMasks.bullet,
					weapon = null,
					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					hitEffectPrefab = FireGauss.hitEffectPrefab
				};
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
		public static float baseMaxDuration = 0.7f;
		public static float baseMinDuration = 0.7f;
		private float maxDuration;
		private float minDuration;
		private bool buttonReleased;
		private bool shotfired;
		private string muzzleString;
		private string muzzleString2;
	}

}




