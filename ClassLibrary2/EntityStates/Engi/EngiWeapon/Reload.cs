using System;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000004 RID: 4
	public class Reload : BaseState
	{
		// Token: 0x06000011 RID: 17 RVA: 0x00002B3C File Offset: 0x00000D3C
		public static void Init(ConfigFile config)
		{
			Reload.baseDuration = config.Bind<float>("Other", "Reload Speed", 3f, "Reload Duration of the Gauss Shotgun");
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002B5E File Offset: 0x00000D5E
		private float duration
		{
			get
			{
				return Reload.baseDuration.Value / this.attackSpeedStat;
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002B74 File Offset: 0x00000D74
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Gesture, Additive", (base.characterBody.isSprinting && base.characterMotor && base.characterMotor.isGrounded) ? "ReloadSimple" : "ChargeGrenades", "Reload.playbackRate", this.duration);
			Util.PlayAttackSpeedSound(Reload.enterSoundString, base.gameObject, Reload.enterSoundPitch);
			EffectManager.SimpleMuzzleFlash(Reload.reloadEffectPrefab, base.gameObject, Reload.reloadEffectMuzzleString, false);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002C00 File Offset: 0x00000E00
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = base.fixedAge >= this.duration / 2f;
			if (flag)
			{
				this.GiveStock();
			}
			bool flag2 = !base.isAuthority || base.fixedAge < this.duration;
			if (!flag2)
			{
				bool flag3 = base.skillLocator.primary.stock < base.skillLocator.primary.maxStock;
				if (flag3)
				{
					this.outer.SetNextState(new Reload());
				}
				else
				{
					Util.PlaySound(ChargeGrenades.chargeStockSoundString, base.gameObject);
					this.outer.SetNextStateToMain();
				}
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002CB0 File Offset: 0x00000EB0
		public override void OnExit()
		{
			Util.PlaySound(ChargeGrenades.chargeLoopStopSoundString, base.gameObject);
			base.OnExit();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002CCC File Offset: 0x00000ECC
		private void GiveStock()
		{
			bool flag = this.hasGivenStock;
			if (!flag)
			{
				base.skillLocator.primary.AddOneStock();
				this.hasGivenStock = true;
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002D00 File Offset: 0x00000F00
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x0400001D RID: 29
		public static ConfigEntry<float> baseDuration;

		// Token: 0x0400001E RID: 30
		public static float enterSoundPitch;

		// Token: 0x0400001F RID: 31
		public static float exitSoundPitch;

		// Token: 0x04000020 RID: 32
		public static string enterSoundString;

		// Token: 0x04000021 RID: 33
		public static string exitSoundString;

		// Token: 0x04000022 RID: 34
		public static GameObject reloadEffectPrefab;

		// Token: 0x04000023 RID: 35
		public static string reloadEffectMuzzleString;

		// Token: 0x04000024 RID: 36
		private bool hasGivenStock;
	}
}
