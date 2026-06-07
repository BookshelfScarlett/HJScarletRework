using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.General
{
    public class SlimeGodShieldSunProj : HJScarletProj
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override string Texture => GetInstance<SlimeGodShield>().Texture;
        public ref float Timer => ref Projectile.ai[0];
        public ref float Timer2 => ref Projectile.ai[1];
        public override void ExSD()
        {
            Projectile.timeLeft = 600;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void ProjAI()
        {
            Projectile.timeLeft = 2;
            Timer++;
            float ratios = Clamp(Timer / (Projectile.MaxUpdates * 30f), 0f, 1f);
            Projectile.velocity = Vector2.Lerp(Projectile.SafeDir() * 5f, Projectile.SafeDir() * .0f, ratios);
            Projectile.rotation += Lerp(.4f,.01f,ratios);
            if (ratios == 1f)
            {
                Timer2++;
                float ratios2 = Clamp(Timer2 / (Projectile.MaxUpdates * 10f), 0f, 1f);

            }

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return true;
        }
        public override bool? CanDamage()
        {
            return Timer > (Projectile.MaxUpdates * 30f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            SB.Draw(HJScarletTexture.Texture_WhiteCube.Value, Owner.Center, null, Color.DarkOrange* .13f,0, HJScarletTexture.Texture_WhiteCube.Origin,new Vector2(30000f,30000f), 0, 0);
            DrawBeam(Color.DarkOrange,.95f);
            DrawBeam(Color.OrangeRed,.75f);
            DrawBeam(Color.White,.65f);
            SB.Draw(projTex, drawPos, null, Color.White, Projectile.rotation, ori, Projectile.scale, 0, 0);
            return false;
        }
        public void DrawBeam(Color color, float heightMult)
        {
                float ratios2 = Clamp(Timer2 / (Projectile.MaxUpdates * 10f), 0f, 1f);
            SB.EnterShaderArea();
            Asset<Texture2D> value = HJScarletTexture.Trail_ManaStreak.Texture;
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(value.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(2200, value.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -101);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * Projectile.Opacity * 0.9f);
            shader.Parameters["uFadeoutLength"].SetValue(0.02f);
            shader.Parameters["uFadeinLength"].SetValue(0.0f);
            shader.CurrentTechnique.Passes[0].Apply();
            // 设定内端宽度（可固定很小）和外端宽度（随时间增大）
            float innerWidth = 5f;
            float outerWidth = Lerp(value.Height() *0.1f, value.Height() * 0.5f, ratios2); // 从细到粗
            float length = 2200f; // 固定长度，不向外移动

            Vector4 vector4 = new(.1f, .1f, 0.05f, 0.6f);
            Color setColor = Color.Lerp(Color.OrangeRed, Color.WhiteSmoke, 0.28f);
            for (int i = 0; i < 20; i++)
            {
                float angle = Projectile.rotation + TwoPi * i / 20;
                HJScarletMethods.ApplyAlphaCut(vector4, angle.ToRotationVector2(), new Vector2(.51f, 1f), setColor);
                DrawRadialTrapezoid(Main.spriteBatch, value.Value, Projectile.Center, angle, innerWidth, outerWidth, length, color);
            }
            SB.EndShaderArea();
        }
        /// <summary>
        /// 绘制从中心向外放射的梯形（窄端在中心，宽端在外）
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch（需要处于 Begin 状态）</param>
        /// <param name="texture">纹理</param>
        /// <param name="center">中心点（世界坐标）</param>
        /// <param name="angle">光线朝向（弧度）</param>
        /// <param name="innerWidth">内端宽度（像素）</param>
        /// <param name="outerWidth">外端宽度（像素）</param>
        /// <param name="length">光线长度（像素）</param>
        /// <param name="color">颜色</param>
        private static void DrawRadialTrapezoid(SpriteBatch spriteBatch, Texture2D texture, Vector2 center, float angle, float innerWidth, float outerWidth, float length, Color color)
        {
            // 计算方向向量
            Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            Vector2 perp = new Vector2(-dir.Y, dir.X); // 垂直方向

            // 四个顶点（顺序：左上、右上、右下、左下），三角形条带绘制需要顺时针或逆时针
            Vector2 innerLeft = center + perp * (innerWidth * 1.95f);
            Vector2 innerRight = center - perp * (innerWidth * 1.95f);
            Vector2 outerLeft = center + dir * length + perp * (outerWidth * 1f);
            Vector2 outerRight = center + dir * length - perp * (outerWidth * 1f);

            // 转换到屏幕坐标
            innerLeft -= Main.screenPosition;
            innerRight -= Main.screenPosition;
            outerLeft -= Main.screenPosition;
            outerRight -= Main.screenPosition;

            // 顶点数组（TriangleStrip 顺序：0: innerLeft, 1: outerLeft, 2: innerRight, 3: outerRight）
            var vertices = new VertexPositionColorTexture[4];
            vertices[0] = new VertexPositionColorTexture(new Vector3(innerLeft, 0), color, new Vector2(0, 0));
            vertices[1] = new VertexPositionColorTexture(new Vector3(outerLeft, 0), color, new Vector2(1, 0));
            vertices[2] = new VertexPositionColorTexture(new Vector3(innerRight, 0), color, new Vector2(0, 1));
            vertices[3] = new VertexPositionColorTexture(new Vector3(outerRight, 0), color, new Vector2(1, 1));

            // 应用纹理和绘制
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
        }
    }
}
