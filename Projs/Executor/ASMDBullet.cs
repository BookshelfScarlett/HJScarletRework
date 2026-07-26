using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class ASMDBullet : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public bool IsHitWall = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(30);
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.extraUpdates = 5;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!IsHitWall)
            {
                IsHitWall = true;
                Projectile.tileCollide = false;
            }
            return false;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            //这里的处理是因为，现在使用的轨迹非常长，如果直接处死的话会让轨迹瞬间消失
            //因此这里会用非常快的lerp来减少这个方面的违和感
            //后续会考虑将轨迹绘制外置
            if (IsHitWall)
            {
                if (Projectile.Opacity == 1)
                {
                    BoomParticle();
                }
                Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.2f);
                Projectile.velocity *= .001f;
                if (Projectile.Opacity <= .20f)
                    Projectile.Kill();
            }
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(3))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePos(8), Projectile.velocity / 8f, RandLerpColor(Color.CornflowerBlue, Color.White), 60, 1, Projectile.scale * Main.rand.NextFloat(.9f, 1.1f) * .40f, 6);
            if (Main.rand.NextBool())
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(8), Projectile.velocity / 8f, RandLerpColor(Color.SkyBlue, Color.CornflowerBlue), 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .5f, 0.2f);
        }
        public void BoomParticle()
        {
            ScarletSound(HJScarletSounds.Frostwave_Boom, Projectile.Center, 0.30f, 1, 0.75f);
            ScarletSound(HJScarletSounds.Frosthammer_SnowCharge, Projectile.Center, 0.30f, 1, -0.75f);
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(15);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(1.2f, 12f);
                ECSParticle.LightntingGlow(pos, vel, RandLerpColor(Color.White, Color.SkyBlue), Main.rand.Next(40, 70), 1f, Main.rand.NextFloat(.75f, 1.1f) * .75f, Main.rand.Next(2, 5));
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(4.2f, 14.8f);
                bool value = Main.rand.NextBool();
                BlendState bs = value ? BlendState.Additive : BlendState.AlphaBlend;
                ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.SkyBlue, Color.White), Main.rand.Next(35, 55), RandRotTwoPi, .95f, 0.40f * Main.rand.NextFloat(.7f, 1.1f), value, bs);
            }
            for (int i = 0; i < 24; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(10);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(.42f, 14.8f);
                ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.White, Color.SkyBlue), Main.rand.Next(35, 55), RandRotTwoPi, .81f, 0.154f * Main.rand.NextFloat(.7f, 1.1f), BlendState.Additive);
            }
            for (int i = 0; i < 18; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(60);
                ECSParticle.TurbulenceShinyOrb(pos, 6.4f, RandLerpColor(Color.White, Color.RoyalBlue), 60, 1, .17f * Main.rand.NextFloat(.85f, 1.1f), glowMult: 0.5f);
            }
            for (int i = 0; i < 44; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(10);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(1.2f, 9.8f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.White, Color.RoyalBlue), 35, 1f, 0.95f * Main.rand.NextFloat(.75f, 1.15f), 0.2f);
            }
            ECSParticle.CrossGlow(Projectile.Center, Color.RoyalBlue, 30, .81f, .48f);
            ECSParticle.CrossGlow(Projectile.Center, Color.White, 30, .81f, .45f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= .01f;
            IsHitWall = true;
            Projectile.AddExecutionTimeImmediate(ItemType<ASMD>());
            foreach (var activeProj in Main.ActiveProjectiles)
            {
                if (activeProj.type != ProjectileType<ASMDIceBlock>())
                    continue;
                if (activeProj.owner != Projectile.owner)
                    continue;
                bool state = ((ASMDIceBlock)activeProj.ModProjectile).AttackState == ASMDIceBlock.State.Idle;
                if (!state)
                    continue;
                if (target.IsLegal())
                    ((ASMDIceBlock)activeProj.ModProjectile).CurTarget = target;
                break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            float rotFixer = PiOver2;
            float rot = Projectile.rotation + rotFixer;
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(projTex, drawPos + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, Color.RoyalBlue.ToAddColor(), rot, ori, Projectile.scale, 0, 0);
            }
            int length = (int)(Projectile.oldPos.Length * Projectile.Opacity);
            for (int i = length - 1; i > 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2;
                float oldrot = Projectile.oldRot[i] + rotFixer;
                float ratios = i / (float)length;
                Color c = Color.Lerp(Color.MediumAquamarine, Color.Transparent, ratios);
                float opac = Lerp(.65f, 0.45f, ratios) * Clamp(Projectile.velocity.Length(), 0, 1);
                float oldScale = Lerp(Projectile.scale * .55f, Projectile.scale * .10f, ratios);
                c *= opac;
                for (int j = 0; j < 4; j++)
                {
                    SB.Draw(projTex, oldPos + (TwoPi / 4f * j).ToRotationVector2() * 2, null, c.ToAddColor(), oldrot, ori, oldScale, 0, 0);
                }
                SB.Draw(projTex, oldPos, null, c, rot, ori, oldScale, 0, 0);
            }
            SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Blue, 0.3265f, 0.95f);
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.RoyalBlue, 1.5f, 0.95f);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.CornflowerBlue, 1.25f, .95f, 1.5f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.315f, .695f, 1.25f);
            SB.EndShaderArea();
            SB.Draw(projTex, drawPos, null, Color.White, rot, ori, Projectile.scale, 0, 0);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(useTex.Width(), useTex.Height()));
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 210f * offsetHeight);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.06f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity;

            int posCount = (int)((Projectile.oldPos.Length) * rad);
            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 vec = Projectile.oldRot[j].ToRotationVector2().RotatedBy(PiOver2);
                    Vector2 drawPos = Projectile.oldPos[j] + Projectile.Size / 2 + vec * -1.2f;
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 35 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }
}
