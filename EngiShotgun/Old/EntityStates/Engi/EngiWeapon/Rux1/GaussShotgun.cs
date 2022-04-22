using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.EngiTurret.EngiTurretWeapon;

namespace EntityStates.Engi.EngiWeapon.Rux1
{
	public class GaussShotgun : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.maxDuration = baseMaxDuration / this.attackSpeedStat;
			this.minDuration = baseMaxDuration / this.attackSpeedStat;
			Util.PlaySound(FireGauss.attackSoundString, base.gameObject);
			base.characterBody.skillLocator.primary.rechargeStopwatch = 0f;
			this.muzzleString = "MuzzleRight";
			this.muzzleString2 = "MuzzleLeft";
			base.PlayAnimation("Gesture Right, Additive", "FireGrenadeRight", "FireGauntlet.playbackRate", maxDuration);
			base.PlayAnimation("Gesture Left, Additive", "FireGrenadeLeft", "FireGauntlet.playbackRate", maxDuration);
			base.characterBody.AddSpreadBloom(0.5f);
			if (muzzleEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, base.gameObject, this.muzzleString, false);
				EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, base.gameObject, this.muzzleString2, false);
			}
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				base.AddRecoil(-4.5f, -1f, -1f, 4f);
				new BulletAttack
				{
					bulletCount = 4U,
					aimVector = aimRay.direction,
					origin = aimRay.origin,
					damage = 0.6f * this.damageStat,
					damageColorIndex = 0,
					damageType = 0,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					force = 9f,
					hitMask = LayerIndex.CommonMasks.bullet,
					minSpread = 2.2f,
					maxSpread = 2.2f,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					owner = base.gameObject,
					muzzleName = this.muzzleString2,
					smartCollision = false,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0.45f,
					radius = 0.53f,
					sniper = false,
					stopperMask = LayerIndex.CommonMasks.bullet,
					weapon = null,
					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					queryTriggerInteraction = 0,
					hitEffectPrefab = FireGauss.hitEffectPrefab
				}.Fire();
				Ray aimRay2 = base.GetAimRay();
				base.AddRecoil(-4.5f, -1f, -1f, 4f);
				new BulletAttack
				{
					bulletCount = 4U,
					aimVector = aimRay2.direction,
					origin = aimRay2.origin,
					damage = 0.6f * this.damageStat,
					damageColorIndex = 0,
					damageType = 0,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					force = 9f,
					hitMask = LayerIndex.CommonMasks.bullet,
					minSpread = 2.2f,
					maxSpread = 2.2f,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					owner = base.gameObject,
					muzzleName = this.muzzleString,
					smartCollision = false,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0.45f,
					radius = 0.53f,
					sniper = false,
					stopperMask = LayerIndex.CommonMasks.bullet,
					weapon = null,
					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					queryTriggerInteraction = 0,
					hitEffectPrefab = FireGauss.hitEffectPrefab
				}.Fire();
				shotfired = true;
				
			}
		}
		public override void OnExit()
		{
			if (!buttonReleased && base.characterBody)
			{
				base.characterBody.SetSpreadBloom(0f, false);
			}
			base.OnExit();
		}
		public override void FixedUpdate()
		{
			if (shotfired == true)
            {
				base.skillLocator.primary.DeductStock(1);
				shotfired = false;
			}
			base.FixedUpdate();
			this.buttonReleased |= !base.inputBank.skill1.down;
			if (base.fixedAge >= this.maxDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (this.buttonReleased && base.fixedAge >= this.minDuration)
			{
				return InterruptPriority.Any;
			}
			return InterruptPriority.PrioritySkill;
		}

		public static GameObject muzzleEffectPrefab;
		public static GameObject tracerEffectPrefab;
		public static GameObject impactEffectPrefab;
		public static float bulletRadius = 0.4f;
		public static float baseMaxDuration = 0.7f;
		public static float baseMinDuration = 0.7f;
		private float maxDuration;
		private float minDuration;
		private bool buttonReleased;
		private bool shotfired;
		private string muzzleString;
		private string muzzleString2;
	}

}




