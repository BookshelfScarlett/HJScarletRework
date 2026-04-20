using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static HJScarletRework.Projs.Executor.AetherfireSmasherName;

namespace HJScarletRework.Projs.Executor
{
    public class AetherfireSmasherFireball : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public ref float Timer => ref Projectile.ai[0];
        public ref float UseNameColor => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 8;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Timer > 10)
            {
                if (Projectile.GetTargetSafe(out NPC target,searchDistance:1200))
                    Projectile.HomingTarget(target.Center, -1f, 18f + (Timer - 30f) / 5f, 15f);
                else
                    Projectile.velocity.Y += 0.18f;
            }
            else
            {
                Projectile.velocity.Y += 0.22f;
                Projectile.timeLeft = 120;
            }
            SetUpDustType();
        }

        public void SetUpDustType()
        {
            if (Projectile.IsOutScreen())
                return;
            PickTagColor(out Color baseColor, out Color targetColor);
            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool(6))
                    new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePosEdge(4f), 0.5f, RandLerpColor(baseColor, targetColor), 20, Main.rand.NextFloat(0.1f, 0.12f), RandRotTwoPi).Spawn();
                if (Main.rand.NextBool(4))
                    new SmokeParticle(Projectile.Center.ToRandCirclePos(8f), -Projectile.velocity / 8f, RandLerpColor(baseColor, Color.DarkGray), 20, RandRotTwoPi, 1f, Main.rand.NextFloat(0.12f, 0.16f) * 1.1f).SpawnToPriorityNonPreMult();
            }
            Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(10f), 0.8f, 1.4f);
            if (Main.rand.NextBool(3))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(4f), vel, RandLerpColor(baseColor, targetColor), 40, RandRotTwoPi, 1f, 0.3f, ToRadians(10f)).Spawn();
        }
        private void PickTagColor(out Color baseColor, out Color targetColor)
        {
            switch (Owner.name.SelectedName())
            {
                case NameType.TrueScarlet:
                    baseColor = Color.LightCoral;
                    targetColor = Color.Crimson;
                    break;
                //查 -- 金
                case NameType.WutivOrChaLost:
                    baseColor = Color.LightGoldenrodYellow;
                    targetColor = Color.Yellow;
                    break;
                //银九 - 粉
                case NameType.Emma:
                    baseColor = Color.HotPink;
                    targetColor = Color.Pink;
                    break;
                case NameType.SherryOrAnnOrKino:
                    baseColor = Color.RoyalBlue;
                    targetColor = Color.LightBlue;
                    break;
                case NameType.Shizuku:
                    baseColor = Color.LightSkyBlue;
                    targetColor = Color.AliceBlue;
                    break;

                //聚胶 - 紫
                case NameType.SerratAntler:
                    targetColor = Color.White;
                    baseColor = Color.DarkViolet;
                    break;
                case NameType.Hanna:
                    baseColor = Color.LimeGreen;
                    targetColor = Color.White;
                    break;
                default:
                    targetColor = Color.Red;
                    baseColor = Color.OrangeRed;
                    break;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTile(oldVelocity);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            PickTagColor(out Color baseColor, out Color targetColor);
            //绘制残影
            float oriScale = 0.64f;
            float scale = 1f;
            int length = 4;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.975f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(baseColor, targetColor    , (1 - rads)).ToAddColor(10) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.50f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_FadedStreak.Texture, baseColor);
            DrawTrails(HJScarletTexture.Trail_FadedStreak.Texture, targetColor, 0.8f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, targetColor, 0.45f);
            SB.EndShaderArea();

            SB.Draw(projTex, projPos, null, baseColor.ToAddColor(50), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.65f, 0, 0);

            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (Projectile.oldPos.Length < 3)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -1.2f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();
            GD.Textures[1] = HJScarletTexture.Texture_Spirite.Value;
            GD.SamplerStates[1] = SamplerState.PointWrap;

            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> _, Projectile.oldPos, Projectile.oldRot);
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2, drawColor, new Vector2(0, 17 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);

        }
    }
}
