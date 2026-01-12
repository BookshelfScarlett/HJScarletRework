using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Ranged
{
    public class BinartStarsPhantom: HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Ranged;
        public HJScarletGlobalProjs ModProj => Projectile.HJScarlet();
        public override string Texture => GetInstance<BinaryStarsMain>().Texture;
        public int TargetIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public ref float AttackTimer => ref Projectile.ai[1];
        public ref float TotalArcAngle => ref Projectile.ai[2];
        public bool IsFlip
        {
            get => ModProj.ExtraAI[0] is 1f;
            set => ModProj.ExtraAI[0] = value ? 1f : 0f;
        }
        public ref float SpriteRotation => ref ModProj.ExtraAI[1];
        public ref float ArcRotation => ref ModProj.ExtraAI[1];
        const int SetUpdate = 3;
        //是否画圆
        private bool _isArcRotating = false; 
        //旋转起始角
        private float _arcStartRotation;
        private bool ShouldDrawVertex = true;
        //总转角
        //持续帧数
        private const int ArcDuration = 15 * SetUpdate;
        //发起旋转前的原始速度
        private float _originalSpeed;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.extraUpdates = SetUpdate;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(IsFlip);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            IsFlip = reader.ReadBoolean();
        }
        public override bool? CanDamage() => AttackTimer > ArcDuration;
        public override void AI()
        {
            if(AttackTimer == 0f)
            {
                IsFlip = TotalArcAngle < 0;
                SpriteRotation = Projectile.velocity.ToRotation();
            }

            if (Projectile.timeLeft < 50)
                Projectile.Kill();

            //生成，渐变
            if (!ShouldDrawVertex)
                Projectile.rotation += 0.2f * IsFlip.ToDirectionInt();
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            Projectile.Opacity += 0.1f;
            Projectile.Opacity = Clamp(Projectile.Opacity, 0f, 1f);
            AttackTimer += 1f;
            //绘制圆弧运动
            if (AttackTimer < ArcDuration)
            {
                DrawArc();
                return;
            }

            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
            {
                Projectile.extraUpdates = 4;
                Projectile.HomingTarget(target.Center, 1800f, 20f, 20f);
            }
            else
            {
                Projectile.extraUpdates = 3;
                Projectile.HomingTarget(Owner.Center, 1800f, 20f, 10f);
                if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                {
                    Projectile.Kill();
                    Projectile.netUpdate = true;
                }
            }
        }

        private void DrawArc()
        {
            if (!_isArcRotating)
            {
                _isArcRotating = true;
                _arcStartRotation = Projectile.velocity.ToRotation();
                _originalSpeed = Projectile.velocity.Length();
                Projectile.velocity *= 0.40f;
            }

            if (_isArcRotating)
            {
                float arcProgress = (float)AttackTimer / ArcDuration;
                //计算当前的角度
                ArcRotation = _arcStartRotation + TotalArcAngle * arcProgress;
                //同步旋转角度与速度
                Projectile.velocity = ArcRotation.ToRotationVector2() * Projectile.velocity.Length();
                //?
                if (AttackTimer >= ArcDuration)
                {
                    //重置速度
                    Projectile.velocity = ArcRotation.ToRotationVector2() * _originalSpeed;
                    //跳出循环
                    _isArcRotating = false;
                }
                return;
            }
        }
        public override Color? GetAlpha(Color lightColor) => new(251, 184, 255, 100);
        #region  Draw
        public float SetProjWidth(float ratio)
        {
            float width = Projectile.width + 20;
            width *= SmoothStep(0.8f, 0.6f, Utils.GetLerpValue(0f, 0.5f, ratio, true));
            return width;
        }
        public Color SetTrailColor(float ratio)
        {
            float velocityOpacityFadeout = Utils.GetLerpValue(1f, 5f, Projectile.velocity.Length(), true);
            Color c = BinaryStarsMain.TrailColor * Projectile.Opacity * (1f - ratio);
            return c * Utils.GetLerpValue(0.04f, 0.1f, ratio, true) * velocityOpacityFadeout;
        }
        //DrawOffset
        public Vector2 PrimitiveOffsetFunction(float ratio)
        {
            return Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.scale * 0.5f * 0.5f;
        }

        //TODO：下面那个轨迹把归元漩涡的轨迹改成另外一种，现在这个纯纯占位符
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White, rotFix: -PiOver4);
            Projectile.DrawProj(Color.White, 4, 0.7f, -PiOver4);
            if (!HJScarletMethods.OutOffScreen(Projectile.Center) && ShouldDrawVertex)
            {
                SB.End();
                SB.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                float spinRotation = Main.GlobalTimeWrappedHourly * 5.2f;
                DrawTrails(HJScarletTexture.Trail_ManaStreak, Color.Violet);
                DrawTrails(HJScarletTexture.Trail_ManaStreak, Color.Violet, 0.4f, 0.8f, offsetHeight: 12f);
                DrawTrails(HJScarletTexture.Trail_ManaStreak, Color.Violet, 0.4f, 0.8f, offsetHeight: -12f);
                DrawTrails(HJScarletTexture.Trail_ParaLine, Color.White, 0.4f,alphaValue: 1f);

                SB.End();
                SB.BeginDefault();
            }
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            //做掉可能存在的零向量
            GD.Textures[0] = useTex.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            //直接获取需要的贝塞尔曲线。
            List<VertexPositionColorTexture2D> list = [];
            int totalpoints = validPosition.Count;
            //创建顶点列表
            for (int i = 0; i < validPosition.Count - 1; i++)
            {
                float progress = (float)i / (validPosition.Count - 1);
                float rotated = (validPosition[i + 1] - validPosition[i]).ToRotation();
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2 + rotated.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight - Main.screenPosition;
                Vector2 posOffset = new Vector2(0, SetProjWidth(progress) * multipleSize).RotatedBy(rotated);
                VertexPositionColorTexture2D upClass = new(oldCenter - posOffset, BinaryStarsMain.TrailColor, new Vector3(progress, 0, 0f));
                VertexPositionColorTexture2D downClass = new(oldCenter + posOffset, BinaryStarsMain.TrailColor, new Vector3(progress, 1, 0f));
                list.Add(upClass);
                list.Add(downClass);
            }
            if (list.Count >= 3)
            {
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            }
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShouldDrawVertex = false;
            //从灾厄上抄下来的, 由于有一些特殊效果所以粒子会少一点
            float numberOfDusts = 4f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                int dType = Main.rand.NextBool(2) ? DustID.GemDiamond : DustID.WitherLightning;
                float rot = ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(4.8f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                Vector2 velOffset = new Vector2(4f, 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 4.1f));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, dType, new Vector2(velOffset.X, velOffset.Y), 0, default, 0.3f);
                dust.noGravity = true;
                dust.velocity = velOffset;
                dust.scale = 1.2f;
            }
            SoundEngine.PlaySound(BinaryStarsMain.HitSound with {Volume = 0.8f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item109 with {MaxInstances = 1, Pitch = 0.2f, PitchVariance = 0.1f }, Owner.Center);
            TargetIndex = target.whoAmI;
            if (Projectile.numHits % 2 is 0)
            {
                int ownedProjCounts = Owner.ownedProjectileCounts[Type]; 
                //每轮生成两个，超过3把以上的锤子在场时生成一个
                int maxCount = ownedProjCounts < 3 ? 2 : 1;
                SpawnNebulaShot(Owner, Projectile, target, maxCount, damage:(int)(Projectile.damage * 0.2f));
            }
            
        }

        public static void SpawnNebulaShot(Player owner, Projectile projectile, NPC target, int maxSpawnCounts = 2, bool canSpawnDust = true, int? damage = null)
        {
            //灾厄抄写下来的
            projectile.netUpdate = true;
            int useDamage = damage ?? projectile.damage;
            for (int i = 0; i < maxSpawnCounts; ++i)
            {
                //确定位置
                Vector2 spawnPosBase = (owner.MountedCenter - target.Center).SafeNormalize(Vector2.UnitX);
                float warpRadians = Main.rand.NextFloat(-PiOver2 * 0.45f, PiOver2 * 0.45f);
                Vector2 warpOffset = 150f * spawnPosBase.RotatedBy(warpRadians);
                Vector2 spawnPos =  owner.MountedCenter + warpOffset * Main.rand.Next(6, 9) * 0.25f;
                //确定初始速度，精准一些。
                Vector2 velDir = (target.Center - spawnPos).SafeNormalize(Vector2.UnitX);
                SpawnDust(spawnPos, velDir);
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile proj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), spawnPos, velDir * Main.rand.NextFloat(15f, 19f), ProjectileType<BinaryStarsLightArrow>(), useDamage, 2.5f, projectile.owner, target.whoAmI);
                    proj.ai[0] = target.whoAmI;
                    proj.HJScarlet().ExtraAI[1] = canSpawnDust.ToInt();
                }
            }
        }
        private static void SpawnDust(Vector2 spawnPos, Vector2 dir)
        {
            float baseRot = dir.ToRotation() + PiOver2;
            int totalParticleCounts = 8;
            int repeatedCountForAxis = 24;
            for (int k = 4; k < repeatedCountForAxis - 4; k++)
            {
                //在外部调用这个以整体对点位进行偏移。
                float shortAxis = k * 1.7f;
                float longAxis = (repeatedCountForAxis - k) * 1.7f;
                for (int j = 0; j < totalParticleCounts; j++)
                {
                    //将所有的点位均匀分布
                    float angle = j * (float)(TwoPi / totalParticleCounts);
                    //而后使用封装的一个自定义方法，为射弹自动分配自己的位置
                    Vector2 edge = spawnPos + GetCertainPointBaseOnVectorCircle(angle, shortAxis, longAxis, baseRot);
                    Color drawColor = Color.Lerp(BinaryStarsMain.TrailColor with { A = 75 }, Color.MediumPurple with { A = 75 }, (totalParticleCounts - j) / (float)totalParticleCounts);
                    ShinyOrbParticle orbs = new ShinyOrbParticle(edge, dir * 0.2f, drawColor, 30, Main.rand.NextFloat(0.11f, 0.22f), BlendStateID.Alpha);
                    orbs.Spawn();
                }
            }
            //在中心点位额外绘制一个orb
             new ShinyOrbParticle(spawnPos, dir * 0.2f, Color.Violet, 30, 0.75f, glowCenter:false).Spawn();
             new ShinyOrbParticle(spawnPos, dir * 0.2f, Color.MediumPurple, 30, 0.45f, glowCenter:false).Spawn();
        }
        /// <summary>
        /// 基于圆+极坐标的复杂计算来获取需要的位置
        /// </summary>
        public static Vector2 GetCertainPointBaseOnVectorCircle(float radians, float shortAxis, float longAxis, float rotation = 0f)
        {
            //极坐标转化
            float x = longAxis * (float)Math.Cos(radians);
            float y = shortAxis * (float)Math.Sin(radians);

            //转化你输入的rotation，让整个图整体旋转一定角度
            float cosRot = (float)Math.Cos(rotation);
            float sinRot = (float)Math.Sin(rotation);

            //最后转化为实际需要的点位
            float rotX = x * cosRot - y * sinRot;
            float rotY = x * sinRot + y * cosRot;
            return new Vector2(rotX, rotY);
        }

        public override bool PreKill(int timeLeft)
        {
            //即将死亡的时候，生成一个克隆锤子。
            int projID = ProjectileType<BinartStarsPhantomClone>();
            //获取当前锤子到玩家的向量，归一化后转90°
            Vector2 dir = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.UnitX).RotatedBy(PiOver2 * IsFlip.ToDirectionInt());
            //转化为实际速度
            Vector2 vel = dir * 18f;
            //直接追加这个射弹。
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, projID, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
            proj.ai[2] = TargetIndex;
            proj.localAI[0] = IsFlip.ToDirectionInt();
            return true;
        }
        
    }
}