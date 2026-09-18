using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players.Dashes
{
    public class ShadowCastDash : PlayerDashClass
    {
        public override int ImmuneTime(Player player) => 30;
        public override int DashTime(Player player) => 24;
        public override int DashDelay(Player player) => 24;
        public override DashEnum DashOnHitType => DashEnum.Slam;
        public override DashDamageInfo DashDamageInfo(Player player)
        {
            return new DashDamageInfo(10, 3f, DamageClass.Generic);
        }
        public override float DashSpeed(Player player) => 32f;
        public override float DashEndSpeedMult(Player player) => 0.5f;
        public override void OnDashStart(Player player)
        {
        }
        public override void OnDashEnd(Player player)
        {
            base.OnDashEnd(player);
        }
        public override void UpdateDash(Player player)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = player.ToRandRec();
                Vector2 dir = player.velocity.ToSafeNormalize() * -1f;
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 1.7f);
                int lifeTime = Main.rand.Next(30, 70);
                pos += player.velocity.ToSafeNormalize() * 20f;
                ECSParticle.LiliesFire(pos, vel.RotatedBy(ToRadians(15f) * i), Color.Black, lifeTime, RandRotTwoPi, 1, 0.3f, true);
                Vector2 stainPos = pos.ToRandCirclePos(6);
                float stainScale = Main.rand.NextFloat(.9f, 1.1f) * .4f;
                ECSParticle.ShrinkParticle(stainPos, vel, Color.Black, 45, 0.9f, dir.ToRotation(), stainScale, 1, new Vector2(2.0f, 1.4f), 0, 0, BlendState.NonPremultiplied);
            }
            for (int i = -1; i < 2; i+=2)
            {
                Vector2 pos = new Vector2(0, 20*i).RotatedBy(player.velocity.ToRotation())+player.Center;
                pos += player.velocity.ToSafeNormalize() * -5f;
                ECSParticle.ShrinkParticle(pos, -player.velocity / 8, Color.Black, 45, .7f, player.velocity.ToRotation()+PiOver2, 0.4f, 0, new Vector2(0.5f, 1.2f), 0, 0, BlendState.NonPremultiplied);
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = player.ToRandRec();
                pos += player.velocity.ToSafeNormalize() * 20f;
                ECSParticle.TurbulenceShinyOrb(pos, 1.2f, Color.Black, 45, 1, 0.5f, blendState: BlendState.NonPremultiplied);
            }

        }
        public override void OnHitNPC(Player player, NPC target, int DamageDone)
        {
            target.HJScarlet().isBeingShadowCast = GetSeconds(5);
        }
    }
}
