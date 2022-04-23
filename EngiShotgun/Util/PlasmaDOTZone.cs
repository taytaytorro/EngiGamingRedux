using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000B8B RID: 2955
	[RequireComponent(typeof(HitBoxGroup))]
	[RequireComponent(typeof(ProjectileController))]
	public class PlasmaDOTZone: MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x0600433A RID: 17210 RVA: 0x001169C0 File Offset: 0x00114BC0
		private void Start()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.ResetOverlap();
			this.onBegin.Invoke();
			if (!string.IsNullOrEmpty(this.soundLoopString))
			{
				Util.PlaySound(this.soundLoopString, base.gameObject);
			}
		}

		// Token: 0x0600433B RID: 17211 RVA: 0x00116A18 File Offset: 0x00114C18
		private void ResetOverlap()
		{
			this.attack = new OverlapAttack();
			this.attack.procChainMask = this.projectileController.procChainMask;
			this.attack.procCoefficient = this.projectileController.procCoefficient * this.overlapProcCoefficient;
			this.attack.attacker = this.projectileController.owner;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = this.projectileController.teamFilter.teamIndex;
			this.attack.attackerFiltering = this.attackerFiltering;
			this.attack.damage = this.damageCoefficient * this.projectileDamage.damage;
			this.attack.forceVector = this.forceVector + this.projectileDamage.force * base.transform.forward;
			this.attack.hitEffectPrefab = this.impactEffect;
			this.attack.isCrit = this.projectileDamage.crit;
			this.attack.damageColorIndex = this.projectileDamage.damageColorIndex;
			this.attack.damageType = this.projectileDamage.damageType;
			this.attack.hitBoxGroup = base.GetComponent<HitBoxGroup>();
		}

		// Token: 0x0600433C RID: 17212 RVA: 0x000026ED File Offset: 0x000008ED
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
		}

		// Token: 0x0600433D RID: 17213 RVA: 0x00116B68 File Offset: 0x00114D68
		public void FixedUpdate()
		{
			this.totalStopwatch += Time.fixedDeltaTime;
			if (NetworkServer.active)
			{
				this.resetStopwatch += Time.fixedDeltaTime;
				this.fireStopwatch += Time.fixedDeltaTime;
				if (this.resetStopwatch >= 1f / this.resetFrequency)
				{
					this.ResetOverlap();
					this.resetStopwatch -= 1f / this.resetFrequency;
				}
				if (this.fireStopwatch >= 1f / this.fireFrequency)
				{
					this.attack.Fire(null);
					this.fireStopwatch -= 1f / this.fireFrequency;
				}
			}
			if (this.lifetime > 0f && this.totalStopwatch >= this.lifetime)
			{
				this.onEnd.Invoke();
				if (!string.IsNullOrEmpty(this.soundLoopStopString))
				{
					Util.PlaySound(this.soundLoopStopString, base.gameObject);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04004165 RID: 16741
		private ProjectileController projectileController;

		// Token: 0x04004166 RID: 16742
		private ProjectileDamage projectileDamage;

		// Token: 0x04004167 RID: 16743
		public float damageCoefficient;

		// Token: 0x04004168 RID: 16744
		public AttackerFiltering attackerFiltering = AttackerFiltering.NeverHitSelf;

		// Token: 0x04004169 RID: 16745
		public GameObject impactEffect;

		// Token: 0x0400416A RID: 16746
		public Vector3 forceVector;

		// Token: 0x0400416B RID: 16747
		public float overlapProcCoefficient = 1f;

		// Token: 0x0400416C RID: 16748
		[Tooltip("The frequency (1/time) at which the overlap attack is tested. Higher values are more accurate but more expensive.")]
		public float fireFrequency = 1f;

		// Token: 0x0400416D RID: 16749
		[Tooltip("The frequency  (1/time) at which the overlap attack is reset. Higher values means more frequent ticks of damage.")]
		public float resetFrequency = 20f;

		// Token: 0x0400416E RID: 16750
		public float lifetime = 30f;

		// Token: 0x0400416F RID: 16751
		[Tooltip("The event that runs at the start.")]
		public UnityEvent onBegin;

		// Token: 0x04004170 RID: 16752
		[Tooltip("The event that runs at the start.")]
		public UnityEvent onEnd;

		// Token: 0x04004171 RID: 16753
		private OverlapAttack attack;

		// Token: 0x04004172 RID: 16754
		private float fireStopwatch;

		// Token: 0x04004173 RID: 16755
		private float resetStopwatch;

		// Token: 0x04004174 RID: 16756
		private float totalStopwatch;

		// Token: 0x04004175 RID: 16757
		public string soundLoopString = "";

		// Token: 0x04004176 RID: 16758
		public string soundLoopStopString = "";
	}
}
