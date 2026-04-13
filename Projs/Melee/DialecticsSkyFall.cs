using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Collections.Specialized;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Materialism = ContinentOfJourney.Projectiles.Meelee.Materialism;

namespace HJScarletRework.Projs.Melee
{
    public class DialecticsSkyFall : HJScarletFriendlyProj, IPixelatedRenderer
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => GetInstance<Materialism>().Texture;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(30, 2);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.extraUpdates = 4;
            Projectile.noEnchantmentVisuals = true;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.timeLeft = 600;
        }
        
        public override void AI()
        {
            if (Projectile.damage != 0)
            {
                if (Projectile.GetTargetSafe(out NPC target, true, 600f))
                    Projectile.HomingTarget(target.Center, 1800f, 26, 20f);
            }
            else
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0, 0.25f);
                Projectile.scale = Lerp(Projectile.scale, 0f, 0.2f);
                if(Projectile.Opacity <= 0.02f && Projectile.timeLeft > 100)
                {
                    Projectile.timeLeft = 50;
                }
            }
                Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.FinalUpdateNextBool(5) && Projectile.damage > 0)
            {
                float ratio = Main.rand.NextFloat(0.32f, 0.42f) *0.85f;
                Vector2 pos = Projectile.Center.ToRandCirclePos(16);
                Vector2 vel = Projectile.velocity.ToRandVelocity(0f, 9f, 11.4f);
                new KiraStar(pos, vel, RandLerpColor(Color.RoyalBlue, Color.LightBlue), 20, 0, 1, ratio).SpawnToPriorityNonPreMult();
                new KiraStar(pos, vel, Color.AliceBlue, 20, 0, 1, ratio * 0.80f).Spawn();
            }
            if (Projectile.FinalUpdate() && Projectile.damage > 0)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(16f);
                Vector2 vel = Projectile.velocity.ToRandVelocity(0, 9f, 11.4f);
                new HRShinyOrb(pos, vel, RandLerpColor(Color.RoyalBlue, Color.LightBlue), 20, 0, 1f, 0.1f).Spawn();
                new HRShinyOrb(pos, vel, Color.White, 20, 0, 1f, 0.05f).Spawn();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            SoundEngine.PlaySound(HJScarletSounds.Dialectics_Hit with { MaxInstances = 1, Pitch = 0.5f, PitchVariance = 0.2f ,Volume = 0.7f}, Projectile.Center);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(PiOver4 / 16 * Main.rand.NextBool().ToDirectionInt()));
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(16f);
                Vector2 vel = dir * Main.rand.NextFloat(18f, 24f);
                if (Main.rand.NextBool())
                    new StarShape(spawnPos, vel, Color.RoyalBlue, 0.8f, 40).Spawn();
            }
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            //DrawCoreStar(sb);
            for (int i = -1; i < 2; i+=2)
            {
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DarkBlue, 1.26f, 1f, 15f * i);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.RoyalBlue, 0.8f, 1f, 15f * i);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.58f, 1f, 15f * i);
            }


            HJScarletMethods.EndShaderAreaPixel();
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue * Projectile.Opacity);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 15 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
        public float GetAlphaFade(float t)
        {
            return Lerp(0.3f, 1f, t);
        }
        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(0.9f, 1.4f);
            Vector2 beginScale = new(0.1f, 0.2f);
            return Vector2.Lerp(beginScale, starScale, t) * 0.91f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            if(Projectile.damage>0)
            Projectile.DrawGlowEdge(Color.White, rotFix: PiOver4);
            Projectile.DrawProj(Color.White * Projectile.Opacity,4, rotFix: PiOver4);
            return false;
        }
    }
}
