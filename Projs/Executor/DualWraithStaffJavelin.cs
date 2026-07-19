using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DualWraithStaffJavelin : HJScarletProj, IPixelatedRenderer
    {
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override ClassCategory Category => ClassCategory.Executor;
        public Vector2 DrawOffset => Projectile.SafeDir() * 80f;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20);
        }
        public override void ExSD()
        {
            Projectile.SetupImmnuity(60);
            Projectile.extraUpdates = 4;
            Projectile.width = Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 8;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            if (Projectile.velocity.Length() < 24)
                Projectile.velocity = Projectile.SafeDir() * 24;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            Vector2 projPos = Projectile.Center - DrawOffset;
            if (Main.rand.NextBool(4))
            {
                if (Main.rand.NextBool())
                {
                    ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                    {
                        p.Position = projPos.ToRandCirclePos(30);
                        p.Velocity = Projectile.velocity / 3f;
                        p.DrawColor = RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue);
                        p.Scale = Main.rand.NextFloat(0.75f, 1.01f) * 0.13f;
                        p.Opacity = 1f;
                        p.Lifetime = Main.rand.Next(40, 70);
                        p.GlowCenterMult = 0.70f;
                    });
                }
                else
                    new ShinyCrossStar(projPos.ToRandCirclePos(25), Projectile.velocity / 3f, RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), Main.rand.NextFromList(40, 70), 0, 1, Main.rand.NextFloat(0.75f, 1.01f) * 0.73f).Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(ItemType<DualWraithStaff>(), Main.rand.Next(1, 3));
            if (Projectile.numHits < 1)
            {
                if (Projectile.ai[1] == 0)
                    SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Hit with { MaxInstances = 1, Variants = [1], Volume = 0.8f, Pitch = -0.5f }, Projectile.Center);
            }
            float randRod = Projectile.SafeDir().ToRandVelocity(ToRadians(10f), 1).ToRotation();
            if (Projectile.ai[1] == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Projectile.SafeDir().RotatedBy(ToRadians(120) * i + randRod);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, dir * 16f, ProjectileType<DualWraithStaffGhost>(), Projectile.damage / 2, Projectile.knockBack, Owner.whoAmI);
                    proj.HJScarlet().HasExecutionMechanic = false;
                    proj.ai[1] = Main.rand.NextBool().ToDirectionInt();
                }
            }
            for (int i = 0; i < 24; i++)
            {
                ScarletParticle.Spawn<HRShinyOrbAlt>(p =>
                {
                    p.Position = target.Center.ToRandCirclePos(8);
                    p.Velocity = RandVelTwoPi(1.2f, 12.5f);
                    p.Scale = 0.1f * Main.rand.NextFloat(0.9f, 1.15f);
                    p.Lifetime = 40;
                    p.DrawColor = RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue);
                    p.Opacity = 1;
                    p.GlowCenterMult = 0.75f;

                });
            }
            for (int i = 0; i < 36; i++)
            {
                new SmokeParticle(target.Center.ToRandCirclePos(8f), RandVelTwoPi(0.1f, 9.4f), RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.5f, Main.rand.NextFloat(0.8f, 1.2f) * 0.32f, Main.rand.NextBool()).Spawn();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            float rotFixer = ToRadians(62.5f);
            Projectile.DrawGlowEdge(Color.White, rotFix: rotFixer, drawPosOffset: DrawOffset);
            Projectile.DrawProj(Color.White, drawTime: 1, rotFix: rotFixer, drawPosOffset: DrawOffset);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public void RenderPixelated(SpriteBatch sb)
        {

            HJScarletMethods.EnterShaderAreaPixel(BlendState.AlphaBlend);

            Effect effect2 = HJScarletShader.AlphaFadeNoiseColor;
            int length = Projectile.oldPos.Length;
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 offset = Projectile.SafeDir().RotatedBy(PiOver2) * 3.1f;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.RoyalBlue, Color.WhiteSmoke, rads)) * 0.7f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Vector2 oldPosOffset = -DrawOffset - Projectile.SafeDir() * (1f + 5f * i) - offset;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.26f);
                SB.Draw(star, lerpPos + Projectile.PosToCenter() + oldPosOffset, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * 1f * new Vector2(1f, 1.5f), 0, 0);
            }
            for (int k = 0; k < length; k++)
            {
                float rads2 = (float)k / length;
                Color drawColor = (Color.Lerp(Color.SkyBlue, Color.White, rads2)) * 0.7f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads2);
                Vector2 oldPosOffset = -DrawOffset - Projectile.SafeDir() * (5f + 5f * k) - offset;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[k], Projectile.oldPos[0], 0.26f);
                SB.Draw(star, lerpPos + Projectile.PosToCenter() + oldPosOffset, null, drawColor, Projectile.rotation - PiOver2, star.Size() / 2, Projectile.scale * 1f * new Vector2(1f, 1.5f), 0, 0);
            }
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawTrail(HJScarletTexture.Trail_Lightning0.Texture, 0.82f, Color.LightSkyBlue);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawTrail(Asset<Texture2D> trail, float multValue, Color color)
        {
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(trail.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, trail.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -18.2f);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * Projectile.Opacity * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.15f);
            shader.CurrentTechnique.Passes[0].Apply();
            DrawSetting sets = new(trail.Value);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 + Projectile.SafeDir() * 10f;
                float ratios = i / (float)Projectile.oldPos.Length;
                date.Add(new(listPos, Color.White, new(0, 40 * multValue * Clamp((1 - ratios), 0.92f, 1f)), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }
    }
}
