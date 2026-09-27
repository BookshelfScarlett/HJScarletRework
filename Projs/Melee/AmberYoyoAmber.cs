using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class AmberYoyoAmber : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Item, ItemID.Amber);
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);

        }
        public override void ExSD()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.MaxUpdates = 4;
            Projectile.width = Projectile.height = 16;
            Projectile.SetupImmnuity(60);
            Projectile.timeLeft = 300 * Projectile.MaxUpdates;
            Projectile.penetrate = 3;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft -= 50 * Projectile.MaxUpdates;
            ScarletSound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(6), RandVelTwoPi(1f, 4f), RandLerpColor(Color.DarkOrange, Color.Orange), 45, 1, Projectile.scale * Main.rand.NextFloat(.75f, 1.15f) * .6f, .2f);
            }
            ECSParticle.HRShinyOrb(Projectile.Center, Vector2.Zero, Color.Orange, 40, 1, 0.2f, 0.4f);

            //Projectile.BounceOnTile(oldVelocity);
            return true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override bool? CanDamage()
        {
            return true;
        }
        public override void ProjAI()
        {
            Projectile.rotation += .15f;
            Timer++;
            if (Timer > Projectile.MaxUpdates * 10f)
                Projectile.tileCollide = true;
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(8))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(6), Projectile.velocity / 4f, RandLerpColor(Color.DarkOrange, Color.Orange), 45, 1, Projectile.scale * Main.rand.NextFloat(.75f, 1.15f) * .6f, .2f);
            if (Main.rand.NextBool(8))
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePosEdge(6), Projectile.velocity / 4f, Color.Orange, 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .75f, 0, 0.1f);
            base.ProjAI();
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.penetrate == 0)
                return;
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePosEdge(6), (-Projectile.oldVelocity).ToRandVelocity(0, 1f, 7), Color.Orange, 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .75f, 0, 0.1f);
            }
            ECSParticle.HRShinyOrb(Projectile.Center, Vector2.Zero, Color.Orange, 40, 1, 0.2f, 0.4f);
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ScarletSound(HJScarletSounds.TheSevenStar_Hit, Projectile.Center, .4f);
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePosEdge(6), RandVelTwoPi(1f, 7), Color.Orange, 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .75f, 0, 0.1f);
            }
            ECSParticle.HRShinyOrb(Projectile.Center, Vector2.Zero, Color.Orange, 40, 1, 0.2f, 0.4f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int drawLength = Projectile.oldPos.Length;
            int length = Projectile.oldPos.Length;
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.Orange, 1f, 1f, 1.1f);
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.DarkOrange, 0.9f, 1f, 1.1f);
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.White, 1f, 1f, 1.1f);
            SB.EnterShaderArea();
            Texture2D glowTex = HJScarletTexture.Particle_CrossGlow.Value;
            SB.FastDraw(glowTex, drawPos.ToRandCirclePos(1.5f, 1.5f), Color.Crimson, 0, glowTex.Size() / 2f, Projectile.scale * Main.rand.NextFloat(.9f, 1.01f) * .15f, 0);
            SB.EndShaderArea();
            SB.EndShaderArea();

            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0f);
                Vector2 oldPos = lerpPos - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i];
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .55f, (progress));
                float yMult = Lerp(1f, .55f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.Orange, Color.DarkOrange, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .3f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.OrangeRed, Color.DarkOrange, .63f), EaseInOutQuad(progress));
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), pixelColor.ToAddColor(50) * opac * 0.815f, oldRot, projTex.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(projTex, oldPos, c.ToAddColor(0) * opac * .315f, oldRot, projTex.Size() / 2f, scale * .98f, 0);
            }

            for (int i = 0; i < 8; i++)
                SB.FastDraw(projTex, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            SB.FastDraw(projTex, drawPos, Color.White, Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -0f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * .85f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.05f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = Main.rand.NextVector2Circular(1.5f, 1.5f);
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 9 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }

    }
}
