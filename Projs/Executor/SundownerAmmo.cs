using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class SundownerAmmo : HJScarletProj, IPixelatedRenderer
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public bool CanPlaySound = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(18);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.timeLeft = GetSeconds(10);
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.Opacity = 0;
            Projectile.extraUpdates = 1;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.Opacity = Lerp(Projectile.Opacity, 1, 0.1f);
            if (Projectile.Opacity >= 0.98f)
                Projectile.Opacity = 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(8f), Projectile.velocity / 8f, RandLerpColor(Color.DarkOrange, Color.OrangeRed), 30, RandRotTwoPi, 1f, Main.rand.NextFloat(0.12f, 0.16f) * 1.2f, true).SpawnToPriority();
            if (Main.rand.NextBool(5))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(8f), Projectile.velocity / 4f, RandLerpColor(Color.DarkOrange, Color.OrangeRed), 30, 0, 1, 0.50f, false).Spawn();
            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                d.velocity = Projectile.velocity / 4f;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
                Projectile.AddExecutionTimePreHit(ItemType<Sundowner>());
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.velocity = Projectile.SafeDir() * 17f;
            Projectile.netUpdate = true;
            Vector2 spawnPos = Projectile.Center - Projectile.SafeDir() * 10f;
            float numberOfDusts = 24f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(4.8f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                Vector2 velOffset = new Vector2(2.4f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                new ShinyOrbParticle(spawnPos + offset, velOffset + Projectile.velocity * .5f * Main.rand.NextFloat(0.8f, 1.1f), RandLerpColor(Color.DarkOrange, Color.OrangeRed), 40, 0.8f).Spawn();
            }
            for (int i = 0; i < 10; i++)
            {
                int dType = Main.rand.NextBool() ? DustID.Torch : DustID.OrangeTorch;
                Dust d = Dust.NewDustPerfect(spawnPos + Main.rand.NextVector2CircularEdge(10f, 10f), dType);
                d.velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(1.2f, 4.2f) + Projectile.velocity * Main.rand.NextFloat(0.8f, 1.1f) * .5f;
                d.scale = Main.rand.NextFloat(1.4f, 1.8f);
                d.noGravity = true;
            }
            for (int i = 0; i < 10; i++)
            {
                new SmokeParticle(spawnPos.ToRandCirclePos(10f), Projectile.SafeDir().ToRandVelocity(ToRadians(10f), 0.8f, 20.1f), RandLerpColor(Color.White, Color.OrangeRed), 40, RandRotTwoPi, Main.rand.NextFloat(.4f, 1f), 0.25f, true).SpawnToPriorityNonPreMult();
            }
            if (CanPlaySound)
            {
            bool nonStop = !Owner.HasProj<SundownerFlare>() && !Owner.HasProj<SundownerFlareGun>() && !Projectile.HJScarlet().ExecutionStrike;
                if (!nonStop)
                {
                    SlotId slotId1 = SoundEngine.PlaySound(HJScarletSounds.Sundowner_Fire with { MaxInstances = 0, Pitch = 0.05f, PitchVariance = 0.1f, Volume = 0.27f }, Projectile.Center);
                    if (SoundEngine.TryGetActiveSound(slotId1, out var sound) && !nonStop)
                        sound.Volume /= 3;
                }
            }
                //SoundEngine.PlaySound(HJScarletSounds.Misc_Boom with { Variants = [1], MaxInstances = 0, Pitch = 0.5f,Volume = .35f  }, Projectile.Center);

            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            float oriScale = .80f;
            float scale = 1f;
            int length = (int)(8 * Projectile.Opacity);
            for (int i = 0; i < length; i++)
            {
                scale *= 0.975f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.DarkOrange, Color.OrangeRed, (1 - rads)).ToAddColor(40) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads) * Projectile.Opacity;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.0f);
                float rot = Projectile.oldRot[i];
                SB.Draw(projTex, lerpPos +Projectile.SafeDir() * 5f+ Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.Opacity, 0, 0);
            }

            Projectile.DrawProj(Color.White * Projectile.Opacity, 1);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Red);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.OrangeRed, 0.75f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DarkOrange, 0.30f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -44.2f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue *Clamp(Projectile.velocity.Length() * Projectile.Opacity, 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.74f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();
            GD.Textures[1] = HJScarletTexture.Texture_Spirite.Value;
            GD.SamplerStates[1] = SamplerState.PointWrap;

            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> _, Projectile.oldPos, Projectile.oldRot);
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)(validPosition.Count * Projectile.Opacity);
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.SafeDir() * 5f+ Projectile.Size / 2, drawColor, new Vector2(0, 20 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }

    }
}
