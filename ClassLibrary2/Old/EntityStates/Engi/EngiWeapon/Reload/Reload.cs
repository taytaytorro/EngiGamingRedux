using RoR2;
using UnityEngine;
using BepInEx;

namespace EntityStates.Engi.EngiWeapon.Reload

{
    // Token: 0x02000479 RID: 1145
    public class Reload : BaseState
    {
        // Token: 0x1700012E RID: 302
        // (get) Token: 0x0600147D RID: 5245 RVA: 0x0005B2FC File Offset: 0x000594FC
        private float duration
        {
            get
            {
                return Reload.baseDuration / this.attackSpeedStat;
            }
        }

        // Token: 0x0600147E RID: 5246 RVA: 0x0005B30C File Offset: 0x0005950C
        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Gesture, Additive", (base.characterBody.isSprinting && base.characterMotor && base.characterMotor.isGrounded) ? "ReloadSimple" : "ChargeGrenades", "Reload.playbackRate", this.duration);
            Util.PlayAttackSpeedSound(Reload.enterSoundString, base.gameObject, Reload.enterSoundPitch);
            EffectManager.SimpleMuzzleFlash(Reload.reloadEffectPrefab, base.gameObject, Reload.reloadEffectMuzzleString, false);
        }

        // Token: 0x0600147F RID: 5247 RVA: 0x0005B394 File Offset: 0x00059594
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration / 2f)
            {
                this.GiveStock();
            }
            if (!base.isAuthority || base.fixedAge < this.duration)
            {
                return;
            }
            if (base.skillLocator.primary.stock < base.skillLocator.primary.maxStock)
            {
                this.outer.SetNextState(new Reload());
                return;
            }
            Util.PlaySound(ChargeGrenades.chargeStockSoundString, base.gameObject);
            this.outer.SetNextStateToMain();
        }

        // Token: 0x06001480 RID: 5248 RVA: 0x0000EBED File Offset: 0x0000CDED
        public override void OnExit()
        {
            Util.PlaySound(ChargeGrenades.chargeLoopStopSoundString, base.gameObject);
            base.OnExit();
        }

        // Token: 0x06001481 RID: 5249 RVA: 0x0005B42C File Offset: 0x0005962C
        private void GiveStock()
        {
            if (this.hasGivenStock)
            {
                return;
            }
            base.skillLocator.primary.AddOneStock();
            this.hasGivenStock = true;
        }

        // Token: 0x06001482 RID: 5250 RVA: 0x0000B44F File Offset: 0x0000964F
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        // Token: 0x04001A38 RID: 6712
        public static float enterSoundPitch;

        // Token: 0x04001A39 RID: 6713
        public static float exitSoundPitch;

        // Token: 0x04001A3A RID: 6714
        public static string enterSoundString;

        // Token: 0x04001A3B RID: 6715
        public static string exitSoundString;

        // Token: 0x04001A3C RID: 6716
        public static GameObject reloadEffectPrefab;

        // Token: 0x04001A3D RID: 6717
        public static string reloadEffectMuzzleString;

        // Token: 0x04001A3E RID: 6718
        public static float baseDuration = 3;

        // Token: 0x04001A3F RID: 6719
        private bool hasGivenStock;
    }
}

