using System;
using BepInEx.Configuration;
using EntityStates.EngiTurret.EngiTurretWeapon;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000002 RID: 2
	public class GaussShotgun : BaseState
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static void AddConfig(ConfigFile config)
		{
			minSpread = config.Bind("Gauss Shotgun", "Min Spread", 1.2f, "The smallest possible angle between a shotgun projectile and the crosshair.");
			maxSpread = config.Bind("Gauss Shotgun", "Max Spread", 2.2f, "The largest possible angle between a shotgun projectile and the crosshair.");
			damageCoeff = config.Bind("Gauss Shotgun", "Projectile Damage%", 60, "The base damage of each shotgun projectile, in %");
			procCoefficient = config.Bind("Gauss Shotgun", "Proc Coeffient%", 0.45f, "The proc coefficient multiplier of each shotgun projectile.");
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020D7 File Offset: 0x000002D7
		public static void Init(ConfigFile config)
		{
			GaussShotgun.AddConfig(config);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020E4 File Offset: 0x000002E4
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
			bool flag = muzzleEffectPrefab;
			if (flag)
			{
				EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, gameObject, muzzleString, false);
				EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, gameObject, muzzleString2, false);
			}
			bool isAuthority = base.isAuthority;
			if (isAuthority)
			{
				Ray aimRay = GetAimRay();
				AddRecoil(-4.5f, -1f, -1f, 4f);
				BulletAttack bulletAttack = new BulletAttack
				{
					queryTriggerInteraction = 0,
					bulletCount = 4U,
					aimVector = aimRay.direction,
					origin = aimRay.origin,
					damage = (float)damageCoeff.Value * damageStat / 100f,
					damageColorIndex = 0,
					damageType = 0,
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
				bulletAttack.Fire();
				Ray aimRay2 = GetAimRay();
				AddRecoil(-4.5f, -1f, -1f, 4f);
				bulletAttack = new BulletAttack
				{
					queryTriggerInteraction = 0,
					bulletCount = 4U,
					aimVector = aimRay2.direction,
					origin = aimRay2.origin,
					damage = (float)damageCoeff.Value * damageStat / 100f,
					damageColorIndex = 0,
					damageType = 0,
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
				bulletAttack.Fire();
				shotfired = true;
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002494 File Offset: 0x00000694
		public override void OnExit()
		{
			bool flag = !buttonReleased && characterBody;
			if (flag)
			{
				characterBody.SetSpreadBloom(0f, false);
			}
			base.OnExit();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000024D8 File Offset: 0x000006D8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = shotfired;
			if (flag)
			{
				shotfired = false;
			}
			buttonReleased |= !inputBank.skill1.down;
			bool flag2 = fixedAge >= maxDuration && isAuthority;
			if (flag2)
			{
				outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000254C File Offset: 0x0000074C
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			bool flag = buttonReleased && fixedAge >= minDuration;
			InterruptPriority result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = InterruptPriority.PrioritySkill;
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		public static ConfigEntry<float> minSpread;

		// Token: 0x04000002 RID: 2
		public static ConfigEntry<float> maxSpread;

		// Token: 0x04000003 RID: 3
		public static ConfigEntry<int> damageCoeff;

		// Token: 0x04000004 RID: 4
		public static ConfigEntry<float> procCoefficient;

		// Token: 0x04000005 RID: 5
		public static GameObject muzzleEffectPrefab;

		// Token: 0x04000006 RID: 6
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000007 RID: 7
		public static GameObject impactEffectPrefab;

		// Token: 0x04000008 RID: 8
		public static float bulletRadius = 0.4f;

		// Token: 0x04000009 RID: 9
		public static float baseMaxDuration = 0.7f;

		// Token: 0x0400000A RID: 10
		public static float baseMinDuration = 0.7f;

		// Token: 0x0400000B RID: 11
		private float maxDuration;

		// Token: 0x0400000C RID: 12
		private float minDuration;

		// Token: 0x0400000D RID: 13
		private bool buttonReleased;

		// Token: 0x0400000E RID: 14
		private bool shotfired;

		// Token: 0x0400000F RID: 15
		private string muzzleString;

		// Token: 0x04000010 RID: 16
		private string muzzleString2;
	}
}
