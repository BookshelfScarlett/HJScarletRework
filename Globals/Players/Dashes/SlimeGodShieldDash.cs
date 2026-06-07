using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players.Dashes
{
    public class SlimeGodShieldDash : PlayerDashClass
    {
        public override int ImmuneTime(Player player) => 25;
        public override int DashTime(Player player) => 15;
        public override int DashDelay(Player player) => 15;
        public override DashEnum DashOnHitType => DashEnum.Slam;
        public override DashDamageInfo DashDamageInfo(Player player)
        {
            return new DashDamageInfo(100, 3f, DamageClass.Generic);
        }
        public override float DashSpeed(Player player) => 30f;
        public override float DashEndSpeedMult(Player player) => 0.5f;
        public override void OnDashStart(Player player)
        {
            SoundEngine.PlaySound(HJScarletSounds.Blunt_Swing with { MaxInstances = 0, PitchVariance = .2f });
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
                Color color2 = RandLerpColor(Color.OrangeRed, Color.DarkOrange);
                ECSParticle.SnowCloud(pos2, vel2, color2, 40, 0, .75f, 0.1f * 0.85f);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos2 = player.ToRandRec();
                Vector2 vel2 = player.velocity / 8f;
                Color color2 = RandLerpColor(Color.OrangeRed, Color.DarkOrange);
                ECSParticle.SmokeParticle(pos2, vel2, color2, 60, RandRotTwoPi, 1f, 0.32f, true, Microsoft.Xna.Framework.Graphics.BlendState.NonPremultiplied);
                //ECSParticle.HRShinyOrb(pos2, vel2, color2, 60, .81f, 0.08f, 0.85f);

            }
        }
        public override void OnHitNPC(Player player, NPC target, int DamageDone)
        {
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { MaxInstances = 0, PitchVariance = .2f });
            int length = 32;
            for (int i = 0; i < length; i++)
            {
                Vector2 pos2 = target.Center.ToRandCirclePos(16f);
                Vector2 vel2 = target.Center.GetNormalVector2(pos2) * Main.rand.NextFloat(1f, 6f);
                Color color2 = RandLerpColor(Color.OrangeRed, Color.WhiteSmoke);
                ECSParticle.SnowCloud(pos2, vel2, color2, 40, 0, .85f, 0.4f * 0.25f);
            }
            new ShinyRing(target.Center, Vector2.Zero, Color.White, 40, 0.15f, opacity: .75f, fadeIn: true).Spawn();
            new ShinyRing(target.Center, Vector2.Zero, RandLerpColor(Color.OrangeRed, Color.Orange), 40, 0.15f, fadeIn: true).SpawnToPriorityNonPreMult();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    Vector2 pos = target.Center.ToRandCirclePos(3);
                    Vector2 vel = Vector2.UnitX.RotatedBy(i * PiOver2) * Main.rand.NextFloat(-1f, 4.8f);
                    Color color = RandLerpColor(Color.Orange, Color.OrangeRed);
                    float scale = Main.rand.NextFloat(.8f, 1.1f) * 1.1f;
                    int lifeTime = Main.rand.Next(45, 60);
                    new StarShape(pos, vel, color, scale, lifeTime).Spawn();
                }
            }
        }
    }
}
