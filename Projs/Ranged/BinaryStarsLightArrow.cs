using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Ranged
{
    public class BinaryStarsLightArrow : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Ranged;
        private enum DoType
        {
            IsHomingToTarget,
            IsArcRotating
        };
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        public enum Flip
        {
            None,
            DoFlip
        }
        private ref float TargetIndex => ref Projectile.ai[0];
        private ref float Accele => ref Projectile.ai[1];
        private bool CanSpawnDust
        {
            get => Projectile.HJScarlet().ExtraAI[1] == 1f;
            set => Projectile.HJScarlet().ExtraAI[1] = value ? 1f : 0f;
        }
        private bool IsHit = false; 
        private ref float Progress => ref Projectile.localAI[2];
        private Vector2 InitCenter;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.height = 8;
            Projectile.width = 8;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 500;
            Projectile.friendly = true;
        }
        private float DrawScale = 1f;
        public override void AI()
        {
            //初始化
            if (Accele is 0)
            {
                InitCenter = Projectile.Center;
                Progress = 45f;
            }
            Accele += 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            switch (AttackType)
            {
                case DoType.IsArcRotating:
                    DoArcRotating();
                    break;
                case DoType.IsHomingToTarget:
                    DoHomingToTarget();
                    break;
            }
        }
        private void DrawDust(Vector2 targetCenter)
        {
            if (!CanSpawnDust)
                return;
            float lengthRatio = Clamp(Vector2.Distance(Projectile.Center, InitCenter) / Vector2.Distance(InitCenter, targetCenter) , 0f, 2f);
            int d = Main.rand.NextFloat(0f, 2f - lengthRatio) < 0.5f ?  DustID.CorruptSpray: DustID.WitherLightning;
            float dScale = Main.rand.NextFloat(0f, 2f - lengthRatio) < 0.5f ? 0.65f : 0.75f;
            Dust newDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), d, (Projectile.velocity * 0.30f).RotatedBy(Main.rand.NextFloat(-PiOver4 / 6, PiOver4 / 6)),0, default, 1);
            newDust.noGravity = true;
            newDust.scale *= dScale;
            newDust.color = Color.Purple;
        }

        private void DoArcRotating()
        {
            float progress = Progress / 45f;
            DrawScale = Clamp(EaseOutCubic(progress), 0f, 1f);
            Projectile.velocity *= 0.89f;
            Progress--;
            if (progress <- 15f)
                Projectile.Kill();
        }

        private void DoHomingToTarget()
        {
            //重新搜索一次单位
            if (!Projectile.GetTargetSafe(out NPC target, (int)TargetIndex, true, 3600))
            {
                Projectile.extraUpdates = 2;
                return;
            }
            if (Main.rand.NextBool(3))
                DrawDust(target.Center);
            float maxAccele = Clamp(Accele / 2f, 0f, 8f);
            Projectile.extraUpdates = 4;
            //这里的惯性有意降的非常低
            Projectile.HomingTarget(target.Center, 3600f, 24f + maxAccele, 10f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //首次命中时开始转角拐弯
            if (!IsHit)
            {
                IsHit = true;
                AttackType = DoType.IsArcRotating;
                Projectile.netUpdate = true;
                DrawDust(target.Center);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            DrawNebulaTrail(Color.MediumPurple, 14f); 
            DrawNebulaTrail(Color.LightPink with { A  = 50 }, 12.2f); 
            DrawNebulaTrail(Color.White with { A = 100 }, 10.8f); 
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (Projectile.oldPos.Length > 12)
            {
                for (int k = 0; k < 12; k += 3)
                {
                    Vector2 dir = Projectile.oldPos[k] - Projectile.oldPos[k + 1];
                    DrawStar(Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition, dir.ToRotation(), Color.Lerp(BinaryStarsMain.TrailColor, Color.Purple, (float)k / 16));
                }
            }
            SB.End();
            SB.BeginDefault();
            return false;
        }
        public void DrawStar(Vector2 drawPos, float rot,Color starColor)
        {
            Texture2D sharpTears = HJScarletTexture.Particle_HRStar.Value;
            Vector2 targetSize = 0.36f * Projectile.scale * new Vector2(1.2f, 0.25f) * DrawScale;
            SB.Draw(sharpTears, drawPos, null, starColor, rot, sharpTears.Size() / 2, targetSize, SpriteEffects.None, 0);
            SB.Draw(sharpTears, drawPos, null, Color.White with { A = 150 }, rot, sharpTears.Size() / 2, targetSize * 0.5f, SpriteEffects.None, 0);
        }
        public void DrawNebulaTrail(Color trailColor, float height)
        {
            float laserLength = 50;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(HJScarletTexture.Trail_ManaStreak.Size);
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, HJScarletTexture.Trail_ManaStreak.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            shader.Parameters["uColor"].SetValue(trailColor.ToVector4() * DrawScale);
            shader.Parameters["uFadeoutLength"].SetValue(0.1f);
            shader.Parameters["uFadeinLength"].SetValue(0.05f);
            shader.CurrentTechnique.Passes[0].Apply();

            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            GD.Textures[0] = HJScarletTexture.Trail_ManaStreak.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            //直接获取需要的贝塞尔曲线。
            List<VertexPositionColorTexture2D> list = [];
            int totalpoints = validPosition.Count;
            //创建顶点列表
            for (int i = 0; i < validPosition.Count; i++)
            {
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2  - Main.screenPosition;
                float progress = (float)i / (validPosition.Count - 1);
                Vector2 posOffset = new Vector2(0, height * DrawScale * ((float)(totalpoints - i) / totalpoints)).RotatedBy(validRot[i]);
                VertexPositionColorTexture2D upClass = new(oldCenter - posOffset, trailColor, new Vector3(progress, 0, 0f));
                VertexPositionColorTexture2D downClass = new(oldCenter + posOffset, trailColor, new Vector3(progress, 1, 0f));
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