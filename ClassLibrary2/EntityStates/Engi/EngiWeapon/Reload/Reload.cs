using System;
using RoR2;
using UnityEngine;
using EntityStates.Engi.EngiWeapon.Rux1;
using EngiShotgu;

namespace EntityStates.Engi.EngiWeapon.Reload
{
	// Token: 0x02000479 RID: 1145
	public class Reload : BaseState
	{
		// Token: 0x1700012E RID: 302
		// (get) Token: 0x0600147D RID: 5245 RVA: 0x0005B2FC File Offset: 0x000594FC
		private static float UsedStock;

		public static float usedStock
		{
			get { return UsedStock; }
			set { UsedStock = value; }

		}
		/*private float Duration
		{
			get
			{
				return Reload.baseDuration / this.attackSpeedStat;
			}
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x0005B30C File Offset: 0x0005950C
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("Gesture, Additive", (base.characterBody.isSprinting && base.characterMotor && base.characterMotor.isGrounded) ? "ChargeGrenades" : "Reload", "Reload.playbackRate", this.Duration);
			Util.PlayAttackSpeedSound(Reload.enterSoundString, base.gameObject, Reload.enterSoundPitch);
			EffectManager.SimpleMuzzleFlash(Reload.reloadEffectPrefab, base.gameObject, Reload.reloadEffectMuzzleString, false);
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x0005B394 File Offset: 0x00059594
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.Duration / .25f)
			{
				this.GiveStock();
			}
			if (!base.isAuthority || base.fixedAge < this.Duration)
			{
				return;
			}
			if (base.skillLocator.primary.stock < base.skillLocator.primary.maxStock)
			{
				this.outer.SetNextState(new Reload());
				return;
			}
			Util.PlayAttackSpeedSound(Reload.exitSoundString, base.gameObject, Reload.exitSoundPitch);
			this.outer.SetNextStateToMain();
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x0000EBED File Offset: 0x0000CDED
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x0005B42C File Offset: 0x0005962C
		private void GiveStock()
		{
			if (this.hasGivenStock)
			{
				return;
			}
			base.skillLocator.primary.AddOneStock();
			this.hasGivenStock = true;
			usedStock--;
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x0000B44F File Offset: 0x0000964F
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}*/




		// Token: 0x04001A38 RID: 6712
		public static float enterSoundPitch;

		// Token: 0x04001A39 RID: 6713
		public static float exitSoundPitch;

		// Token: 0x04001A3A RID: 6714
		public static string enterSoundString;

		// Token: 0x04001A3B RID: 6715
		public static string exitSoundString;

		// Token: 0x04001A3C RID: 6716
		public static GameObject reloadEffectPrefab;

		// Token: 0x04001A3D RID: 6717
		public static string reloadEffectMuzzleString;

		// Token: 0x04001A3E RID: 6718
		public static float baseDuration;

		// Token: 0x04001A3F RID: 6719
		//private bool hasGivenStock;
	}
}
