using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using static EngiShotgun.EngineerShotgunPlugin;
using UnityEngine;
using R2API;
using static R2API.RecalculateStatsAPI;

namespace ShotgunengiREDUX.SkillDefs
{
    public class JetpackBuff
    {
        private static ConfigEntry<float> _speedBoost;
        private static Sprite buffIcon => MainAssets.LoadAsset<UnityEngine.Sprite>("JetpackBuffIcon");
        public static void Init(ConfigFile config)
        {
            AddConfig(config);
            AddBuffDef();
        }

        public static void AddConfig(ConfigFile config)
        {
            _speedBoost = config.Bind(SkillDefs.JetpackSlot.configPrefix, "Speed Boost", 20f, "The % bonus to speed while the Jump Jets are active.");
        }

        public static BuffDef jetpackBuff;
        public static void AddBuffDef()
        {
            jetpackBuff = UnityEngine.ScriptableObject.CreateInstance<BuffDef>();
            jetpackBuff.name = "Active Jetpack";
            jetpackBuff.isHidden = false;
            jetpackBuff.canStack = false;
            jetpackBuff.buffColor = new Color(0, 1, 0);
            Debug.LogWarning($"Buff Icon is {buffIcon}");
            jetpackBuff.iconSprite = buffIcon;
            ContentAddition.AddBuffDef(jetpackBuff);
            GetStatCoefficients += AddJetpackSpeed;
        }
        public static void AddJetpackSpeed(CharacterBody Sender, StatHookEventArgs EventArgs)
        {
            if (Sender.HasBuff(jetpackBuff))
            {
                EventArgs.moveSpeedMultAdd += _speedBoost.Value * 0.01f;
            }
        }
    }
}
