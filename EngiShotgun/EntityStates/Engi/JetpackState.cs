using BepInEx.Configuration;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ShotgunengiREDUX.EntityStates.Engi
{
    public class JetpackState : BaseState
    {

		public static ConfigEntry<float> flightDuration;
        public static ConfigEntry<float> boostPower;
        private static ConfigEntry<float> _speedBoost;

        public static string configPrefix = "Jump Jets";

        public static void Init(ConfigFile config)
        {
            Debug.LogWarning("Jetpack has initialized!");
            AddConfig(config);
        }

        public static void AddConfig(ConfigFile config)
        {
            flightDuration = config.Bind(configPrefix, "Flight Duration", 5f, "The time that the Jump Jets remain active.");
            boostPower = config.Bind(configPrefix, "Boost Power", 3f, "The power of your jumps while the Jump Jets are active.");
            _speedBoost = config.Bind(configPrefix, "Speed Boost", 20f, "The % bonus to speed while the Jump Jets are active.");
        }

        public override void OnEnter()
        {
			RoR2.Util.PlaySound("Play_engi_shift_start", characterBody.gameObject);
			base.OnEnter();
        }
        public override void OnExit()
        {

			RoR2.Util.PlaySound("Play_engi_shift_end", characterBody.gameObject);
			base.OnExit();
		}
        public override void FixedUpdate()
		{
			base.FixedUpdate();
			stopwatch += Time.fixedDeltaTime;
			boostCooldownTimer -= Time.fixedDeltaTime;
			if (isAuthority && characterBody && characterBody.characterMotor)
			{
				if (stopwatch < flightDuration.Value)
				{
					if (boostCooldownTimer <= 0f && characterBody.inputBank.jump.justPressed && characterBody.inputBank.moveVector != Vector3.zero)
					{
						boostCooldownTimer = boostCooldown;
						characterBody.characterMotor.velocity = characterBody.inputBank.moveVector * (characterBody.moveSpeed * boostPower.Value);
						characterBody.characterMotor.disableAirControlUntilCollision = false;
					}
				}
				else
				{
					Vector3 velocity = characterBody.characterMotor.velocity;
					velocity.y = Mathf.Max(velocity.y, -5f);
					characterBody.characterMotor.velocity = velocity;
				}
			}
		}
		public float boostCooldown = 0.5f;
		private float stopwatch = 0;
		private float boostCooldownTimer;



	}
}
