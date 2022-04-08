using System;
using EntityStates.Rogue.Weapon43;
using UnityEngine;

namespace EntityStates.Rogue.Weapon42
{
	// Token: 0x02000004 RID: 4
	public class SecretCharge : BaseSkillState
	{
		// Token: 0x06000010 RID: 16 RVA: 0x000023BF File Offset: 0x000005BF
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = SecretCharge.baseChargeDuration / this.attackSpeedStat;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000023DC File Offset: 0x000005DC
		private float CalcCharge()
		{
			return Mathf.Clamp01(base.fixedAge / this.duration);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002400 File Offset: 0x00000600
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			float charge = this.CalcCharge();
			bool flag = base.isAuthority && ((!base.IsKeyDownAuthority() && base.fixedAge >= SecretCharge.minChargeDuration) || base.fixedAge >= this.duration);
			bool flag2 = flag;
			if (flag2)
			{
				SecretFire nextState = new SecretFire
				{
					charge = charge
				};
				this.outer.SetNextState(nextState);
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002474 File Offset: 0x00000674
		public override void OnExit()
		{
			base.OnExit();
			bool flag = this.chargeEffectInstance;
			bool flag2 = flag;
			if (flag2)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000024A8 File Offset: 0x000006A8
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		// Token: 0x04000010 RID: 16
		public static float baseChargeDuration = 1.4f;

		// Token: 0x04000011 RID: 17
		public static float minChargeDuration = 0.3f;

		// Token: 0x04000012 RID: 18
		public static float minBloomRadius = 0.1f;

		// Token: 0x04000013 RID: 19
		public static float maxBloomRadius = 2f;

		// Token: 0x04000014 RID: 20
		private float duration;

        // Token: 0x04000016 RID: 22
        private readonly GameObject chargeEffectInstance;
	}
}
