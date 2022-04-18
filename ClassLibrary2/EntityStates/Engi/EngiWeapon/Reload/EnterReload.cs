using System;
using RoR2;

namespace EntityStates.Engi.EngiWeapon.Reload
{
	// Token: 0x02000478 RID: 1144
	public class EnterReload : BaseState
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06001478 RID: 5240 RVA: 0x0005B287 File Offset: 0x00059487
		private float duration
		{
			get
			{
				return EnterReload.baseDuration / this.attackSpeedStat;
			}
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x0005B295 File Offset: 0x00059495
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayCrossfade("Gesture, Additive", "ChargeGrenades", "Reload.playbackRate", this.duration, 0.1f);
			Util.PlaySound(ChargeGrenades.chargeLoopStartSoundString, base.gameObject);
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x0005B2CE File Offset: 0x000594CE
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge > this.duration)
			{
				this.outer.SetNextState(new Reload());
			}
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x0000B44F File Offset: 0x0000964F
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04001A36 RID: 6710
		public static string enterSoundString;

		// Token: 0x04001A37 RID: 6711
		public static float baseDuration;
	}
}


