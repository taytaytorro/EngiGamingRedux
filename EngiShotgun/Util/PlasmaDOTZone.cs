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
			projectileController = base.GetComponent<ProjectileController>();
			projectileDamage = base.GetComponent<ProjectileDamage>();
			ResetOverlap();
			onBegin.Invoke();
			if (!string.IsNullOrEmpty(soundLoopString))
			{
				Util.PlaySound(soundLoopString, base.gameObject);
			}
		}

		// Token: 0x0600433B RID: 17211 RVA: 0x00116A18 File Offset: 0x00114C18
		private void ResetOverlap()
		{
			attack = new OverlapAttack();
			attack.procChainMask = projectileController.procChainMask;
			attack.procCoefficient = projectileController.procCoefficient * overlapProcCoefficient;
			attack.attacker = projectileController.owner;
			attack.inflictor = base.gameObject;
			attack.teamIndex = projectileController.teamFilter.teamIndex;
			attack.attackerFiltering = attackerFiltering;
			attack.damage = damageCoefficient * projectileDamage.damage;
			attack.forceVector = forceVector + projectileDamage.force * base.transform.forward;
			attack.hitEffectPrefab = impactEffect;
			attack.isCrit = projectileDamage.crit;
			attack.damageColorIndex = projectileDamage.damageColorIndex;
			attack.damageType = projectileDamage.damageType;
			attack.hitBoxGroup = base.GetComponent<HitBoxGroup>();
		}

		// Token: 0x0600433C RID: 17212 RVA: 0x000026ED File Offset: 0x000008ED
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
		}

		// Token: 0x0600433D RID: 17213 RVA: 0x00116B68 File Offset: 0x00114D68
		public void FixedUpdate()
		{
			totalStopwatch += Time.fixedDeltaTime;
			if (NetworkServer.active)
			{
				resetStopwatch += Time.fixedDeltaTime;
				fireStopwatch += Time.fixedDeltaTime;
				if (resetStopwatch >= 1f / resetFrequency)
				{
					ResetOverlap();
					resetStopwatch -= 1f / resetFrequency;
				}
				if (fireStopwatch >= 1f / fireFrequency)
				{
					attack.Fire(null);
					fireStopwatch -= 1f / fireFrequency;
				}
			}
			if (lifetime > 0f && totalStopwatch >= lifetime)
			{
				onEnd.Invoke();
				if (!string.IsNullOrEmpty(soundLoopStopString))
				{
					Util.PlaySound(soundLoopStopString, base.gameObject);
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
