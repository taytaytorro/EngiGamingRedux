using BepInEx.Configuration;
using EntityStates;
using RoR2;
using ShotgunengiREDUX.SkillDefs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ShotgunengiREDUX.EntityStates.Engi
{
    public class JetpackState : GenericCharacterMain
    {

		public static ConfigEntry<float> flightDuration;
        public static ConfigEntry<float> boostPower;

        public static void Init(ConfigFile config)
        {
            Debug.LogWarning("Jetpack has initialized!");
            AddConfig(config);
        }

        public static void AddConfig(ConfigFile config)
        {
            flightDuration = config.Bind(SkillDefs.JetpackSlot.configPrefix, "Flight Duration", 5f, "The time that the Jump Jets remain active.");
            boostPower = config.Bind(SkillDefs.JetpackSlot.configPrefix, "Boost Power", 3f, "The power of your jumps while the Jump Jets are active.");
        }

        public override void OnEnter()
        {
			RoR2.Util.PlaySound("Play_engi_shift_start", characterBody.gameObject);
			characterBody.AddBuff(JetpackBuff.jetpackBuff);
			base.OnEnter();
        }
        public override void OnExit()
        {

			RoR2.Util.PlaySound("Play_engi_shift_end", characterBody.gameObject);
			characterBody.RemoveBuff(JetpackBuff.jetpackBuff);
			base.OnExit();
		}
        public override void FixedUpdate()
		{
			base.FixedUpdate();
			stopwatch += Time.fixedDeltaTime;
			boostCooldownTimer -= Time.fixedDeltaTime;

			if (isAuthority && characterBody && characterBody.characterMotor)
			{
				characterBody.isSprinting = true;
				if (stopwatch < flightDuration.Value)
				{
					if (boostCooldownTimer <= 0f && characterBody.inputBank.jump.justPressed && characterBody.inputBank.moveVector != Vector3.zero)
					{
						boostCooldownTimer = boostCooldown;
						characterBody.characterMotor.velocity = characterBody.inputBank.aimDirection * (characterBody.moveSpeed * boostPower.Value);
						characterBody.characterMotor.disableAirControlUntilCollision = false;
					}
					var velocity = characterBody.characterMotor.velocity;
					velocity.y = Mathf.Max(velocity.y, -2f);
					characterBody.characterMotor.velocity = velocity;
				}
				else
				{
					outer.SetNextStateToMain();
				}
			}
		}
		public float boostCooldown = 0.5f;
		private float stopwatch = 0;
		private float boostCooldownTimer;



	}
}
