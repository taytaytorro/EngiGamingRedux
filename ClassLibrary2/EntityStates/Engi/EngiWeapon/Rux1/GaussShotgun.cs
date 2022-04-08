using System;
using EntityStates.EngiTurret.EngiTurretWeapon;
using EntityStates.Mage.Weapon;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon.Rux1
{
	// Token: 0x02000006 RID: 6
	public class GaussShotgun : BaseState
	{
		// Token: 0x0600001C RID: 28 RVA: 0x000026DC File Offset: 0x000008DC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = 0.7f / attackSpeedStat;
			FireLaserbolt.Gauntlet gauntlet = this.gauntlet;
			bool flag = gauntlet > 0;
			if (flag)
			{
				bool flag2 = gauntlet > 0;
				if (flag2)
				{
					this.muzzleString = "MuzzleRight";
					base.PlayAnimation("Gesture Right, Additive", "FireGrenadeRight", "FireGauntlet.playbackRate", this.duration);
				}
			}
			else
			{
				this.muzzleString = "MuzzleLeft";
				base.PlayAnimation("Gesture Left, Additive", "FireGrenadeLeft", "FireGauntlet.playbackRate", this.duration);
			}
			base.PlayAnimation("Gesture, Additive", "HoldGauntletsUp", "FireGauntlet.playbackRate", this.duration);
			Util.PlaySound(FireGauss.attackSoundString, base.gameObject);
			this.animator = base.GetModelAnimator();
			base.characterBody.SetAimTimer(2f);
			bool flag3 = base.characterBody && base.characterBody.isSprinting;
			bool flag4 = flag3;
			if (flag4)
			{
				base.characterBody.isSprinting = false;
			}
			this.FireGauntlet();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000027F4 File Offset: 0x000009F4
		public override void OnExit()
		{
			base.OnExit();
			bool flag = base.characterBody && base.characterBody.isSprinting;
			bool flag2 = flag;
			if (flag2)
			{
				base.characterBody.isSprinting = false;
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000283C File Offset: 0x00000A3C
		private void FireGauntlet()
		{
			this.hasFiredGauntlet = true;
			Ray aimRay = base.GetAimRay();
			bool flag = FireGauss.effectPrefab;
			if (flag)
			{
				EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, base.gameObject, this.muzzleString, false);
			}
			bool isAuthority = base.isAuthority;
			if (isAuthority)
			{
				base.characterBody.AddSpreadBloom(0.56f);
				this.defaultCrosshairPrefab = base.characterBody.defaultCrosshairPrefab;
				bool flag2 = this.crosshairOverridePrefab;
				if (flag2)
				{
					RoR2.UI.CrosshairUtils.RequestOverrideForBody(this.characterBody, crosshairOverridePrefab, RoR2.UI.CrosshairUtils.OverridePriority.Sprint);
				}
				base.AddRecoil(-9f, -2f, -2f, 8f);
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = 2.2f,
					maxSpread = 2.2f,
					damage = 0.6f * this.damageStat,
					procCoefficient = 0.45f,
					force = 9f,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					muzzleName = this.muzzleString,
					hitEffectPrefab = FireGauss.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					radius = 0.53f,
					bulletCount = 8U,
					smartCollision = false
				}.Fire();
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000029C8 File Offset: 0x00000BC8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = this.animator.GetFloat("FireGauntlet.fire") > 0f && !this.hasFiredGauntlet;
			if (flag)
			{
				this.FireGauntlet();
			}
			bool flag2 = base.fixedAge < this.duration || !base.isAuthority;
			if (!flag2)
			{
				bool down = base.inputBank.skill1.down;
				if (down)
				{
                    GaussShotgun gaussShotgun = new GaussShotgun
                    {
                        gauntlet = (false) ? Mage.Weapon.FireLaserbolt.Gauntlet.Right : Mage.Weapon.FireLaserbolt.Gauntlet.Left
                    };
                    bool flag3 = base.characterBody && base.characterBody.isSprinting;
					bool flag4 = flag3;
					if (flag4)
					{
						base.characterBody.isSprinting = false;
					}
					this.outer.SetNextState(gaussShotgun);
				}
				else
				{
					this.outer.SetNextStateToMain();
				}
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002AAC File Offset: 0x00000CAC
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000019 RID: 25
		public static GameObject muzzleEffectPrefab;

		// Token: 0x0400001A RID: 26
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400001B RID: 27
		public static GameObject impactEffectPrefab;

		// Token: 0x0400001C RID: 28
		public static float baseDuration = 2f;

		// Token: 0x0400001D RID: 29
		public static float damageCoefficient = 1.2f;

		// Token: 0x0400001E RID: 30
		public static float force = 20f;

		// Token: 0x0400001F RID: 31
		public static string attackString;

		// Token: 0x04000020 RID: 32
		private float duration;

		// Token: 0x04000021 RID: 33
		private bool hasFiredGauntlet;

		// Token: 0x04000022 RID: 34
		private string muzzleString;

		// Token: 0x04000023 RID: 35
		private Animator animator;

		// Token: 0x04000024 RID: 36
		public FireLaserbolt.Gauntlet gauntlet;

		// Token: 0x04000025 RID: 37
		[SerializeField]
		public GameObject crosshairOverridePrefab = Resources.Load<GameObject>("Prefabs/Crosshair/BanditCrosshair");

		// Token: 0x04000026 RID: 38
		private GameObject defaultCrosshairPrefab = Resources.Load<GameObject>("Prefabs/Crosshair/BanditCrosshair");

		// Token: 0x02000009 RID: 9
		public enum Gauntlet
		{
			// Token: 0x04000031 RID: 49
			Left,
			// Token: 0x04000032 RID: 50
			Right
		}
	}
}
