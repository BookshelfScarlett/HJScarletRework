using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
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
            //事实上，敌对单位是会移动的，因此这里提供的速度是相对于较快的
            if (Projectile.GetTargetSafe(out NPC tar, true, 200) && Projectile.timeLeft < 40)
                Projectile.HomingTarget(tar.Center, -1, 4f, 20f);
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
            //SpawnFlowerParticles();
            //将五角星置入一个圆进行处理的话，这个就是从圆心到外角的半径距离。
            float starRadius = 15f * RandScale;
            int starPoints = 10;
            List<Vector2> starVertices = [];
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
            for (int i = 0; i < starPoints; i++)
            {
                //当前顶点和下一个顶点。
                Vector2 startVertex = starVertices[i];
                Vector2 endVertex = starVertices[(i+ 1) % starPoints];

                //沿两点连线均匀生成粒子
                int connectCounts = 10;
                for (int j = 0; j < connectCounts; j++)
                {
                    float t = (float)j / (connectCounts - 1);
                    Vector2 currentPos = Vector2.Lerp(startVertex, endVertex, t);
                    float lerpValue = Lerp(MathF.Abs(i% 10 - 5) / 5f, 1, 0.6f);

                    Vector2 dir2 = currentPos.SafeNormalize(Vector2.UnitX) *lerpValue;
                    ShinyOrbParticle orbs = new(Projectile.Center + currentPos, dir2 * 1f, RandLerpColor(Color.DarkOrange,Color.DarkGoldenrod), 30, 0.5f);
                    orbs.SpawnToPriority();
                }
            }
            return base.PreKill(timeLeft);
        }
        public void SpawnFlowerParticles()
        {
            //花蕊的半径。
            float heartRads = 3.6f;
            //花蕊的粒子数量。
            int heartDustCounts = 20;
            for (int i = 0; i < heartDustCounts; i++)
            {
                //花蕊粒子与中心的向量差
                Vector2 heartPos = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(heartRads);
                //需要表现出从中心往外扩散的效果
                Vector2 dir = heartPos.SafeNormalize(Vector2.UnitX) * 1.6f;
                Dust d = Dust.NewDustPerfect(Projectile.Center + heartPos, DustID.TheDestroyer);
                d.scale *= 1.1f;
                d.velocity = dir;
                d.noGravity = true;
            }

            //尝试生成花瓣
            int petalCount = 5;
            float outerRadius = 18f;
            float innerRadius = 3.6f;
            List<List<Vector2>> petalVertexList = [];
            float randAngle = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int k = 0; k < petalCount; k++)
            {
                //每次过来的时候都会新建一次顶点数据
                //然后放入到上面的顶点列表里的顶点列表内
                List<Vector2> petalVertexs = [];
                //五个花瓣，总共72°
                float petalStartAngle = randAngle + MathHelper.ToRadians(k * 72);
                float petalEndAngle = petalStartAngle + MathHelper.ToRadians(72);
                //生成花瓣内侧弧线顶点
                for (int i = 0; i <= 4; i++)
                {
                    float t = (float)i / 4;
                    float angle = MathHelper.Lerp(petalStartAngle, petalEndAngle, t);
                    Vector2 vertex = angle.ToRotationVector2() * innerRadius;
                    petalVertexs.Add(vertex);
                }

                //生成花瓣外侧弧线顶点，需逆向
                for (int i = 4; i >= 0; i--)
                {
                    float t = (float)i / 4;
                    float angle = MathHelper.Lerp(petalStartAngle, petalEndAngle, t);
                    //调整了一点弧度。
                    float dynamicOuterRadius = outerRadius * (1 + 0.1f * (float)Math.Sin(angle * 5));
                    Vector2 vertex = angle.ToRotationVector2() * dynamicOuterRadius;
                    petalVertexs.Add(vertex);
                }

                petalVertexList.Add(petalVertexs);
            }

            //连线。
            foreach (var petalVertex in petalVertexList)
            {
                for (int i = 0; i < petalVertex.Count - 1; i++)
                {
                    Vector2 startVertex = petalVertex[i];
                    Vector2 endVertex = petalVertex[i + 1];
                    int connectCounts = 8;
                    for (int j = 0; j < connectCounts; j++)
                    {
                        float t = (float)j / (connectCounts - 1);
                        Vector2 currentPos = Vector2.Lerp(startVertex, endVertex, t);
                        float lerpValue = MathHelper.Lerp(MathF.Abs(i % 10 - 5) / 5f, 1, 0.6f);
                        Vector2 dir2 = currentPos.SafeNormalize(Vector2.UnitX) * lerpValue;
                        Dust d = Dust.NewDustPerfect(Projectile.Center + currentPos, DustID.TheDestroyer);
                        d.scale *= 1.1f;
                        d.velocity = dir2;
                        d.noGravity = true;
                    }
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, GetSeconds(5));
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
