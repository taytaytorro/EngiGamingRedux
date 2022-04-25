using BepInEx.Configuration;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using ShotgunengiREDUX.EntityStates.Engi;
using System;
using System.Collections.Generic;
using System.Text;
using static EngiShotgun.EngineerShotgunPlugin;

namespace ShotgunengiREDUX.SkillDefs
{
    public class JetpackSlot
    {
        private static ConfigEntry<float> _speedBoost;
        public static float SpeedBoost
        {
            get => (_speedBoost.Value);
        }
        static SkillDef jetpackSkillDef;

        public static UnityEngine.Sprite skillSprite => MainAssets.LoadAsset<UnityEngine.Sprite>("JetpackIcon");

        public static string configPrefix = "Jump Jets";

        public static ConfigEntry<float> flightDuration;
        public static ConfigEntry<float> skillCooldown;

        public static void Init(ConfigFile config)
        {
            AddConfig(config);
            AddLanguage();
            JetpackBuff.Init(config);
            JetpackState.Init(config);
            AddSkillDef();
            UpdateSkillFamily();
        }
        public static void AddConfig(ConfigFile config)
        {
            flightDuration = config.Bind(configPrefix, "Flight Duration", 2f, "The time that the Jump Jets remain active.");
            skillCooldown = config.Bind(configPrefix, "Cooldown", 15f, "The cooldown before the Jump Jets may be reused.");
            _speedBoost = config.Bind(SkillDefs.JetpackSlot.configPrefix, "Speed Boost", 20f, "The % bonus to speed while the Jump Jets are active.");
        }

        public static void AddLanguage()
        {
            LanguageAPI.Add("ENGIPLUS_JETPACK_NAME", "JumpJets");
            LanguageAPI.Add("ENGIPLUS_JETPACK_DESCRIPTION", $"Use your jets and <style=cIsUtility>fly</style> for {flightDuration.Value} seconds, gaining {SpeedBoost}% movement speed.");
            LanguageAPI.Add("ENGIPLUS_JETPACK_NAMETOKEN", "Fusion Jump Jets");
            LanguageAPI.Add("ENGIPLUS_KEYWORD_FLYING", "<style=cKeywordName>Flying</style>Move and jump through the air during this effect's duration.");
        }

        public static void AddSkillDef()
        {
            SerializableEntityStateType jetpackState = R2API.ContentAddition.AddEntityState<JetpackState>(out _);


            jetpackSkillDef = UnityEngine.ScriptableObject.CreateInstance<SkillDef>();

            jetpackSkillDef.activationState = jetpackState;
            jetpackSkillDef.activationStateMachineName = "Body";
            jetpackSkillDef.baseMaxStock = 1;
            jetpackSkillDef.baseRechargeInterval = skillCooldown.Value;
            jetpackSkillDef.beginSkillCooldownOnSkillEnd = true;
            jetpackSkillDef.canceledFromSprinting = false;
            jetpackSkillDef.fullRestockOnAssign = false;
            jetpackSkillDef.interruptPriority = InterruptPriority.PrioritySkill;
            jetpackSkillDef.isCombatSkill = false;
            jetpackSkillDef.mustKeyPress = true;
            jetpackSkillDef.cancelSprintingOnActivation = false;
            jetpackSkillDef.forceSprintDuringState = true;
            jetpackSkillDef.rechargeStock = 1;
            jetpackSkillDef.requiredStock = 1;
            jetpackSkillDef.stockToConsume = 1;
            jetpackSkillDef.icon = skillSprite;
            jetpackSkillDef.skillDescriptionToken = "ENGIPLUS_JETPACK_DESCRIPTION";
            jetpackSkillDef.skillName = "ENGIPLUS_JETPACK_NAME";
            jetpackSkillDef.skillNameToken = "ENGIPLUS_JETPACK_NAMETOKEN";
            jetpackSkillDef.keywordTokens = new string[] { "ENGIPLUS_KEYWORD_FLYING" };
            ContentAddition.AddSkillDef(jetpackSkillDef);
        }
        
        public static void UpdateSkillFamily()
        {
            SkillLocator component = UnityEngine.Resources.Load<UnityEngine.GameObject>("prefabs/characterbodies/EngiBody").GetComponent<SkillLocator>();
            var skillFamily = component.utility.skillFamily;
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = jetpackSkillDef,
                viewableNode = new ViewablesCatalog.Node(jetpackSkillDef.skillNameToken, false, null)
            };
        }
    }
}
