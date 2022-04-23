using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.EngiTurret.EngiTurretWeapon;
using BepInEx.Configuration;
using EngiShotgun;
using RoR2.UI;
using UnityEngine.UI;
using static RoR2.UI.CrosshairController;

namespace EntityStates.Engi.EngiWeapon
{
	public class EngiShotgun : BaseState
	{
		static string configPrefix = EngiShotgunDef.configPrefix;
		static int shotgunStock = EngiShotgunDef.shotgunStock.Value;

		public static ConfigEntry<float> minSpread;
		public static ConfigEntry<float> maxSpread;
		public static ConfigEntry<int> damageCoeff;
		public static ConfigEntry<float> procCoefficient;
		public static ConfigEntry<uint> bulletCount;
		public static ConfigEntry<float> shotgunBloom;
		public static ConfigEntry<int> maxVisibleStocks;

		private static GameObject _crosshairPrefab;
		private static GameObject _stockPrefab;
		static HorizontalLayoutGroup stockGroup;
		public static GameObject CrosshairPrefab 
		{ get
			{
				if(CrosshairPrefab == null) _crosshairPrefab = EngineerShotgunPlugin.MainAssets.LoadAsset<UnityEngine.GameObject>("GaussCrosshair");
				return _crosshairPrefab;
			} 
		}
		public static GameObject StockPrefab
		{
			get
			{
				if (_stockPrefab == null) _stockPrefab = EngineerShotgunPlugin.MainAssets.LoadAsset<UnityEngine.GameObject>("GaussStock");
				return _stockPrefab;
			}
		}

		public static void AddConfig(ConfigFile config)
        {
			minSpread = config.Bind(configPrefix, "Min Spread", 1.2f, "The smallest possible angle between a shotgun projectile and the crosshair.");
			maxSpread = config.Bind(configPrefix, "Max Spread", 2.2f, "The largest possible angle between a shotgun projectile and the crosshair.");
			damageCoeff = config.Bind(configPrefix, "Projectile Damage%", 60, "The base damage of each shotgun projectile, in %");
			procCoefficient = config.Bind(configPrefix, "Proc Coeffient%", 0.45f, "The proc coefficient multiplier of each shotgun projectile.");
			shotgunBloom = config.Bind(configPrefix, "Shotgun Bloom", 0.5f, "The temporary inaccuracy added by each round fired.");
			bulletCount = config.Bind(configPrefix, "Bullet Count", 8U, "The bullet count of each shotgun blast.");
			maxVisibleStocks = config.Bind(configPrefix, "Max Visible Stock", 10, "The maximum amount of shotgun stock that can appear in your HUD.");
            {
				bulletCount.Value = (uint) Mathf.CeilToInt(bulletCount.Value / 2);
            }
			stockGroup = CrosshairPrefab.GetComponent<HorizontalLayoutGroup>();
			var stockController = CrosshairPrefab.GetComponent<CrosshairController>();
			var displays = stockController.skillStockSpriteDisplays;
			var newDisplays = new SkillStockSpriteDisplay[2 + shotgunStock];
			newDisplays[0] = displays[0];
			newDisplays[1] = displays[1];

			var max = Mathf.Min(shotgunStock, maxVisibleStocks.Value);
			newDisplays[1].maximumStockCountToBeValid = max;
			for (int stock = 0; stock < max; stock++)
            {
				GameObject newStock = GameObject.Instantiate(StockPrefab, stockGroup.transform);
				var stockSlot = newDisplays[stock + 2];
				stockSlot.target = newStock;
				stockSlot.minimumStockCountToBeValid = stock + 1;
				stockSlot.maximumStockCountToBeValid = max;
            }
			stockController.skillStockSpriteDisplays = newDisplays;
		
		}
		public static void Init(ConfigFile config)
        {
			AddConfig(config);
        }

		public override void OnEnter()
		{
			base.OnEnter();
			overrideRequest = CrosshairUtils.RequestOverrideForBody(characterBody, CrosshairPrefab, CrosshairUtils.OverridePriority.Skill);
			maxDuration = baseMaxDuration / attackSpeedStat;
			minDuration = baseMaxDuration / attackSpeedStat;
			Util.PlaySound(FireGauss.attackSoundString, gameObject);
			characterBody.skillLocator.primary.rechargeStopwatch = 0f;
			muzzleString = "MuzzleRight";
			muzzleString2 = "MuzzleLeft";
			PlayAnimation("Gesture Right, Additive", "FireGrenadeRight", "FireGrenadeRight.playbackRate", maxDuration);
			PlayAnimation("Gesture Left, Additive", "FireGrenadeLeft", "FireGrenadeLeft.playbackRate", maxDuration);
			characterBody.AddSpreadBloom(shotgunBloom.Value);

			if (muzzleEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, gameObject, muzzleString, false);
				EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, gameObject, muzzleString2, false);
			}
			if (isAuthority)
			{
				Ray aimRay = GetAimRay();
				AddRecoil(-4.5f, -1f, -1f, 4f);
				var attack = new BulletAttack
				{
					queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
					bulletCount = bulletCount.Value,
					aimVector = aimRay.direction,
					origin = aimRay.origin,
					damage = damageCoeff.Value * damageStat / 100f,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					force = 9f,
					hitMask = LayerIndex.CommonMasks.bullet,
					minSpread = minSpread.Value,
					maxSpread = maxSpread.Value,
					isCrit = Util.CheckRoll(critStat, characterBody.master),
					owner = gameObject,
					muzzleName = muzzleString2,
					smartCollision = false,
					procChainMask = default(ProcChainMask),
					procCoefficient = procCoefficient.Value,
					radius = 0.53f,
					sniper = false,	
					stopperMask = LayerIndex.CommonMasks.bullet,
					weapon = null,
					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					hitEffectPrefab = FireGauss.hitEffectPrefab
				};
				attack.Fire();
				Ray aimRay2 = GetAimRay();
				base.AddRecoil(-4.5f, -1f, -1f, 4f);
				attack = new BulletAttack
				{
					queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
					bulletCount = bulletCount.Value,
					aimVector = aimRay2.direction,
					origin = aimRay2.origin,
					damage = damageCoeff.Value * damageStat / 100f,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					force = 9f,
					hitMask = LayerIndex.CommonMasks.bullet,
					minSpread = 2.2f,
					maxSpread = 2.2f,
					isCrit = Util.CheckRoll(critStat, characterBody.master),
					owner = gameObject,
					muzzleName = muzzleString,
					smartCollision = false,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0.45f,
					radius = 0.53f,
					sniper = false,
					stopperMask = LayerIndex.CommonMasks.bullet,
					weapon = null,
					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					hitEffectPrefab = FireGauss.hitEffectPrefab
				};
				attack.Fire();
				shotfired = true;
				
			}
		}
		public override void OnExit()
		{
			if (!buttonReleased && characterBody)
			{
				characterBody.SetSpreadBloom(0f, false);
			}
			base.OnExit();
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (shotfired == true)
            {
				//skillLocator.primary.DeductStock(1);
				shotfired = false;
			}
			//base.FixedUpdate();
			buttonReleased |= !inputBank.skill1.down;
			if (fixedAge >= maxDuration && isAuthority)
			{
				outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (buttonReleased && fixedAge >= minDuration)
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
        private CrosshairUtils.OverrideRequest overrideRequest;
        private float maxDuration;
		private float minDuration;
		private bool buttonReleased;
		private bool shotfired;
		private string muzzleString;
		private string muzzleString2;
	}

}




