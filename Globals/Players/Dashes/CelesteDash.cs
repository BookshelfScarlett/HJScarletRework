using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Items.Accessories;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players.Dashes
{
    public class CelesteDash : PlayerDashClass
    {
        public override int ImmuneTime(Player player) => 12;
        public override int DashTime(Player player) => 24;
        public override int DashDelay(Player player) => 12;
        public override DashEnum DashOnHitType => DashEnum.Slam;
        public override DashDamageInfo DashDamageInfo(Player player)
        {
            return new DashDamageInfo(50, 3f, DamageClass.Generic);
        }
        public override DashDirectionEnum DashDirection => DashDirectionEnum.VerticalAndHorizonal;
        public override float DashSpeed(Player player) => 20f;
        public override float DashEndSpeedMult(Player player) => 0.75f;
        public override void OnDashStart(Player player)
        {
            SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Charge with { Variants = [1], MaxInstances = 0, PitchVariance = .2f, Pitch = -.4f });
        }
        public override void OnDashEnd(Player player)
        {
            base.OnDashEnd(player);
        }
        public override void UpdateDash(Player player)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos2 = player.ToRandRec();
                Vector2 vel2 = player.velocity / 8f;
                Color color2 = RandLerpColor(Color.SkyBlue, Color.RoyalBlue);
                if (player.HJScarlet().mayaPumperDashType == PinballPurgatory.PinkType)
                    color2 = RandLerpColor(Color.Pink, Color.HotPink);
                ECSParticle.SnowCloud(pos2, vel2, color2, 40, 0, .75f, 0.1f * 0.85f);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos2 = player.ToRandRec();
                Vector2 vel2 = player.velocity / 8f;
                Color color2 = RandLerpColor(Color.SkyBlue, Color.RoyalBlue);
                if (player.HJScarlet().mayaPumperDashType == PinballPurgatory.PinkType)
                    color2 = RandLerpColor(Color.Pink, Color.HotPink);
                ECSParticle.HRShinyOrb(pos2, vel2, color2, 60, .81f, 0.08f, 0.85f);

            }
        }
        public override void OnHitNPC(Player player, NPC target, int DamageDone)
        {
        }
    }

}
