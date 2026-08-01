using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class ChlorophyteCrystalExecutor : HJScarletProj, IPixelatedRenderer
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.CrystalLeaf);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public ref float Timer => ref Projectile.ai[0];
        public List<Vector2> CenterPosList = [];
        private Vector2 TopLeftPoint = new Vector2(0, 0);
        private Vector2 TopRightPoint = new Vector2(30, -100);
        private Vector2 BottomLeftPoint = new Vector2(30, 100);
        private Vector2 BottomRightPoint = new Vector2(0, 0);
        private Vector4 RandValueSummary = new Vector4();

        public override void OnFirstFrame()
        {
            RandValueSummary = new Vector4(Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(0.7f, 1.1f));
            float maxPoints = 60;
            for (int i = 0; i < maxPoints; i++)
            {
                float progress = i / maxPoints;
                Vector2 finalPos = Vector2.CatmullRom(TopLeftPoint, TopRightPoint, BottomLeftPoint, BottomRightPoint, progress);
                finalPos.X *= 0.9f;
                finalPos.Y *= 0.18f;
                CenterPosList.Add(finalPos.RotatedBy(PiOver2));
            }
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj();
        }
        public override void ProjAI()
        {
            Vector2 targetVec = Owner.MountedCenter - Vector2.UnitY * 80f;
            targetVec.Y += Owner.gfxOffY;
            Projectile.position.X = Owner.position.X;
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetVec, .22f);
            if(Main.rand.NextBool())
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(10, 20), -Vector2.UnitY * Main.rand.NextFloat(.8f, 1.2f), RandLerpColor(Color.White, Color.LightGreen), 40, 1, 0.3f,.2f);
            }
            if (Owner.HJScarlet().chlorophyteHeadExecutor)
                Projectile.timeLeft = 2;
            Timer++;
            if (Timer > 60)
            {

                if (Projectile.IsMe())
                {
                    float searchDist = 1100f;
                    List<NPC> availableTarget = [];
                    foreach (NPC needTar in Main.ActiveNPCs)
                    {
                        if (availableTarget.Count >= 3)
                            break;
                        bool legalTarget = needTar.CanBeChasedBy() && (Collision.CanHitLine(needTar.Center, 1, 1, Projectile.Center, 1, 1));
                        float distPerTar = Vector2.Distance(needTar.Center, Projectile.Center);
                        if (legalTarget && distPerTar < searchDist)
                        {
                            availableTarget.Add(needTar);
                        }
                    }
                    if (availableTarget.Count == 0)
                    {
                        return;
                    }
                    ScarletSound(HJScarletSounds.Misc_ManaClearUse, Projectile.Center, 0.4f, pitch: .7f, pitchVariance: .2f);
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 dir = Projectile.velocity.ToSafeNormalize();
                        float rotvalue = ToRadians(360f / 4f * i) * 1f;
                        float scale = (i % 2 == 0) ? 0.5f : 0.35f;
                        for (int j = 0; j < 3; j++)
                        {
                            ECSParticle.LightntingGlow(Projectile.Center + dir.RotatedBy(rotvalue) * j * 1.5f, dir.RotatedBy(rotvalue), Color.DarkSeaGreen, 50, 1, scale);
                            ECSParticle.LightntingGlow(Projectile.Center + dir.RotatedBy(rotvalue) * j * 1.5f, dir.RotatedBy(rotvalue), Color.ForestGreen, 50, 1, scale);
                        }
                    }
                    for (int i = 0; i < 16; i++)
                    {

                        ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(16), 1.18f, RandLerpColor(Color.LimeGreen, Color.Green), Main.rand.Next(80, 90), 1, Main.rand.NextFloat(0.85f, 1.15f) * .1f * 1.6f, glowMult: .45f);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        ECSParticle.LiliesFire(Projectile.Center + RandVelTwoPi(2f, 12.4f), RandVelTwoPi(0.8f, 5.1f), RandLerpColor(Color.Black, Color.DarkViolet), 60, RandRotTwoPi, 1f, 0.24f, true, Microsoft.Xna.Framework.Graphics.BlendState.Additive);
                    }
                    ECSParticle.CrossGlow(Projectile.Center, Color.LawnGreen, 30, .75f, 0.4f);
                    ECSParticle.CrossGlow(Projectile.Center, Color.Green, 30, .75f, 0.3f);
                    ECSParticle.CrossGlow(Projectile.Center, Color.White, 30, .75f, 0.2f);
                    ScarletSound(HJScarletSounds.Misc_ManaClearUse, Projectile.Center, 0.4f, pitch: .7f, pitchVariance: .2f);
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 dir = Projectile.velocity.ToSafeNormalize();
                        float rotvalue = ToRadians(360f / 4f * i) * 1f;
                        float scale = (i % 2 == 0) ? 0.5f : 0.35f;
                        for (int j = 0; j < 3; j++)
                        {
                            ECSParticle.LightntingGlow(Projectile.Center + dir.RotatedBy(rotvalue) * j * 1.5f, dir.RotatedBy(rotvalue), Color.DarkSeaGreen, 50, 1, scale);
                            ECSParticle.LightntingGlow(Projectile.Center + dir.RotatedBy(rotvalue) * j * 1.5f, dir.RotatedBy(rotvalue), Color.ForestGreen, 50, 1, scale);
                        }
                    }
                    for (int i = 0; i < 16; i++)
                    {

                        ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(16), 1.18f, RandLerpColor(Color.LimeGreen, Color.Green), Main.rand.Next(80, 90), 1, Main.rand.NextFloat(0.85f, 1.15f) * .1f * 1.6f, glowMult: .45f);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        ECSParticle.LiliesFire(Projectile.Center + RandVelTwoPi(2f, 12.4f), RandVelTwoPi(0.8f, 5.1f), RandLerpColor(Color.Black, Color.DarkViolet), 60, RandRotTwoPi, 1f, 0.24f, true, Microsoft.Xna.Framework.Graphics.BlendState.Additive);
                    }
                    ECSParticle.CrossGlow(Projectile.Center, Color.LawnGreen, 30, .75f, 0.4f);
                    ECSParticle.CrossGlow(Projectile.Center, Color.Green, 30, .75f, 0.3f);
                    ECSParticle.CrossGlow(Projectile.Center, Color.White, 30, .75f, 0.2f);

                    for (int i = 0; i < availableTarget.Count; i++)
                    {
                        Vector2 dir = Projectile.Center.GetNormalVector2(availableTarget[i].Center);
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * 10f, ProjectileType<ChlorophyteCrystalBolt>(), Projectile.originalDamage, 2f, Owner.whoAmI);
                    }

                }
                Timer = 0;
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            SB.EnterShaderArea();
            Texture2D glow = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            SB.Draw(glow, pos, null, Color.DarkGreen, 0, glow.Size() / 2, Projectile.scale, 0, 0);
            SB.EndShaderArea();

            Texture2D tex = Projectile.GetTexture();
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, pos + (TwoPi / 8 * i).ToRotationVector2() * 2f, null, Color.Green.ToAddColor(10), 0, tex.Size() / 2, Projectile.scale, 0, 0);
            SB.Draw(tex, pos, null, Color.White.ToAddColor(220), 0, tex.Size() / 2, Projectile.scale, 0, 0);
            return false;
        }
        public BlendState BlendState => BlendState.Additive;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D tex = HJScarletTexture.Texture_StandardGradient.Value;
            Effect e = HJScarletShader.AlphaFade;
            e.Parameters["uFadeoutLeftLength"].SetValue(0.3f);
            e.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            e.Parameters["uFadeinTopLength"].SetValue(0.3f);
            e.Parameters["uFadeinBottomLength"].SetValue(0.5f);
            e.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            e.CurrentTechnique.Passes[0].Apply();
            TrailFunc(tex, Color.DarkGreen* 0.60f, 9f);
            TrailFunc(tex, Color.Green* 0.20f, 6f);
            TrailFunc(tex, Color.Lime* .20f, 3f);
            TrailFunc(tex, Color.White * .20f, 2f);

            Vector4 vector4 = new(0.2f, 0.2f, 0.1f, 0.6f);
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 1.79f * RandValueSummary.X), new Vector2(1f, 0.985f), Color.White);
            Texture2D texture2 = HJScarletTexture.Noise_Aura.Value;
            TrailFunc(texture2, Color.ForestGreen, 10f);

            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 2.79f * RandValueSummary.Y), new Vector2(2f, 0.975f), Color.White);
            texture2 = HJScarletTexture.Noise_Misc.Value;
            TrailFunc(texture2, Color.White * 0.42f, 10f);
            texture2 = HJScarletTexture.Noise_Aura.Value;

            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.79f * RandValueSummary.Z), new Vector2(3.2f, 1.94f), Color.White);
            TrailFunc(texture2, Color.White* 0.92f, 20f);
            TrailFunc(texture2, Color.White * 0.92f, 10f);
            ApplyTrailAlt(HJScarletTexture.Trail_ManaStreakTiny.Value, Color.DarkGray);
            ApplyTrailAlt(HJScarletTexture.Trail_FadedStreak.Value, Color.Gray, 10);
            ApplyTrailAlt(HJScarletTexture.Trail_TerraRayFlow.Value, Color.WhiteSmoke, 28);

            HJScarletMethods.EndShaderAreaPixel();

        }
        public void ApplyTrailAlt(Texture2D tex, Color color, float primitiveHeight = 30, float heightPosOffset = 0f)
        {
            float laserLength = 150;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, tex.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -150);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * 1);
            shader.Parameters["uFadeoutLength"].SetValue(1.13f);
            shader.Parameters["uFadeinLength"].SetValue(0.12f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting sets = new(tex);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 + Projectile.SafeDir().RotatedBy(PiOver2) * heightPosOffset;
                date.Add(new(listPos, Color.White, new(0, primitiveHeight * 1.92f), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }
        public void TrailFunc(Texture2D tex, Color c, float mult)
        {
            List<ScarletVertex> vertices = new List<ScarletVertex>();
            Vector2 dir = (-Vector2.UnitY) * 12f * mult;
            for (int i = 0; i < CenterPosList.Count; i++)
            {
                float progress = (float)i / CenterPosList.Count;
                Vector2 pos = Projectile.Center - Main.screenPosition - Vector2.UnitY * 15 - Vector2.UnitX;
                Vector2 posHead = CenterPosList[i] + pos;
                Vector2 posSrc = CenterPosList[i] + pos + dir * Projectile.Opacity;
                vertices.Add(new ScarletVertex(posHead, c, new Vector3(progress, 0, 0)));
                vertices.Add(new ScarletVertex(posSrc, c, new Vector3(progress, 1, 0)));
            }
            if (vertices.Count < 3)
                return;
            GD.Textures[0] = tex;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
        }
    }
}
