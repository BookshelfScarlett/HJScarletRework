using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class TerraSpearPortal : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public enum State
        {
            InitSpawnState,
            HitSpawnState
        }
        public bool IsShooted = false;
        public ref float Timer => ref Projectile.ai[0];
        public State PortalType = State.InitSpawnState;
        public override ClassCategory Category => ClassCategory.Melee;
        public Vector2 MountedPos;
        public float MountedDistance = 160f;
        public bool LeftAngle = false;
        public List<Vector2> TrailPosList = [];
        public List<float> TrailRotList = [];
        public float TotalTrailCounts =14;

        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            switch (PortalType)
            {
                case State.InitSpawnState:
                    DoInitSpawnState();
                    break;
                case State.HitSpawnState:
                    DoHitSpawnState();
                    break;

            }
        }
        #region 主矛命中
        private void DoShooted()
        {
            Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.1f);
            Projectile.scale = Lerp(Projectile.scale, 0f, 0.1f);
            if (Projectile.Opacity < 0.22f && Projectile.scale < 0.22f)
                Projectile.Kill();
        }

        private void DoHitSpawnState()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (IsShooted)
            {
                DoShooted();
                return;
            }
            Projectile.velocity *= 0.86f;
            if (Projectile.GetLegalTarget(out NPC target))
            {
                Vector2 vecotr = (target.Center - Projectile.Center).ToSafeNormalize() * 20f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vecotr, ProjectileType<TerraSpearLaser>(), Projectile.originalDamage / 4, 1f, Owner.whoAmI);
            }
            IsShooted = true;
        }
        #endregion

        #region 常规挂载态
        private void DoInitSpawnState()
        {
            if (!Projectile.HJScarlet().FirstFrame)
            {
                InitPosList();
            }
            UpdatePosList();
            Projectile.velocity *= 0.86f;
            UpdateAttackAI();
            Vector2 setPos = Projectile.Center;
            if(Main.rand.NextBool())
            new ShinyCrossStar(setPos.ToRandCirclePosEdge(18f * Projectile.scale), RandVelTwoPi(1f), RandLerpColor(Color.LimeGreen, Color.DarkGreen), 100, RandRotTwoPi, 1f, 0.3f, 0.2f).Spawn();
            if (Main.rand.NextBool(4))
                new SmokeParticle(setPos.ToRandCirclePos(16f * Projectile.scale), RandVelTwoPi(1f), RandLerpColor(Color.Lime, Color.LimeGreen), 100, RandRotTwoPi, 1, 0.13f).Spawn();
        }

        private void UpdateAttackAI()
        {
            if (IsShooted)
            {
                DoShooted();
                return;
            }
            Timer++;
            if (Timer < 30f)
                return;
            if (Projectile.GetTargetSafe(out NPC target,canPassWall:true))
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center).ToSafeNormalize(Vector2.UnitX) * 5f, ProjectileType<TerraSpearArrow>(), Projectile.originalDamage, 2f, Owner.whoAmI);
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                DrawArrowSpawnDust();
            }
            IsShooted = true;
            Timer = 0;

        }
        private void DrawArrowSpawnDust()
        {
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            new CrossGlow(Projectile.Center, Color.Green, 40, 1f, 0.2f).Spawn();
            new CrossGlow(Projectile.Center, Color.White, 40, 1f, 0.1f).Spawn();
            int count = 36;
            for (int i = 1; i <= count; i++)
            {
                //生成一组圆环圈
                Vector2 dir = -Vector2.UnitY.RotatedBy(ToRadians(360 / count * i));
                new TurbulenceGlowOrb(Projectile.Center + dir * 1.2f, 1.2f, RandLerpColor(Color.DarkGreen, Color.PaleGreen), 40, 0.08f, dir.ToRotation()).SpawnToPriority();
            }
        }

        private void UpdatePortalPos()
        {
            Vector2 mountedPos = Owner.Center - Owner.ToMouseVector2().RotatedBy(ToRadians(30 * LeftAngle.ToDirectionInt())) * MountedDistance;
            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, 0.05f);
            Vector2 vector = (Main.MouseWorld - Projectile.Center).ToSafeNormalize() * 12f;
            Projectile.rotation = vector.ToRotation();
        }

        public void UpdatePosList()
        {
            TrailPosList.Add(Projectile.Center);
            TrailRotList.Add(Projectile.rotation);
            if (TrailPosList.Count > TotalTrailCounts)
                TrailPosList.RemoveAt(0);
            if (TrailRotList.Count > TotalTrailCounts)
                TrailRotList.RemoveAt(0);
        }

        public void InitPosList()
        {
            for (int i = 0; i < TotalTrailCounts; i++)
            {
                TrailPosList.Add(Vector2.Zero);
                TrailRotList.Add(0);
            }
        }

        #endregion
        public override bool PreDraw(ref Color lightColor)
        {
            DrawPortals();
            return false;
        }
        public void DrawTrailOnNeed()
        {
            List<Vector2> PosList = TrailPosList;
            //最顶端绘制一个star。
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Texture2D tex = Projectile.GetTexture();
            Vector2 scale = new Vector2(0.6f, 1.2f) * 12 * Projectile.scale;
            Vector2 ori = tex.Size() / 2;
            for (int i = 0; i < PosList.Count - 1; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                Color drawColor = (Color.Lerp(Color.Green, Color.Lime, rads) with { A = 50 }) * 0.9f * Projectile.Opacity * (1 - rads);
                Vector2 shapeScale = scale * Clamp(i / ((float)PosList.Count - 4f), 0f, 1f);
                Vector2 lerpPos = PosList[i] - Main.screenPosition;
                float lerpRot = TrailRotList[i] - PiOver2; 
                if (shapeScale.X > 0.1f && shapeScale.Y > 0.2f)
                {
                    SB.Draw(starShape, lerpPos, null, drawColor, lerpRot + PiOver2, starShape.ToOrigin(), shapeScale, 0, 0);
                    SB.Draw(starShape, lerpPos - Projectile.SafeDir() * 10f, null, drawColor, lerpRot + PiOver2, starShape.ToOrigin(), shapeScale, 0, 0);
                    SB.Draw(starShape, lerpPos, null, Color.White.ToAddColor(50), lerpRot + PiOver2, starShape.ToOrigin(), shapeScale * 0.35f, 0, 0);
                    SB.Draw(starShape, lerpPos - Projectile.SafeDir() * 10f, null, Color.White.ToAddColor(50), lerpRot + PiOver2, starShape.ToOrigin(), shapeScale * 0.35f, 0, 0);
                }
            }
        }
        public void DrawPortals()
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D bloomEdge = HJScarletTexture.Texture_BloomShockwave.Value;
            Texture2D crossGlow = HJScarletTexture.Particle_HRStar.Value;
            Texture2D backgroundCenter = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            Texture2D circle = HJScarletTexture.Texture_SoftCircleEdge.Value;
            float scaleLerp = Projectile.Opacity * Projectile.scale;
            //底图处理
            List<Vector2> PosList = TrailPosList;
            SB.Draw(backgroundCenter, drawPos, null, Color.Green, 0, backgroundCenter.ToOrigin(), 0.02f * scaleLerp, 0, 0);
            SB.EnterShaderArea();
            //光圈，叠加
            SB.Draw(bloomEdge, drawPos, null, Color.ForestGreen, 0, bloomEdge.ToOrigin(), 0.05f * scaleLerp, 0, 0);
            SB.Draw(bloomEdge, drawPos, null, Color.ForestGreen, 0, bloomEdge.ToOrigin(), 0.05f * scaleLerp, 0, 0);
            //绘制辉光
            SB.Draw(circle, drawPos, null, Color.LightGreen, 0, circle.Size() / 2, 0.12f * scaleLerp, 0, 0);
            SB.Draw(crossGlow, drawPos, null, Color.Lerp(Color.Green, Color.Silver, 0.5f), 0, crossGlow.Size() / 2, 0.18f * scaleLerp, 0, 0);
            SB.EndShaderArea();
        }
    }
    }
