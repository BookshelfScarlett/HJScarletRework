using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class SpectreStaffGhost : HJScarletProj, IPixelatedRenderer
    {
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        public NPC CurTarget = null;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 1;
            Projectile.Opacity = 0;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public float InitSpeed = 0;
        public override void OnFirstFrame()
        {
            InitSpeed = Projectile.velocity.Length();
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            //Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.GetTargetSafe(out NPC target, searchDistance: 1200f) && CurTarget is null)
            {
                CurTarget = target;
            }
            CurTarget = CurTarget.IsLegal() ? CurTarget : null;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.penetrate == -1 && Projectile.damage == 0)
            {
                Projectile.velocity *= 0.06f;
                Projectile.Opacity -= 0.1f;
                if (Projectile.Opacity <= 0.03f)
                {
                    Projectile.Kill();
                    return;

                }
            }
            else
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 1.1f, 0.12f);
                if (Main.rand.NextBool(8))
                    new SnowCloud(Projectile.Center.ToRandCirclePos(10f), Projectile.velocity / 4f, RandLerpColor(Color.White, Color.LightSkyBlue), 20, RandRotTwoPi, 0.7f, Projectile.scale * 0.064f * Main.rand.NextFloat(0.75f, 1.2f)).Spawn();
                if (Main.rand.NextBool(6))
                    new ShinyCrossStar(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 4f, RandLerpColor(Color.LightSkyBlue, Color.SkyBlue), 40, 0, 1 * Projectile.Opacity, 0.8f * Projectile.Opacity, false).Spawn();
                Timer++;
                if(Projectile.MeetMaxUpdatesFrame(Timer, 5))
                {
                    if (CurTarget.IsLegal())
                        Projectile.HomingTarget(CurTarget.Center, -1, InitSpeed, 20f, 5f);
                    else
                    {
                        if (Projectile.velocity.LengthSquared() < InitSpeed * InitSpeed)
                            Projectile.velocity *= 1.1f;
                        else Projectile.velocity *= 0.9f;
                    }
                }

            }
            Vector2 baseSpawnPos = Projectile.Center - Projectile.SafeDir() * 27f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelatedRenderManager.BeginDrawProj = true;
            Texture2D sp = TextureAssets.Npc[NPCID.DungeonSpirit].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle src = sp.Frame(1, 3, 0, 1);
            float scale = Lerp(0.91f, 0.98f, (float)(Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly)))) * Projectile.scale * .85f * Projectile.Opacity;

            for(int i =0;i<16;i++)
            SB.Draw(sp, drawPos + ToRadians(360f / 16 * i).ToRotationVector2() * 2, src, Color.White.ToAddColor(), Projectile.rotation - PiOver2, src.Size()/2, scale, 0, 0);
            SB.Draw(sp, drawPos, src, Color.White, Projectile.rotation - PiOver2, src.Size()/2, scale, 0, 0);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public void RenderPixelated(SpriteBatch sb)
        {

            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D orb = HJScarletTexture.Texture_Spirite.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float scale = Projectile.scale * 0.40f * Lerp(1f, 1.12f, (float)(Math.Abs(Math.Sin( Main.GlobalTimeWrappedHourly)))) * Projectile.Opacity;
            Vector2 newVec = new(1);
            float rot = Main.GlobalTimeWrappedHourly * 1.2f + Projectile.rotation;
            SB.Draw(orb, drawPos, null, Color.DeepSkyBlue* 1f, rot, orb.ToOrigin(), scale * newVec, 0, 0);
            orb = HJScarletTexture.Particle_HRShinyOrb.Value;
            SB.Draw(orb, drawPos, null, Color.White* 1f, rot, orb.ToOrigin(), scale * newVec * 0.85f, 0, 0);

            DrawTrail(HJScarletTexture.Trail_ManaStreak.Texture, 1f, Color.DeepSkyBlue * 0.65f);
            DrawTrail(HJScarletTexture.Trail_Lightning0.Texture, 0.8f, Color.LightSkyBlue *0.95f);
            DrawTrail(HJScarletTexture.Trail_Lightning0.Texture, .5f, Color.White * 0.85f);
            HJScarletMethods.EndShaderAreaPixel();
            
        }
       
        public void DrawTrail(Asset<Texture2D> trail, float multValue, Color color)
        {
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(trail.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, trail.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -18.2f);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * Projectile.Opacity  * Clamp(Projectile.velocity.Length(), 0f, 1f));
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
                date.Add(new(listPos, Color.White, new(0, 30 * multValue * Clamp((1-ratios), 0.32f, 1f)), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }
            }
}
