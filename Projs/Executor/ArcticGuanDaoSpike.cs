using ContinentOfJourney.Buffs;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ParticleScarlet;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Graphics.ParticleScarlet;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class ArcticGuanDaoSpike :HJScarletProj,  IPixelatedRenderer
    {
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.NorthPoleSnowflake);
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public int RandFrame = 0;
        public override Vector2 TileHitbox => new(3);
        public ref float Timer => ref Projectile.ai[0];
        public bool ActiveHoming = false;
        public NPC CurTarget = null;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.SetupImmnuity(10, ImmnuityType.Static);
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
            Projectile.Opacity = 0;
            Projectile.extraUpdates = 0;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
        }
        public override void OnFirstFrame()
        {
            RandFrame = Main.rand.Next(1, 3);
        }
        public override void ProjAI()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Opacity = Lerp(0, 1, Timer / Projectile.MaxUpdates * 10);
            Projectile.AffactedByGrav(yAdd: 1.1f);
            Projectile.tileCollide = true;
            ActiveHoming = false;
            if (Projectile.IsOutScreen())
                return;
            if (HJScarletConfigClient.Instance.PerformanceMode)
            {
                if (Main.rand.NextBool(38))
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.75f, Projectile.scale * Main.rand.NextFloat(.7f, 1.2f) * .3f, Main.rand.NextBool(), BlendState.Additive);
                if (Main.rand.NextBool(35))
                    ECSParticle.SnowCloud(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.56f, Projectile.scale * Main.rand.NextFloat(.7f, 1.2f) * .07f);
                if (Main.rand.NextBool(34))
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 1, 0.68f * Main.rand.NextFloat(.8f, 1.2f), .2f);

            }
            else
            {

                if (Main.rand.NextBool(8))
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.75f, Projectile.scale * Main.rand.NextFloat(.7f, 1.2f) * .3f, Main.rand.NextBool(), BlendState.Additive);
                if (Main.rand.NextBool(5))
                    ECSParticle.SnowCloud(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f, RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.56f, Projectile.scale * Main.rand.NextFloat(.7f, 1.2f) * .07f);
                if (Main.rand.NextBool(4))
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 1, 0.68f * Main.rand.NextFloat(.8f, 1.2f), .2f);
            }
        }
        public override void OnKill(int timeLeft)
        {

            if (HJScarletConfigClient.Instance.PerformanceMode)
            {
                ScarletSound(HJScarletSounds.Misc_Ding, Projectile.Center, 0.25f, 1, -0.3f, 0.05f);
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(4), 1.2f, RandLerpColor(Color.WhiteSmoke, Color.SkyBlue), Main.rand.Next(45, 60), 1, Main.rand.NextFloat(.7f, 1.2f) * .1f);
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(10), (-Vector2.UnitY) * Main.rand.NextFloat(1.2f, 6.9f), RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 1, 0.68f * Main.rand.NextFloat(.8f, 1.2f), .2f);
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(10), RandVelTwoPi(0.1f, 1.1f), RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.75f, Projectile.scale * Main.rand.NextFloat(.7f, 1.2f) * .13f, true, BlendState.AlphaBlend);
            }

            else
            {
                ScarletSound(HJScarletSounds.Misc_Ding, Projectile.Center, 0.25f, 1, -0.3f, 0.05f);
                for (int i = 0; i < 8; i++)
                    ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(4), 1.2f, RandLerpColor(Color.WhiteSmoke, Color.SkyBlue), Main.rand.Next(45, 60), 1, Main.rand.NextFloat(.7f, 1.2f) * .1f);
                for (int i = 0; i < 4; i++)
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(10), (-Vector2.UnitY) * Main.rand.NextFloat(1.2f, 6.9f), RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 1, 0.68f * Main.rand.NextFloat(.8f, 1.2f), .2f);
                for (int i = 0; i < 4; i++)
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(10), RandVelTwoPi(0.1f, 1.1f), RandLerpColor(Color.WhiteSmoke, Color.LightSkyBlue), 40, RandRotTwoPi, 0.75f, Projectile.scale * Main.rand.NextFloat(.7f, 1.2f) * .13f, true, BlendState.AlphaBlend);
            }
        }
        public override bool? CanDamage()
        {
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            Texture2D tex = HJScarletTexture.Particle_SharpTear;
            Rectangle frame = tex.Frame();
            Vector2 ori = tex.Size() / 2;
            SB.EnterShaderArea();
            //绘制残影
            float oriScale = 0.8f;
            float scale = 0.9f;
            int length = 15;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.975f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.RoyalBlue, Color.LightSkyBlue, (1 - rads)).ToAddColor(255) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.20f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f) + PiOver2;
                SB.Draw(tex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }

            Vector2 pos = Projectile.Center - Main.screenPosition;
            SB.Draw(tex, pos, null, Color.SkyBlue, Projectile.rotation + PiOver2, ori, oriScale, 0, 0);
            SB.EndShaderArea();
            return false;
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            if (HJScarletConfigClient.Instance.PerformanceMode)
                return;
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.RoyalBlue, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.DeepSkyBlue, .95f);
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.White, 0.85f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 10;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50.2f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.08f);
            shader.CurrentTechnique.Passes[0].Apply();
            GD.Textures[1] = HJScarletTexture.Texture_Spirite.Value;
            GD.SamplerStates[1] = SamplerState.PointWrap;
            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = (int)(Projectile.oldPos.Length * Projectile.Opacity * 0.70f);
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 drawPos = Projectile.oldPos[j] + new Vector2(Projectile.width / 2, Projectile.height / 2);
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(5, 25 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }

}
