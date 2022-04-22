using System;
using EngiShotgu;
using EntityStates.EngiTurret.EngiTurretWeapon;
using EntityStates.Toolbot;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon.Rux2
{
	// Token: 0x02000005 RID: 5
	public class PlasmaGrenade : AimThrowableBase
	{
		// Token: 0x06000017 RID: 23 RVA: 0x000024F0 File Offset: 0x000006F0
		public override void OnEnter()
		{
			bool flag = this.goodState == null;
			if (flag)
			{
				this.goodState = new AimStunDrone();
			}
			this.maxDistance = 60f;
			this.rayRadius = 8.51f;
			this.arcVisualizerPrefab = this.goodState.arcVisualizerPrefab;
			this.projectilePrefab = Engiplugin.PlasmaGrenadeObject;
			this.endpointVisualizerPrefab = this.goodState.endpointVisualizerPrefab;
			this.endpointVisualizerRadiusScale = 4f;
			this.setFuse = false;
			this.damageCoefficient = 6f;
			this.baseMinimumDuration = 0.1f;
			this.projectileBaseSpeed = 35f;
			base.OnEnter();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002598 File Offset: 0x00000798
		public override void FixedUpdate()
		{
			base.characterBody.SetAimTimer(0.25f);
			base.fixedAge += Time.fixedDeltaTime;
			bool flag = false;
			bool flag2 = base.isAuthority && !this.KeyIsDown() && base.fixedAge >= this.minimumDuration;
			bool flag3 = flag2;
			if (flag3)
			{
				flag = true;
			}
			bool flag4 = base.characterBody && base.characterBody.isSprinting;
			bool flag5 = flag4;
			if (flag5)
			{
				flag = true;
			}
			bool flag6 = flag;
			bool flag7 = flag6;
			if (flag7)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000263C File Offset: 0x0000083C
		public override void OnExit()
		{
			base.OnExit();
			base.PlayCrossfade("Gesture, Additive", "FireMineRight", "FireMine.playbackRate", 0.3f, 0.05f);
			base.AddRecoil(0f, 0f, 0f, 0f);
			base.characterBody.AddSpreadBloom(1.75f);
			Util.PlaySound("Play_commando_M2_grenade_throw", base.gameObject);
			EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, base.gameObject, PlasmaGrenade.muzzleString, false);
		}

		// Token: 0x04000017 RID: 23
		private AimStunDrone goodState;

		// Token: 0x04000018 RID: 24
		public static string muzzleString = "MuzzleCenter";
	}
}
