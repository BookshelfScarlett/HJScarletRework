using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class LightBiteDarkStar : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => $"Terraria/Images/Item_{ItemID.FallenStar}";
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20, 2);
            Main.projFrames[Type] = 8;
        }
        public ref float Timer => ref Projectile.ai[0];
        public ref float RandRot => ref Projectile.localAI[0];
        public ref float RandScale => ref Projectile.ai[2];
        public Vector2 MountedCenter = Vector2.Zero;
        public override void ExSD()
        {
            Projectile.penetrate = 1;
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 80;
            Projectile.scale *= 0.7f;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            if(!Projectile.HJScarlet().FirstFrame)
            {
                MountedCenter = Projectile.Center;
            }
            //AI每一帧都更新一下避免过小或者过大
            //主要是为了防止外部修改
            RandScale = Clamp(RandScale, 0.7f, 1.32f);
            Projectile.velocity *= 0.95f;
            Projectile.scale += 0.05f;
            if (Projectile.scale > RandScale)
                Projectile.scale = RandScale;
            Projectile.rotation = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y);
            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(12f * RandScale), DustID.YellowStarDust);
                d.velocity = Projectile.velocity / 4f;
                d.scale *= Main.rand.NextFloat(0.6f, 1f) * RandScale;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override bool? CanDamage()
        {
            return Projectile.timeLeft < 40;
        }
        public override bool PreKill(int timeLeft)
        {

            //将五角星置入一个圆进行处理的话，这个就是从圆心到外角的半径距离。
            float starRadius = 15f * RandScale;
            int starPoints = 10;
            List<Vector2> starVertices = new List<Vector2>();
            for (int i = 0; i < starPoints; i++)
            {
                //五角星在视觉上有10个顶点，每个顶点各自实际上是36°
                float angle = Projectile.rotation + RandRot + ToRadians(i * 36) - PiOver2; // 向上为初始方向
                                                                                 //这里主要为了取内凹点。对于内凹的点，其距离会被设定为外角半径的0.4
                float radius = i % 2 == 0 ? starRadius : starRadius * 0.4f;
                //将对应的角度与距离转化为顶点坐标。存入这个list内
                Vector2 vertex = angle.ToRotationVector2() * radius;
                starVertices.Add(vertex);
            }

            //开始生成需要的粒子。f
            for (int edgeIndex = 0; edgeIndex < starPoints; edgeIndex++)
            {
                //当前顶点和下一个顶点。
                Vector2 startVertex = starVertices[edgeIndex];
                Vector2 endVertex = starVertices[(edgeIndex + 1) % starPoints];

                //沿两点连线均匀生成粒子
                int particlesPerEdge = 10;
                for (int i = 0; i < particlesPerEdge; i++)
                {
                    float t = (float)i / (particlesPerEdge - 1);
                    Vector2 currentPos = Vector2.Lerp(startVertex, endVertex, t);
                    float lerpValue = Lerp(MathF.Abs(edgeIndex % 10 - 5) / 5f, 1, 0.6f);

                    Vector2 dir2 = currentPos.SafeNormalize(Vector2.UnitX) *lerpValue;
                    ShinyOrbParticle orbs = new(Projectile.Center + currentPos, dir2 * 1f, RandLerpColor(Color.DarkOrange,Color.DarkGoldenrod), 30, 0.5f);
                    orbs.SpawnToPriority();
                }
            }

            return base.PreKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawProjItself();
            SB.EnterShaderArea();
            SB.EndShaderArea();
            return false;
        }
        public void DrawProjItself()
        {
            Texture2D projTex = Projectile.GetTexture();
            //取用的那张图无所谓，因为画出来的时候是纯色
            //同时不要画残影，没必要画。
            Rectangle frames = projTex.Frame(1, 8, 0, 1);
            Vector2 origin = frames.Size() / 2;
            for (int i = 0; i < 8; i++)
            {
                SB.Draw(projTex, Projectile.Center - Main.screenPosition + ToRadians(60f * i).ToRotationVector2() *  2.3f, frames, Color.Gold.ToAddColor(50), Projectile.rotation + RandRot, origin, Projectile.scale, 0, 0);
            }
            SB.Draw(projTex, Projectile.Center- Main.screenPosition, frames, Color.Black, Projectile.rotation + RandRot, origin, Projectile.scale, 0, 0);
        }
    }
}
