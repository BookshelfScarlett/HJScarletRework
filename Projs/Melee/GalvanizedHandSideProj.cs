using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class GalvanizedHandSideProj : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => GetInstance<GalvanizedHandThrownProj>().Texture;
        public bool IsAlreadyHitToTarget
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20);
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            DrawParticles();
            if (!IsAlreadyHitToTarget)
            {
                if (Projectile.GetTargetSafe(out NPC target, false, canPassWall: true))
                {
                    Projectile.HomingTarget(target.Center, -1, 24f, 0f, 0.3f);
                }
            }
            else
            {
                Projectile.damage *= 0;
            }
            if (Projectile.TooAwayFromOwner())
                Projectile.Kill();
        }
        public void DrawParticles()
        {
            for (int i = -1; i < 2; i += 1)
            {
                Vector2 spawnDustPos = Projectile.Center + Projectile.SafeDirByRot().RotatedBy(PiOver2) * 15f * i;
                Vector2 vel = Projectile.velocity / 3;
                if (Main.rand.NextBool())
                {
                    Vector2 shapePos = spawnDustPos.ToRandCirclePosEdge(3.2f);
                    float shapeScale = Main.rand.NextFloat(0.60f, 0.75f);
                    new StarShape(shapePos, vel * 1.2f, RandLerpColor(Color.CornflowerBlue, Color.RoyalBlue), shapeScale, 30).Spawn();
                }
                if (Main.rand.NextBool(3))
                {
                    float speed = Main.rand.NextFloat(0.8f, 1.1f);
                    float randDir = RandRotTwoPi;
                    int seedVa = Main.rand.Next(0, 10000);
                    float scale = 0.6f;
                    Vector2 pos = spawnDustPos.ToRandCirclePos(1.2f);
                    new TurbulenceShinyOrb(pos, speed, RandLerpColor(Color.CornflowerBlue, Color.RoyalBlue), 80, scale, randDir, seedValue: seedVa).Spawn();
                    new TurbulenceShinyOrb(pos, speed, Color.White, 80, scale * 0.5f, randDir, seedValue: seedVa).Spawn();
                }
            }

        }
        private void HitParticle(Vector2 center)
        {
            Vector2 dir = Projectile.SafeDirByRot();
            SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Hit with { MaxInstances = 1 }, Projectile.Center);
            for (int i = -1; i < 2; i += 1)
            {
                //生成主方向上的粒子
                for (int j = 0; j < 35; j++)
                {
                    Vector2 spawnDustPos = Projectile.Center + dir.RotatedBy(PiOver2) * 15f * i + dir * 20f;
                    Vector2 vel = dir * Main.rand.NextFloat(4f, 14f);
                    new StarShape(spawnDustPos.ToRandCirclePosEdge(3.2f), vel, RandLerpColor(Color.RoyalBlue, Color.CornflowerBlue), Main.rand.NextFloat(0.65f, 0.75f), 40).Spawn();
                }
            }
        }
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            IsAlreadyHitToTarget = true;
            HitParticle(target.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float fix = PiOver4 + PiOver2;
            SB.EnterShaderArea(BlendState.NonPremultiplied);
            DrawTrail(Color.RoyalBlue, 15f);
            DrawTrail(Color.DeepSkyBlue, 12f);
            DrawTrail(Color.White, 8f);
            SB.EnterShaderArea();
            DrawBack(Color.DeepSkyBlue, 15f, 0.85f);
            SB.EndShaderArea();
            Projectile.DrawProj(Color.LightBlue.ToAddColor(), 2, .4f, rotFix: fix);
            return false;
        }
        public void DrawBack(Color trailColor, float primitiveHeight, float alphaValue = 1f)
        {
            Asset<Texture2D> tex = HJScarletTexture.ColorMap_Aqua.Texture;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(tex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(10, tex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(trailColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(1.3f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            GD.Textures[0] = tex.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            List<ScarletVertex> list = [];
            int totalpoints = validPosition.Count;
            //创建顶点列表
            for (int i = 0; i < totalpoints - 1; i++)
            {
                if (validPosition[i + 1] - validPosition[i] == Vector2.Zero)
                    continue;
                float progress = (float)i / (totalpoints - 1);
                float rot = (validPosition[i + 1] - validPosition[i]).ToRotation();
                Vector2 posOffset = new Vector2(0, primitiveHeight * 0.9f).RotatedBy(rot);
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2 - Main.screenPosition - rot.ToRotationVector2() * 40f;
                QuickGetClass(ref list, oldCenter, posOffset, progress);
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            }

        }
        public void DrawTrail(Color trailColor, float primitiveHeight, float alphaValue = 1f)
        {
            Asset<Texture2D> tex = HJScarletTexture.Trail_ManaStreak.Texture;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(tex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(20, tex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -3.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(trailColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(1f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);

            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            GD.Textures[0] = tex.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            List<ScarletVertex> list = [];
            List<ScarletVertex> list2 = [];
            List<ScarletVertex> list3 = [];
            int totalpoints = validPosition.Count;
            for (int i = 0; i < totalpoints - 1; i++)
            {
                if (validPosition[i + 1] - validPosition[i] == Vector2.Zero)
                    continue;
                float progress = (float)i / (totalpoints - 1);
                float rot = (validPosition[i + 1] - validPosition[i]).ToRotation();
                Vector2 rotDir = rot.ToRotationVector2();
                Vector2 primiWidth = new Vector2(0, primitiveHeight * 0.9f).RotatedBy(rot);

                //这里的顶点实际绘制位置需要手动做一下偏移对上矛尖
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2 - Main.screenPosition - rotDir * 80f;
                Vector2 offset2 = rotDir.RotatedBy(PiOver2) * 15f;
                QuickGetClass(ref list, oldCenter, primiWidth, progress);
                QuickGetClass(ref list2, oldCenter + offset2, primiWidth, progress);
                QuickGetClass(ref list3, oldCenter - offset2, primiWidth, progress);
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list2.ToArray(), 0, list.Count - 2);
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list3.ToArray(), 0, list.Count - 2);
            }
        }
        public void QuickGetClass(ref List<ScarletVertex> list, Vector2 oldCenter, Vector2 posOffset, float progress)
        {
            ScarletVertex upClass = new(oldCenter - posOffset, Color.White, new Vector3(progress, 0, 0f));
            ScarletVertex downClass = new(oldCenter + posOffset, Color.White, new Vector3(progress, 1, 0f));
            list.Add(upClass);
            list.Add(downClass);
        }
    }
}
