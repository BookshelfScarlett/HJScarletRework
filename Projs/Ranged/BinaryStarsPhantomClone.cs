using HJScarletRework.Assets.Registers;
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
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Ranged
{
    public class BinaryStarsPhantomClone : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        //攻击枚举
        private enum AttackStyle
        {
            DoShooted,
            DoReturning,
            DoArcRotating,
            DoReverse
        }
        public override string Texture => GetInstance<BinaryStarsMain>().Texture;
        //切换至返程玩家的AI时间
        private const float SwitchToReturning = 12f;
        //进行圆弧运动的总时间
        private const float TotalArcDuration = 30;
        //允许的旋转弧度。这里取180
        private const float TotalArcAngle = Pi;
        //是否进行了圆弧运动的初始化
        private bool _initArcRotation = false;
        //进入圆弧前的初始角度
        private float _arcStartRotation;
        //进入圆弧前的初始速度
        private float _originalSpeed;
        private float CurArcRotation = 0f;
        private AttackStyle AttackType
        {
            get => (AttackStyle)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        private int TargetIndex => (int)Projectile.ai[2];
        private float InitCenterX => Projectile.localAI[0];
        private float InitCenterY => Projectile.localAI[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1f;
        }
        public override void AI()
        {
            if (AttackType != (AttackStyle)(-1))
                Projectile.rotation = Projectile.velocity.ToRotation();
            switch (AttackType)
            {
                case AttackStyle.DoShooted:
                    IsShooted();
                    break;
                case AttackStyle.DoReturning:
                    IsReturning();
                    break;
                case AttackStyle.DoArcRotating:
                    IsArcRotating();
                    break;
                case AttackStyle.DoReverse:
                    IsReverse();
                    break;
                default:
                    IsFading();
                    break;
            }
        }

        private void IsFading()
        {
            if (Projectile.velocity.Length() > 0.01f)
                Projectile.velocity *= 0.84f;
            else
                Projectile.velocity *= 0f;
            Projectile.Opacity -= 0.02f;
            if (Projectile.velocity.Length() <= 0f && Projectile.Opacity <= 0f)
                Projectile.Kill();
        }

        //制作一个小的圆弧运动
        private void IsArcRotating()
        {
            bool isFlip = Projectile.localAI[0] == 1f;
            float realArcAngle = TotalArcAngle * isFlip.ToDirectionInt();
            //初始化这个圆弧运动
            if (!_initArcRotation)
            {
                _initArcRotation = true;
                _arcStartRotation = Projectile.velocity.ToRotation();
                _originalSpeed = Projectile.velocity.Length();
                //根据原始速度的大小提供不同的减速
                float decayFactor = 1.2f;
                //结果在 0f ~ 1f 之间
                float normalized = 1f / (1f + _originalSpeed * decayFactor);
                //映射到 0.4f ~ 0.8f
                float arcSpeed = Lerp(0.6f, 0.9f, normalized);
                arcSpeed = Clamp(arcSpeed, 0.65f, 0.9f);
                //降低初始速度以减小旋转半径
                Projectile.velocity *= arcSpeed;
            }
            if (_initArcRotation)
            {
                //当前进程
                float arcProgress = AttackTimer / (TotalArcDuration / 1.5f);
                //物品转角
                CurArcRotation = _arcStartRotation + realArcAngle * arcProgress;
                Projectile.velocity = CurArcRotation.ToRotationVector2() * Projectile.velocity.Length();
                if (arcProgress >= 1f)
                {
                    //恢复初始速度。
                    Projectile.velocity = CurArcRotation.ToRotationVector2() * _originalSpeed;
                    Projectile.netUpdate = true;
                    AttackTimer = 0;
                    AttackType = AttackStyle.DoReverse;
                }
            }
            AttackTimer += 1f;
        }

        private void IsReverse()
        {
            AttackTimer += 1f;
            bool hasTarget = Projectile.GetTargetSafe(out NPC target, TargetIndex, true, canPassWall: true);
            Projectile.extraUpdates = 3 + hasTarget.ToInt();
            //在这个过程中持续检查target合法性。
            if (hasTarget)
                Projectile.HomingTarget(target.Center, 9999f, 20f + AttackTimer * 0.8f, 10f);

        }

        private void IsReturning()
        {
            AttackTimer += 1f;
            Projectile.HomingTarget(Owner.Center, 1800f, 20f + AttackTimer / 2f, 10f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                //否则切换至第三个状态。
                AttackType = AttackStyle.DoArcRotating;
                Projectile.netUpdate = true;
                Projectile.penetrate = -1;
                Projectile.localNPCHitCooldown = -1;
                AttackTimer = 0f;
            }
        }

        private void IsShooted()
        {
            //初始发射。
            AttackTimer += 1f;
            if (AttackTimer > SwitchToReturning)
            {
                AttackType = AttackStyle.DoReturning;
                AttackTimer = 0f;
                Projectile.netUpdate = true;
                //收回并拐弯时播报落星的声音
                SoundEngine.PlaySound(SoundID.Item4 with { Volume = 1f, Pitch = 0.8f }, Owner.Center);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //从灾厄抄写的锤子特效
            SoundStyle select = Utils.SelectRandom(Main.rand, HJScarletSounds.Smash_AirHeavy);
            SoundEngine.PlaySound(select, Projectile.Center);
            float damageInterpolant = Utils.GetLerpValue(950f, 1600f, hit.Damage, true);
            Vector2 splatterDirection = Projectile.velocity;
            for (int i = 0; i < 8; i++)
            {
                int sparkLifetime = Main.rand.Next(55, 70);
                float sparkScale = Main.rand.NextFloat(0.7f, Main.rand.NextFloat(3.3f, 5.5f)) + damageInterpolant * 0.85f;
                Color sparkColor = Color.Lerp(Color.Purple, Color.GhostWhite, Main.rand.NextFloat(0.7f));
                sparkColor = Color.Lerp(sparkColor, Color.HotPink, Main.rand.NextFloat());

                Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.7f) * Main.rand.NextFloat(1f, 1.2f);
                sparkVelocity.Y -= 7f;
                new StarShape(Projectile.Center, sparkVelocity, sparkColor, sparkScale, sparkLifetime).Spawn();
            }
            if (AttackType != AttackStyle.DoReverse)
                return;
            //命中时的圆环
            //强转化为不存在的类型
            AttackType = (AttackStyle)(-1);
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 0.5f;
            for (int i = 0; i < 36; i++)
            {
                Vector2 dir2 = ToRadians(i * 10f).ToRotationVector2() * 0.5f;
                dir2.X /= 3.6f;
                dir2 = dir2.RotatedBy(Projectile.velocity.ToRotation());
                Vector2 pos = Projectile.Center + dir * 8f + dir2 * 10f;
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(pos, dir2 * 5f, Main.rand.NextBool() ? Color.White : Color.HotPink, 40, 3.5f - Math.Abs(18f - i) / 6f);
                shinyOrbParticle.Spawn();
            }
            //这里会给一个爆冲
            Projectile.velocity *= 1.5f;
            Projectile.extraUpdates = 0;
            SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, HJScarletSounds.Smash_AirHeavy);
            SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.6f, 0.7f), Volume = 0.7f, MaxInstances = 1 }, target.Center);
        }
        #region DrawMethod
        //DrawProjWidth
        public float SetProjWidth(float ratio)
        {
            float width = 100;
            width *= SmoothStep(0.8f, 0.6f, Utils.GetLerpValue(0f, 0.5f, ratio, true));
            return width;
        }
        public Color SetTrailColor(float ratio)
        {
            float velocityOpacityFadeout = Utils.GetLerpValue(2f, 5f, Projectile.velocity.Length(), true);
            Color c = BinaryStarsMain.TrailColor * Projectile.Opacity * 0.8f * (1f - ratio);
            return c * Utils.GetLerpValue(0.04f, 0.08f, ratio, true) * velocityOpacityFadeout;
        }
        //DrawOffset
        public Vector2 PrimitiveOffsetFunction(float ratio)
        {
            Vector2 pos = Projectile.Size * 0.4f + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.scale * 0.5f;
            return pos;
        }
        #endregion

        //TODO：下面那个轨迹把归元漩涡的轨迹改成另外一种，现在这个纯纯占位符
        public override bool PreDraw(ref Color lightColor)
        {
            float value2 = Projectile.Opacity * ( 1 + (Projectile.Opacity < 1f).ToInt() * 1f);
            Projectile.DrawGlowEdge(Color.White * Projectile.Opacity, posMove: 2 * Projectile.Opacity,rotFix: -PiOver4);
            Projectile.DrawProj(Color.White * value2, 4, 0.7f, -PiOver4);
            if (!HJScarletMethods.OutOffScreen(Projectile.Center) && Projectile.velocity.Length() > 0f)
            {
                SB.End();
                SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                float spinRotation = Main.GlobalTimeWrappedHourly * 5.2f;
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Violet);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Orchid, 0.4f, 0.8f, offsetHeight: 12f);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Orchid, 0.4f, 0.8f, offsetHeight: -12f);
                DrawTrails(HJScarletTexture.Trail_ParaLine.Texture, Color.White, 0.4f, alphaValue: 1f);

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
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue * Projectile.Opacity);
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
            List<ScarletVertex> list = [];
            int totalpoints = validPosition.Count;
            //创建顶点列表
            for (int i = 0; i < validPosition.Count - 1; i++)
            {
                float progress = (float)i / (validPosition.Count - 1);
                float rotated = (validPosition[i + 1] - validPosition[i]).ToRotation();
                Vector2 oldCenter = validPosition[i] + Projectile.Size / 2 + rotated.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight - Main.screenPosition;
                Vector2 posOffset = new Vector2(0, SetProjWidth(progress) * multipleSize).RotatedBy(rotated);
                ScarletVertex upClass = new(oldCenter - posOffset, BinaryStarsMain.TrailColor, new Vector3(progress, 0, 0f));
                ScarletVertex downClass = new(oldCenter + posOffset, BinaryStarsMain.TrailColor, new Vector3(progress, 1, 0f));
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