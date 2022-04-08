using System;
using EntityStates.Huntress.HuntressWeapon;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Rogue.Weapon43
{
	// Token: 0x02000003 RID: 3
	public class SecretFire : BaseSkillState
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002184 File Offset: 0x00000384
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = SecretFire.baseDuration / this.attackSpeedStat;
			this.speed = Util.Remap(this.charge, 0f, 1f, SecretFire.minSpeed, SecretFire.maxSpeed);
			this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, SecretFire.minDamageCoefficient, SecretFire.maxDamageCoefficient);
			this.recoil = Util.Remap(this.charge, 0f, 1f, SecretFire.minRecoil, SecretFire.maxRecoil);
			this.hasFired = false;
			this.Fire();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002228 File Offset: 0x00000428
		private void Fire()
		{
			bool flag = !this.hasFired;
			bool flag2 = flag;
			if (flag2)
			{
				this.hasFired = true;
				EffectManager.SimpleMuzzleFlash(FireArrow.effectPrefab, base.gameObject, "Muzzle", false);
				bool isAuthority = base.isAuthority;
				bool flag3 = isAuthority;
				if (flag3)
				{
					base.AddRecoil(-1f * this.recoil, -2f * this.recoil, -0.5f * this.recoil, 0.5f * this.recoil);
					Ray aimRay = base.GetAimRay();
					ProjectileManager.instance.FireProjectile(FireArrow.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageCoefficient * this.damageStat, 2000f, base.RollCrit(), 0, null, this.speed);
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002304 File Offset: 0x00000504
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = base.fixedAge >= this.duration && base.isAuthority;
			bool flag2 = flag;
			if (flag2)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002344 File Offset: 0x00000544
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002350 File Offset: 0x00000550
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000003 RID: 3
		public float charge;

		// Token: 0x04000004 RID: 4
		public static float baseDuration = 0.7f;

		// Token: 0x04000005 RID: 5
		public static float minSpeed = 10f;

		// Token: 0x04000006 RID: 6
		public static float maxSpeed = 300f;

		// Token: 0x04000007 RID: 7
		public static float minDamageCoefficient = 3f;

		// Token: 0x04000008 RID: 8
		public static float maxDamageCoefficient = 12f;

		// Token: 0x04000009 RID: 9
		public static float minRecoil = 0.5f;

		// Token: 0x0400000A RID: 10
		public static float maxRecoil = 5f;

		// Token: 0x0400000B RID: 11
		private float duration;

		// Token: 0x0400000C RID: 12
		private float speed;

		// Token: 0x0400000D RID: 13
		private float damageCoefficient;

		// Token: 0x0400000E RID: 14
		private float recoil;

		// Token: 0x0400000F RID: 15
		private bool hasFired;
	}
}
