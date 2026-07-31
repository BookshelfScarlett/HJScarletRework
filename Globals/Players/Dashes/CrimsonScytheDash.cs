using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players.Dashes
{
    public class CrimsonScytheDash : PlayerDashClass
    {
        public override int ImmuneTime(Player player) => 24;
        public override int DashTime(Player player) => 18;
        public override int DashDelay(Player player) => 12;
        public override DashEnum DashOnHitType => DashEnum.Slam;
        public override DashDamageInfo DashDamageInfo(Player player)
        {
            return new DashDamageInfo(100, 3f, DamageClass.Generic);
        }
        public override float DashSpeed(Player player) => 28f;
        public override float DashEndSpeedMult(Player player) => 0.5f;
        public override void OnDashStart(Player player)
        {
            ScarletSound(HJScarletSounds.GalvanizedHand_Charge, player.Center, 1, 1, pitch: .1f, .1f, 1);
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
                Color color2 = RandLerpColor(Color.DarkRed, Color.Crimson);
                ECSParticle.SmokeParticle(pos2, vel2, color2, 40, 0, .55f, 0.65f);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos2 = player.ToRandRec();
                Vector2 vel2 = player.velocity / 8f;
                Color color2 = RandLerpColor(Color.RoyalBlue, Color.WhiteSmoke);
                BloodyMetaball.SpawnParticle(pos2, vel2, 0.7f, vel2.ToRotation());

            }
        }
        public override void OnHitNPC(Player player, NPC target, int DamageDone)
        {
            Vector2 dir = player.velocity.ToSafeNormalize();
            ScarletSound(HJScarletSounds.SodomsDisaster_BoomHit, target.Center);
            for (int i = 0; i < 32; i++)
            {
                ECSParticle.SmokeParticle(target.Center, dir.ToRandVelocity(ToRadians(35), 1.2f, 22.5f), RandLerpColor(Color.DarkRed, Color.Black), 40, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.2f) * .52f, blendstate: BlendState.AlphaBlend);
            }
            for (int i = 0; i < 32; i++)
            {
                ECSParticle.SmokeParticle(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.Red, Color.Black), 40, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.2f) * .28f, Main.rand.NextBool(), blendstate: BlendState.NonPremultiplied);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.LiliesFire(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.Black, Color.Red), 40, RandRotTwoPi, 1, 0.2f * Main.rand.NextFloat(0.45f, 1.3f));
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1.2f, 8f), RandLerpColor(Color.DarkRed, Color.Red), 40, 1, 0.92f * Main.rand.NextFloat(0.95f, 1.3f), blendstate: BlendState.AlphaBlend);
            }
            for (int i = 0; i < 12; i++)
            {
                Vector2 vel = dir.ToRandVelocity(ToRadians(25), 1.9f, 39f);
                BloodyMetaballAlt.SpawnParticle(target.Center, vel, Main.rand.NextFloat(0.75f, 1.2f) * 0.35f, vel.ToRotation() - Pi, true);
            }
            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = dir.ToRandVelocity(ToRadians(15), 12f, 44f);
                BloodyMetaballAlt.SpawnParticle(target.Center, vel, Main.rand.NextFloat(0.75f, 1.2f) * 0.62f, vel.ToRotation(), false, true);
            }

        }
    }
}
