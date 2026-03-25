using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class ColdSakuraProj : ThrownSpearProjClass, IPixelatedRenderer
    {
        public override string Texture => ProjPath + "Proj_ColdSakura";
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.AlphaBlend;

        public enum Style
        {
            Attack,
            Decay
        }
        public ref float Timer => ref Projectile.ai[0];
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public List<Vector2> TrailPosList = [];
        public List<float> TrailRotList = [];
        public int TotalTrailCounts = 24;
        public Vector2 InitPos = Vector2.Zero;
        public override void ExSSD()
        {
            Projectile.ToTrailSetting(14, 2);
        }
        public override void ExSD()
        {
            //是的，这个矛的hitbox比你想的要大
            Projectile.width = Projectile.height = 60;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.Opacity = 0;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
                InitInFirstFrame();
            //手动存点用于绘制轨迹
            //而且这里的轨迹是用ex98的预制素材而不是纯顶点绘制
            //最主要是为了尽量与双子星区分
            TrailPosList.Add(Projectile.Center);
            TrailRotList.Add(Projectile.velocity.ToRotation());
            if (TrailPosList.Count > TotalTrailCounts)
                TrailPosList.RemoveAt(0);
            if (TrailRotList.Count > TotalTrailCounts)
                TrailRotList.RemoveAt(0);
            Lighting.AddLight(Projectile.Center, new Vector3(Color.HotPink.R / 255f, Color.HotPink.G / 255f, Color.HotPink.B / 255f));
            switch (AttackType)
            {
                case Style.Attack:
                    DoAttack();
                    break;
                case Style.Decay:
                    DoDecay();
                    break;
            }
            if (Main.rand.NextBool(6))
            {
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(30f), 2f, 6f);
                new PetalNoCollision(Projectile.Center.ToRandCirclePosEdge(4f), vel, RandLerpColor(Color.LightPink, Color.Violet).ToAddColor(), 30, RandRotTwoPi, 1f, .1f, 0.8f, true).Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float searchDistance = 600f;
            List<NPC> legalTargetList = [];
            foreach (var tar in Main.ActiveNPCs)
            {
                bool legalTar = tar != target && tar.CanBeChasedBy();
                float distPerTar = Vector2.Distance(tar.Center, Projectile.Center);
                //别穿墙搜索
                if (legalTar && distPerTar < searchDistance)
                {
                    searchDistance = distPerTar;
                    legalTargetList.Add(tar);
                }
            }
            NPC targetThatHit;
            if (legalTargetList.Count <= 0)
            {
                targetThatHit = target;
            }
            else
            {
                //将链表进行逆向操作，方便索引遍历
                legalTargetList.Reverse();
                //随机选择距离最近的其中两个单位，如果有可能的话。
                int maxIndex = Math.Min(legalTargetList.Count, 2);
                targetThatHit = legalTargetList[Main.rand.Next(0, maxIndex)];
            }
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileType<ColdSakuraArrow>(), Projectile.damage / 2, Projectile.knockBack, Owner.whoAmI);
            proj.HJScarlet().GlobalTargetIndex = targetThatHit.whoAmI;


        }

        private void InitInFirstFrame()
        {
            InitPos = Projectile.Center;
            for (int i = 0; i < TotalTrailCounts; i++)
            {
                TrailPosList.Add(Vector2.Zero);
                TrailRotList.Add(0);
            }
            Vector2 spawnPos = Projectile.Center + Projectile.SafeDir() * 12f;
            Color color = RandLerpColor(Color.Pink, Color.HotPink);
            //我感觉我有点预制素材上瘾。
            for (int i = 0; i < 6; i++)
            {
                new StarShape(spawnPos, Projectile.velocity.ToRandVelocity(ToRadians(15f), 3f), RandLerpColor(Color.Pink, Color.HotPink), 0.5f, 40).Spawn();
            }
            new CrossGlow(spawnPos + Projectile.SafeDir() * 5f, color, 20, 1, 0.05f).Spawn();
            new BloomShockwave(spawnPos + Projectile.SafeDir() * 5f, color, 20, 1, 0.015f).Spawn();
            for (int i = 0; i < 18; i++)
            {
                Vector2 dVel = Math.Sign(i - 18 / 2) * Main.rand.NextFloat(3f) * Projectile.SafeDir();
                Vector2 pos = spawnPos.ToRandCirclePos(8);
                float dscale = Main.rand.NextFloat(0.8f, 1.2f);
                new ShinyCrossStar(pos, dVel, RandLerpColor(Color.HotPink, color), 40, RandRotTwoPi, 1f, dscale * 0.2f, false, 0.2f).Spawn();
            }
        }
        //花

        private void DoAttack()
        {
            if (Main.rand.NextBool())
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(8), Projectile.velocity / 4f, RandLerpColor(Color.HotPink, Color.Violet), 40, RandRotTwoPi, 1f, 0.60f, false, 0.2f).Spawn();
            Projectile.Opacity = Lerp(Projectile.Opacity, 1f, 0.15f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Timer > 45f * Projectile.MaxUpdates)
            {
                AttackType = Style.Decay;
                Projectile.netUpdate = true;
                Timer *= 0;
            }
        }

        private void DoDecay()
        {
            Projectile.velocity *= 0.96f;
            if (Projectile.velocity.LengthSquared() < 10f * 10f)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float rotFixer = PiOver4;
            //我有点预制素材上瘾……
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            drawPos -= Projectile.SafeDir() * 60f;
            DrawStarShapeTrail(SB);
            if (Projectile.Opacity > 0.5f)
            {
                for (int i = 0; i < 8; i++)
                    SB.Draw(projTex, drawPos + ToRadians(i * 60).ToRotationVector2() * 2f, null, Color.White.ToAddColor() * Projectile.Opacity, Projectile.rotation + rotFixer, ori, Projectile.scale, 0, 0);
            }
            SB.Draw(projTex, drawPos, null, Color.White * Projectile.Opacity, Projectile.rotation + rotFixer, ori, Projectile.scale, 0, 0);
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.AlphaBlend);
            //DrawStarShapeTrail(sb);
            HJScarletMethods.EndShaderAreaPixel();
        }

        public void DrawStarShapeTrail(SpriteBatch sb)
        {
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            Vector2 scale = new Vector2(.45f, 1.4f);
            List<Vector2> PosList = TrailPosList;
            List<float> RotList = TrailRotList;
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                Color drawColor = (Color.Lerp(Color.HotPink, Color.Pink, rads) with { A = 80 }) * 0.9f * Projectile.Opacity * (1 - rads);
                Vector2 shapeScale = scale * Clamp(i / ((float)PosList.Count - 4f), 0f, 1f);
                Vector2 lerpPos = PosList[i] - Main.screenPosition;
                if (shapeScale.X > 0.3f && shapeScale.Y > 0.5f)
                {
                    sb.Draw(starShape, lerpPos, null, drawColor, RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale, 0, 0);
                    sb.Draw(starShape, lerpPos - Projectile.SafeDir() * 10f, null, drawColor, RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale, 0, 0);
                    sb.Draw(starShape, lerpPos, null, Color.White.ToAddColor(50), RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale * 0.25f, 0, 0);
                    sb.Draw(starShape, lerpPos - Projectile.SafeDir() * 10f, null, Color.White.ToAddColor(50), RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale * 0.25f, 0, 0);
                }
            }
        }
    }
}
