using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class TerraSpearPortal : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public bool IsShooted = false;
        public ref float Timer => ref Projectile.ai[0];
        public override ClassCategory Category => ClassCategory.Melee;

        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.86f;

            if (IsShooted)
                DoShooted();
            else
                DoAttacked();

            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool())
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(18f * Projectile.scale), RandVelTwoPi(1f), RandLerpColor(Color.LimeGreen, Color.DarkGreen), 100, RandRotTwoPi, 1f, 0.3f, 0.2f).Spawn();
            if (Main.rand.NextBool(4))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(16f * Projectile.scale), RandVelTwoPi(1f), RandLerpColor(Color.Lime, Color.LimeGreen), 100, RandRotTwoPi, 1, 0.13f).Spawn();

        }
        public void DoAttacked()
        {

            Timer++;
            if (Timer < 30f)
                return;

            if (Projectile.GetTargetSafe(out NPC target, canPassWall: true))
            {

                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.Center.GetNormalVector2(target.Center) * 5f, ProjectileType<TerraSpearArrow>(), Projectile.originalDamage, 2f, Owner.whoAmI);
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                DrawArrowSpawnDust();
            }
            IsShooted = true;
            Timer = 0;

        }

        private void DoShooted()
        {
            Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.1f);
            Projectile.scale = Lerp(Projectile.scale, 0f, 0.1f);
            if (Projectile.Opacity < 0.22f && Projectile.scale < 0.22f)
                Projectile.Kill();
        }

        private void DrawArrowSpawnDust()
        {
            if (Projectile.IsOutScreen())
                return;

            new CrossGlow(Projectile.Center, Color.Green, 40, 1f, 0.2f).Spawn();
            new CrossGlow(Projectile.Center, Color.White, 40, 1f, 0.1f).Spawn();
            int count = 36;
            for (int i = 1; i <= count; i++)
            {
                //生成一组圆环圈
                Vector2 dir = -Vector2.UnitY.RotatedBy(ToRadians(360 / count * i));
                new TurbulenceGlowOrb(Projectile.Center + dir * 1.2f, 1.2f, RandLerpColor(Color.DarkGreen, Color.PaleGreen), 40, 0.08f, dir.ToRotation()).SpawnToPriority();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D bloomEdge = HJScarletTexture.Texture_BloomShockwave.Value;
            Texture2D crossGlow = HJScarletTexture.Particle_HRStar.Value;
            Texture2D backgroundCenter = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            Texture2D circle = HJScarletTexture.Texture_SoftCircleEdge.Value;
            float scaleLerp = Projectile.Opacity * Projectile.scale;
            //底图处理
            SB.Draw(backgroundCenter, drawPos, null, Color.Green, 0, backgroundCenter.ToOrigin(), 0.02f * scaleLerp, 0, 0);
            SB.EnterShaderArea();
            //光圈，叠加
            SB.Draw(bloomEdge, drawPos, null, Color.ForestGreen, 0, bloomEdge.ToOrigin(), 0.05f * scaleLerp, 0, 0);
            SB.Draw(bloomEdge, drawPos, null, Color.ForestGreen, 0, bloomEdge.ToOrigin(), 0.05f * scaleLerp, 0, 0);
            //绘制辉光
            SB.Draw(circle, drawPos, null, Color.LightGreen, 0, circle.Size() / 2, 0.12f * scaleLerp, 0, 0);
            SB.Draw(crossGlow, drawPos, null, Color.Lerp(Color.Green, Color.Silver, 0.5f), 0, crossGlow.Size() / 2, 0.18f * scaleLerp, 0, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
