using System;
using EntityStates.Engi.EngiWeapon.Reload;
using EntityStates.EngiTurret.EngiTurretWeapon;
using EntityStates.Mage.Weapon;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon.Rux1
{
	// Token: 0x02000004 RID: 4
	public class GaussShotgun : BaseState
	{
		// Token: 0x0600000E RID: 14 RVA: 0x000022C4 File Offset: 0x000004C4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = 0.7f / this.attackSpeedStat;
			base.characterBody.SetAimTimer(2f);
			this.muzzleString = "MuzzleRight";
			this.muzzleString2 = "MuzzleLeft";
			base.PlayAnimation("Gesture Right, Additive", "FireGrenadeRight", "FireGauntlet.playbackRate", this.duration);
			base.PlayAnimation("Gesture Left, Additive", "FireGrenadeLeft", "FireGauntlet.playbackRate", this.duration);
			this.FireGauntlet();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000234B File Offset: 0x0000054B
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002354 File Offset: 0x00000554
		private void FireGauntlet()
		{
			base.characterBody.AddSpreadBloom(0.5f);
			EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, base.gameObject, this.muzzleString, false);
			Util.PlaySound(FireGauss.attackSoundString, base.gameObject);
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				base.AddRecoil(-4.5f, -1f, -1f, 4f);
				new BulletAttack
				{
					bulletCount = 4U,
					aimVector = aimRay.direction,
					origin = aimRay.origin,
					damage = 0.6f * this.damageStat,
					damageColorIndex = 0,
					damageType = 0,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					force = 9f,
					hitMask = LayerIndex.CommonMasks.bullet,
					minSpread = 2.2f,
					maxSpread = 2.2f,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					owner = base.gameObject,
					muzzleName = this.muzzleString2,
					smartCollision = false,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0.45f,
					radius = 0.53f,
					sniper = false,
					stopperMask = LayerIndex.CommonMasks.bullet,
					weapon = null,
					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					queryTriggerInteraction = 0,
					hitEffectPrefab = FireGauss.hitEffectPrefab
				}.Fire();
				Ray aimRay2 = base.GetAimRay();
				base.AddRecoil(-4.5f, -1f, -1f, 4f);
				new BulletAttack
				{
					bulletCount = 4U,
					aimVector = aimRay2.direction,
					origin = aimRay2.origin,
					damage = 0.6f * this.damageStat,
					damageColorIndex = 0,
					damageType = 0,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					force = 9f,
					hitMask = LayerIndex.CommonMasks.bullet,
					minSpread = 2.2f,
					maxSpread = 2.2f,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					owner = base.gameObject,
					muzzleName = this.muzzleString,
					smartCollision = false,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0.45f,
					radius = 0.53f,
					sniper = false,
					stopperMask = LayerIndex.CommonMasks.bullet,
					weapon = null,
					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					queryTriggerInteraction = 0,
					hitEffectPrefab = FireGauss.hitEffectPrefab
				}.Fire();
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002608 File Offset: 0x00000808

		// Token: 0x06000012 RID: 18 RVA: 0x00002639 File Offset: 0x00000839
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000005 RID: 5
		public static float procCoefficient = 0.45f;

		// Token: 0x04000006 RID: 6
		public static float force = 20f;

		// Token: 0x04000007 RID: 7
		public static float recoil = 1.5f;

		// Token: 0x04000008 RID: 8
		public static float range = 256f;

		// Token: 0x04000009 RID: 9
		private float duration;

		// Token: 0x0400000A RID: 10
		private string muzzleString;

		// Token: 0x0400000B RID: 11
		private string muzzleString2;

		// Token: 0x0400000C RID: 12
		public static GameObject muzzleEffectPrefab;

		// Token: 0x0400000D RID: 13
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400000E RID: 14
		public static GameObject impactEffectPrefab;

		// Token: 0x0400000F RID: 15
		public static float baseDuration = 2f;

		// Token: 0x04000010 RID: 16
		public static float damageCoefficient = 1.2f;

		// Token: 0x04000011 RID: 17
		public static string attackString;

		// Token: 0x04000012 RID: 18
		private bool hasFiredGauntlet;

		// Token: 0x04000013 RID: 19
		private Animator animator;

		// Token: 0x04000014 RID: 20
		public FireLaserbolt.Gauntlet gauntlet;

		// Token: 0x04000015 RID: 21
		[SerializeField]
		public GameObject crosshairOverridePrefab = Resources.Load<GameObject>("Prefabs/Crosshair/BanditCrosshair");

		// Token: 0x04000016 RID: 22
		private GameObject defaultCrosshairPrefab = Resources.Load<GameObject>("Prefabs/Crosshair/BanditCrosshair");

		// Token: 0x02000008 RID: 8
		public enum Gauntlet
		{
			// Token: 0x0400002E RID: 46
			Left,
			// Token: 0x0400002F RID: 47
			Right
		}
	}
}