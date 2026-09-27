using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class RubyYoyoRuby : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Item, ItemID.Ruby);
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
            Projectile.MaxUpdates = 2;
            Projectile.width = Projectile.height = 16;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void ProjAI()
        {
            Projectile.rotation += .15f;
            if (Projectile.damage != 0 && Projectile.timeLeft > 50)
            {
                Projectile.scale = Lerp(Projectile.scale, 1.01f, .12f);
                if (Projectile.scale > 1 && Timer > 45)
                {
                    if (!Owner.dead)
                    {
                        float speedValue = Projectile.velocity.Length();
                        float rotation = Projectile.velocity.ToRotation();
                        float angleTo = Projectile.AngleTo(Owner.Center);
                        float dist = Projectile.Distance(Owner.Center);
                        float r = dist * 0.30f / (float)Math.Abs(Math.Sin(rotation - angleTo));
                        if (Vector2.Dot(Projectile.velocity, Projectile.DirectionTo(Owner.Center)) < 0)
                        {
                            r = Clamp(r, 1, 240);
                        }
                        if (Owner.Hitbox.Intersects(Projectile.Hitbox))
                        {
                            Owner.Heal(1);
                            Projectile.Kill();
                        }
                        Projectile.velocity = Projectile.velocity.RotatedBy(-Math.Sign(WrapAngle(rotation - angleTo)) * speedValue / r);
                        if (Projectile.velocity.LengthSquared() < 11f * 11f)
                            Projectile.velocity *= 1.01f;
                        else
                            Projectile.velocity *= 0.9f;
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    if (Projectile.velocity.LengthSquared() >= 12f * 12f)
                        Projectile.velocity *= 0.9f;
                }
            }
            else
            {
                Projectile.scale = Lerp(Projectile.scale, 0f, .12f);
                if (Projectile.scale <= 0.2f)
                    Projectile.Kill();
            }

            Timer++;

            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(8))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(6), Projectile.velocity / 4f, RandLerpColor(Color.Red, Color.DarkRed), 45, 1, Projectile.scale * Main.rand.NextFloat(.75f, 1.15f) * .6f, .2f);
            if (Main.rand.NextBool(8))
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePosEdge(6), Projectile.velocity / 4f, Color.Crimson, 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .75f, 0, 0.1f);
            base.ProjAI();
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePosEdge(6), (-Projectile.oldVelocity).ToRandVelocity(0, 1f, 7), Color.Crimson, 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .75f, 0, 0.1f);
            }
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int drawLength = Projectile.oldPos.Length;
            int length = Projectile.oldPos.Length;
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.DarkRed, 1f, 1f, 1.1f);
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.Crimson, 0.9f, 1f, 1.1f);
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
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.DarkRed, Color.Crimson, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .3f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.DarkRed, Color.Lerp(Color.Crimson, Color.Red, .63f), EaseInOutQuad(progress));
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), pixelColor.ToAddColor(50) * opac * 0.815f, oldRot, projTex.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(projTex, oldPos, c.ToAddColor(0) * opac * .315f, oldRot, projTex.Size() / 2f, scale * .98f, 0);
            }

            for (int i = 0; i < 8; i++)
                SB.FastDraw(projTex, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.Red.ToAddColor(), Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
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
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 6 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
    }
}
