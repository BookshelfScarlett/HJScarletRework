using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class FierySpearProj : ThrownSpearProjClass
    {
        public override string Texture => HJScarletItemProj.Proj_FierySpear.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(10, 2);
        }
        public override void ExSD()
        {
            Projectile.noEnchantmentVisuals = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            if(HJScarletMethods.HasFuckingCalamity && !Projectile.HJScarlet().FirstFrame)
            {
                Projectile.extraUpdates = 5;
                Projectile.penetrate = 2;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            new Fire(Projectile.Center + Main.rand.NextVector2CircularEdge(6f, 6f), -Projectile.velocity / 4, Color.Orange.RandLerpTo(Color.OrangeRed), 40, Main.rand.NextFloat(TwoPi), 1, 0.12f).Spawn();
            //获得最前面的顶点位置
            Vector2 dir = Projectile.SafeDirByRot();
            Vector2 drawPos = Projectile.Center + dir * 60f;
            //而后，获取粒子需要的方向
            Vector2 dustDir = dir.RotatedBy(PiOver2);
            //而后我们开始绘制需要的粒子
            int i = 0;
            while (i < 2)
            {
                new TurbulenceShinyOrb(drawPos + Main.rand.NextVector2CircularEdge(12f, 12f), 0.8f, Color.OrangeRed.RandLerpTo(Color.Orange), 40, 0.12f, Main.rand.NextFloat(TwoPi)).Spawn();
                i++;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnVolcanoDustAndProj(target.whoAmI);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnVolcanoDustAndProj();
            return true;
        }
        public void SpawnVolcanoDustAndProj(int targetIndex = -1)
        {
            int spawnBallCounts = 2 + HJScarletMethods.HasFuckingCalamity.ToInt() * 2;
            for (int i = 0; i < spawnBallCounts; i++)
            {
                Vector2 dir = Projectile.SafeDirByRot().ToRandVelocity(PiOver4);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * -Main.rand.NextFloat(6f, 12f), ProjectileType<FierySpearFireball>(), Projectile.damage, Owner.whoAmI);
                proj.HJScarlet().GlobalTargetIndex = targetIndex;
                for (int j = 0; j < 30; j++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                    dust.velocity -= dir * 12f * Main.rand.NextFloat(0.1f, 0.5f);
                    dust.scale *= Main.rand.NextFloat(1.5f, 3f);
                    dust.noGravity = true;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            DrawSideStreak(star, drawPos);
            Projectile.DrawGlowEdge(Color.Gold, rotFix: PiOver4);
            Projectile.DrawProj(Color.White, offset: 0.3f, rotFix: PiOver4);
            
            return false;
        }
        public void DrawSideStreak(Texture2D star, Vector2 drawPos)
        {
            for (int i = 0; i < 8; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                for (int k = -1; k < 2; k += 2)
                {
                    float rads = (float)i / 10;
                    Color drawColor = (Color.Lerp(Color.OrangeRed, Color.Orange, rads) with { A = 0 }) * 0.7f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                    Vector2 pos = drawPos + Projectile.SafeDir() * 20f;
                    Vector2 offset = Projectile.SafeDir().RotatedBy(PiOver2);
                    Main.spriteBatch.Draw(star, pos + offset * 7f * k - Projectile.velocity * 0.7f * i, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(0.3f, 1.0f), 0, 0);
                }
            }
        }
        public void DrawNebulaTrail(Color trailColor, float height)
        {
            float laserLength = 15;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(HJScarletTexture.Trail_TerraRayFlow.Size);
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, HJScarletTexture.Trail_TerraRayFlow.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -10);
            shader.Parameters["uColor"].SetValue(trailColor.ToVector4());
            shader.Parameters["uFadeoutLength"].SetValue(0.5f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();

            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            GD.Textures[0] = HJScarletTexture.Trail_TerraRayFlow.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            //直接获取需要的贝塞尔曲线。
            List<VertexPositionColorTexture2D> list = [];
            int totalpoints = validPosition.Count;
            //创建顶点列表
            for (int i = 0; i < validPosition.Count; i++)
            {
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2 - Main.screenPosition;
                float progress = (float)i / (validPosition.Count - 1);
                Vector2 posOffset = new Vector2(0, 3f * height).RotatedBy(validRot[i]);
                VertexPositionColorTexture2D upClass = new(oldCenter - posOffset, trailColor, new Vector3(progress, 1, 0f));
                VertexPositionColorTexture2D downClass = new(oldCenter + posOffset, trailColor, new Vector3(progress, 0, 0f));
                list.Add(upClass);
                list.Add(downClass);
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            }
        }
    }
}
