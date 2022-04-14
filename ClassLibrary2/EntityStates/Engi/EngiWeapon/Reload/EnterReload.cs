using System;
using RoR2;

namespace EntityStates.Engi.EngiWeapon.Reload
{
	// Token: 0x0200001D RID: 29
	public class EnterReload : BaseState
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600009B RID: 155 RVA: 0x000053E0 File Offset: 0x000035E0
		private float duration
		{
			get
			{
				return EnterReload.baseDuration / this.attackSpeedStat;
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000053FE File Offset: 0x000035FE
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayCrossfade("Gesture, Additive", "ChargeGrenades", "Reload.playbackRate", this.duration, 0.1f);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x0000542C File Offset: 0x0000362C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = base.isAuthority && base.fixedAge > this.duration;
			if (flag)
			{
				this.outer.SetNextState(new Reload());
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005474 File Offset: 0x00003674
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040000BF RID: 191
		public static float baseDuration = 0.3f;
	}
}
