using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Globals.Players;
using HJScarletRework.Items.Weapons.Ranged;
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
    public class BinaryStarsMain: ThrownHammerProj, ILocalizedModType
    {
        protected override BoomerangDefault BoomerangStat => new(
            returnTime: 34,
            returnSpeed: 28f,
            acceleration: 0.4f,
            killDistance: 1800
        );
        private enum DoType
        {
            IsShooted,
            IsReturning,
            IsStealth
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public static Color TrailColor => new(255, 142, 246);
        public static readonly SoundStyle HitSound = SoundID.Item88 with { Volume = 0.85f }; //Item88:使用流星法杖的音效
        public ref bool Update => ref Projectile.netUpdate;
        public bool CanDrawTrail
        {
            get => ModProj.ExtraAI[0] == 1f;
            set => ModProj.ExtraAI[0] = value ? 1f : 0f;
        }
        public ref float SpriteRotation => ref ModProj.ExtraAI[1];
        public override string Texture => GetInstance<BinaryStars>().Texture;
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.width = 86;
            Projectile.height = 72;
            Projectile.usesLocalNPCImmunity = true;
            //4eu, 12 => 3
            Projectile.localNPCHitCooldown = 40;
            Projectile.extraUpdates = 4;
        }
        public override void AI()
        {
            //这里需要手动处死
            Projectile.timeLeft = 2;
            DoGeneric();
            switch (AttackType)
            {
                case DoType.IsShooted:
                    DoShooted();
                    break;
                case DoType.IsReturning:
                    DoReturning();
                    break;
                case DoType.IsStealth:
                    DoStealth();
                    break;
            }
        }
        public override bool PreKill(int timeLeft)
        {
            if (AttackType is DoType.IsStealth && _drawArcTime > 0 && Stealth)
            {
                SoundStyle select = Utils.SelectRandom(Main.rand, HJScarletSounds.Smash_AirHeavy);
                SoundEngine.PlaySound(select, Projectile.Center);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, -80 * Owner.direction, 23, Projectile.rotation, 0.2f, true, 1000);
            }
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Stealth)
            {
                DrawHitSpark(target, hit.Damage);
                if (_drawArcTime > 0)
                {
                    StealthHit(target, hit.Damage, target.whoAmI);
                    Projectile.Kill();
                }
            }
            else
            {
                if (Projectile.numHits < 5)
                    NormalHit(target);
                TargetIndex = target.whoAmI;
                SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, HJScarletSounds.Smash_AirHeavy);
                SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.3f, 0.5f), Volume = 0.7f, MaxInstances = 1 }, target.Center);
            }
        }
        private void DrawHitSpark(NPC target, int damage)
        {
            SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, HJScarletSounds.Smash_GroundHeavy);
            SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.8f, 0.7f), Volume = 0.7f, MaxInstances = 1 }, target.Center);
            PrettySpark(damage);
        }
        public void StealthHit(NPC target, int hitDamage, int targetIndex)
        {
            for (int i = -1; i < 2; i += 2)
            {
                //归一化方向并提供自定义速度
                Vector2 direc = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Vector2 spawnVelocity = (direc * 32f).RotatedBy(PiOver4 * i);
                //两把锤子有属于自己的转角
                //不会这里写成了一个终极史山吧？
                float arcAngle = i * (PiOver2 + PiOver4 * 1.1f);
                int echo = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, spawnVelocity, ProjectileType<BinartStarsPhantom>(), Projectile.damage / 2, 0f, Owner.whoAmI, targetIndex, 0f, arcAngle);
                Main.projectile[echo].HJScarlet().FocusStrike = true;
            }
        }
        public void NormalHit(NPC target)
        {
            ModProj.IsHitOnEnablFocusMechanicProj = true;
            int dustSets = Main.rand.Next(5, 8);
            int dustRadius = 6;
            Vector2 corner = new(target.Center.X - dustRadius, target.Center.Y - dustRadius);
            for (int i = 0; i < dustSets; ++i)
            {
                float scaleOrb = 1.2f + Main.rand.NextFloat(1f);
                Dust orb = Dust.NewDustDirect(corner, 2 * dustRadius, 2 * dustRadius, DustID.Clentaminator_Purple);
                orb.noGravity = true;
                orb.velocity *= 4f;
                orb.scale = scaleOrb;
                for (int j = 0; j < 6; ++j)
                {
                    float scaleSparkle = 0.8f + Main.rand.NextFloat(1.1f);
                    Dust sparkle = Dust.NewDustDirect(corner, 2 * dustRadius, 2 * dustRadius, DustID.ShadowbeamStaff);
                    sparkle.noGravity = true;
                    float dustSpeed = Main.rand.NextFloat(10f, 18f);
                    sparkle.velocity = Main.rand.NextVector2Unit() * dustSpeed;
                    sparkle.scale = scaleSparkle;
                }
            }
            //生成一点星云射线。
            Projectile.netUpdate = true;
            BinartStarsPhantom.SpawnNebulaShot(Owner, Projectile, target, 2, false);
        }
        private void PrettySpark(int hitDamage)
        {
            //圆环
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX) * Projectile.scale;
            for (int i = 0; i < 36; i++)
            {
                Vector2 dir2 = ToRadians(i * 10f).ToRotationVector2() * Projectile.scale;
                dir2.X /= 3.6f;
                dir2 = dir2.RotatedBy(Projectile.velocity.ToRotation());
                Vector2 pos = Projectile.Center + dir * 12f + dir2 * 18f;
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(pos, dir2 * 5f, Main.rand.NextBool() ? Color.White : Color.HotPink, 40, 3.5f - Math.Abs(18f - i) / 6f, BlendStateID.Additive);
                shinyOrbParticle.Spawn();
            }
            //从灾厄抄写的锤子特效
            float damageInterpolant = Utils.GetLerpValue(950f, 2000f, hitDamage, true);
            Vector2 splatterDirection = Projectile.velocity * 0.8f;
            for (int i = 0; i < 10; i++)
            {
                int sparkLifetime = Main.rand.Next(55, 70);
                float sparkScale = Main.rand.NextFloat(0.7f, Main.rand.NextFloat(3.3f, 5.5f)) + damageInterpolant * 0.85f;
                Color sparkColor = Color.Lerp(Color.Purple, Color.GhostWhite, Main.rand.NextFloat(0.7f));
                sparkColor = Color.Lerp(sparkColor, Color.HotPink, Main.rand.NextFloat());

                Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.7f) * Main.rand.NextFloat(1.4f, 1.8f);
                sparkVelocity.Y -= 7f;
                 new StarShape(Projectile.Center, sparkVelocity, sparkColor, sparkScale , sparkLifetime).Spawn();
            }
        }
        #region DrawMethod
        public float SetProjWidth(float ratio)
        {
            float width = Projectile.width + 20;
            width *= SmoothStep(0.8f, 0.6f, Utils.GetLerpValue(0f, 0.5f, ratio, true));
            return width;
        }
        public Color SetTrailColor(float ratio)
        {
            float velocityOpacityFadeout = Utils.GetLerpValue(2f, 5f, Projectile.velocity.Length(), true);
            Color c = TrailColor * Projectile.Opacity * (1f - ratio);
            return c * Utils.GetLerpValue(0.04f, 0.1f, ratio, true) * velocityOpacityFadeout;
        }
        public Vector2 PrimitiveOffsetFunction(float ratio)
        {
            Vector2 off = Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.scale * 0.2f * Vector2.UnitX;
            return off;
        }
        #endregion
        //TODO：下面那个轨迹把归元漩涡的轨迹改成另外一种，现在这个纯纯占位符
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White, rotFix: -PiOver4);
            Projectile.DrawProj(Color.White, 4, 0.7f, -PiOver4);
            if (!HJScarletMethods.OutOffScreen(Projectile.Center) && Stealth)
            {
                SB.End();
                SB.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                float spinRotation = Main.GlobalTimeWrappedHourly * 5.2f;
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Violet);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Orchid, 0.4f, 0.8f, offsetHeight: 12f);
                DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Orchid, 0.4f, 0.8f, offsetHeight: -12f);
                DrawTrails(HJScarletTexture.Trail_ParaLine.Texture, Color.White, 0.4f,alphaValue: 1f);
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
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, SetProjWidth(ratio) * multipleSize), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
        private void DoStealth()
        {
            //初始化上一锚点缓存位为玩家中心
            if (_lastAnchorPosition == Vector2.Zero)
                _lastAnchorPosition = Owner.Center;

            if (CanDrawTrail && !Stealth && AttackTimer < 10f)
            {
                AttackTimer += 1;
                return;
            }
            else if (_drawArcTime < 1 && Stealth)
            {
                //小于特定次数前ban掉下方所有AI
                //并且额外生成的锤子别直接去画圆弧，而是冲过去
                DrawDynamicArc();
                return;
            }

            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, true, 4800f))
            {
                //以超高的速度冲向你的敌怪
                Projectile.extraUpdates = 6;
                Projectile.HomingTarget(target.Center, 4800f, 24f, 18f);
            }
            else
                Projectile.extraUpdates = 4;
        }
        private void DoGeneric()
        {
            //潜伏射弹允许绘制轨迹
            if (Stealth)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                return;
            }
            Projectile.rotation += 0.2f;
            if (Main.rand.NextBool(8))
            {
                Vector2 offset = new Vector2(10, 0).RotatedByRandom(ToRadians(360f));
                Vector2 velOffset = new Vector2(2, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.WitherLightning, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(10))
            {
                Vector2 offset = new Vector2(12, 0).RotatedByRandom(ToRadians(360f));
                Vector2 velOffset = new Vector2(4, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.GemDiamond, new Vector2(Projectile.velocity.X * 0.15f + velOffset.X, Projectile.velocity.Y * 0.15f + velOffset.Y), 100, default, 0.8f);
                dust.noGravity = true;
            }
        }
        private void InitIfStealth()
        {
            Projectile.extraUpdates = Projectile.extraUpdates + 2;
            Projectile.localNPCHitCooldown = 20;
            SoundStyle selectOne = Utils.SelectRandom(Main.rand, HJScarletSounds.Hammer_Shoot) with { Volume = 0.8f, MaxInstances = 0 };
            SoundEngine.PlaySound(selectOne, Projectile.Center);
        }
        private void DoShooted()
        {
            //潜伏+初始状态下，执行特殊ACT音效，并获得超高EU
            if (AttackTimer == 0)
            {
                SpriteRotation = Projectile.velocity.ToRotation();
                if (Stealth)
                    InitIfStealth();
            }
        
            AttackTimer += 1;
            if (AttackTimer > BoomerangStat.ReturnTime)
            {
                AttackTimer = 0;
                AttackType = DoType.IsReturning;
                if (Stealth)
                    SpawnSkyFallHammer();
                Update = true;
            }
        }
        private void DoReturning()
        {
            Projectile.HomingTarget(Owner.Center, BoomerangStat.KillDistance, BoomerangStat.ReturnSpeed, 10);
            if (!Projectile.Hitbox.Intersects(Owner.Hitbox))
                return;
            if (Stealth)
            {
                //否则，其他情况下都会执行这个潜伏ai
                AttackType = DoType.IsStealth;
                //重新设定无敌帧
                Projectile.localNPCHitCooldown = 45;
                Update = true; 
            }
            else
            {
                if (ModProj.IsHitOnEnablFocusMechanicProj)
                    Owner.HJScarlet().FocusStrikeTime += 1;
                Projectile.Kill();
                Update = true;
            }
        }

        private void SpawnSkyFallHammer()
        {
            Vector2 mw = Main.MouseWorld;
            Vector2 pos = new Vector2(mw.X + Main.rand.NextFloat(-300f, 300f), mw.Y - 1200f);
            Vector2 vel = (mw - pos).SafeNormalize(Vector2.UnitX) * 22f;
            Projectile extraHammer = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, vel, ProjectileType<BinaryStarsClone>(), Projectile.damage / 2, Projectile.knockBack);
            extraHammer.extraUpdates = 6;
        }
        #region 一个比较顺滑的，圆弧运动示例
        /// <summary>
        /// 射弹锚点“前一次更新”的位置，初始设定为Vector.Zero，在下一帧会被瞬间更新
        /// </summary>
        private Vector2 _lastAnchorPosition = Vector2.Zero;
        /// <summary>
        /// 标记射弹是否开始“绕圆弧旋转”
        /// </summary>
        private bool _isArcRotating = false;
        /// <summary>
        /// 存储射弹进入圆弧运动前的转角
        /// </summary>
        private float _arcStartRotation;
        private Vector2 _rotCenter;
        /// <summary>
        /// 总圆弧的运动角
        /// </summary>
        private readonly float TotalArcAngle = ToRadians(360f);
        /// <summary>
        /// 存储射弹开始绕圆弧旋转前的原始速度
        /// </summary>
        private float _originalSpeed;
        /// <summary>
        /// 过渡到圆弧运动起始点的判定
        /// </summary>
        private bool _isMovingStartPoint = false;
        /// <summary>
        /// 过度到圆弧运动起始点的进度
        /// </summary>
        private float _startTransitionProgress = 0f;

        /// <summary>
        /// 绕圆弧的旋转次数
        /// </summary>
        private int _drawArcTime = 0;
        /// <summary>
        /// 总圆弧绘制时间
        /// </summary>
        private const float ArcDuration = 50f;
        /// <summary>
        /// 圆弧半径
        /// </summary>
        private const float ArcRadius = 12 * 16;
        #endregion
        #region ArcRotation
        /// <summary>
        /// 超级大史山，必须得找个时间重构了
        /// </summary>
        private void DrawDynamicArc()
        {
            //平滑玩家自身位置，因为玩家自身是一个不断位移的单位
            _rotCenter = Vector2.Lerp(_lastAnchorPosition, Owner.Center, 0.2f);
            //刷新记录位置
            _lastAnchorPosition = _rotCenter;
            //初始化一次圆弧情况
            if (!_isArcRotating)
            {
                //存入初始转角
                _arcStartRotation = Projectile.velocity.ToRotation();
                //存入原始速度
                _originalSpeed = Projectile.velocity.Length();
                //重置过渡状态
                _startTransitionProgress = 0f;
                //启用过度开始
                _isMovingStartPoint = true;
                _isArcRotating = true;
            }

            if (_isMovingStartPoint)
            {
                _startTransitionProgress += 1f / 20f;
                //这里需要先取用后者可能会用上的“默认第一帧”
                float progress = Clamp(_startTransitionProgress, 0f, 1f);
                //计算圆弧起点位置
                Vector2 nextPosition = _rotCenter + _arcStartRotation.ToRotationVector2() * ArcRadius;
                float possibleFinalRot = _arcStartRotation + TotalArcAngle * 0.02f;
                //使用三次方函数进行缓动
                float posEase = 1f - (float)Math.Pow(1f - progress, 3);
                Projectile.Center = Vector2.Lerp(Owner.Center, nextPosition, posEase);
                //进行方向过度，使其能够在当前方向刀圆弧切线之间平滑
                Vector2 initDir = (nextPosition - Owner.Center).SafeNormalize(Vector2.UnitX);
                Vector2 tarDir = possibleFinalRot.ToRotationVector2().RotatedBy(PiOver2);
                //球形线性插值
                Vector2 curDir = Vector2.SmoothStep(initDir, tarDir, progress);
                //速度过度
                Projectile.velocity = curDir * _originalSpeed * Lerp(0.2f, 0.8f, posEase);
                //旋转过度
                SpriteRotation = Lerp(initDir.ToRotation(), possibleFinalRot, progress);
                //过渡完成
                if (_startTransitionProgress >= 1 || Vector2.Distance(Projectile.Center,nextPosition)<8f)
                {
                    _isMovingStartPoint = false;
                    Projectile.Center = nextPosition;
                    //过渡完成后再调整速度，这里故意放缓
                    Projectile.velocity = tarDir * _originalSpeed * 0.8f;
                    SpriteRotation = _arcStartRotation;
                }
            }
            else if (_isArcRotating)
            {

                AttackTimer += 1;
                float progress = AttackTimer / ArcDuration;
                float curAngle = TotalArcAngle * progress;
                float orbitAngle = _arcStartRotation + curAngle;
                //跟随转角
                SpriteRotation = orbitAngle;

                //切线方向
                Vector2 radiusDir = (Projectile.Center - _rotCenter).SafeNormalize(Vector2.Zero);
                Vector2 wantedTanDir = radiusDir.RotatedBy(PiOver2);
                //实际调整速度
                Vector2 targetVelocity = wantedTanDir * _originalSpeed * 0.8f;
                //接近圆弧终点时开始向最终朝向过渡
                Vector2 finalDir = orbitAngle.ToRotationVector2();
                float transitionStart = 0.8f;
                if (progress >= transitionStart && progress <= 1.0f)
                {
                    //过渡因子
                    float transitionFactor = (progress - transitionStart) / (1.0f - transitionStart);
                    //从切线方向平滑过渡到最终朝向
                    targetVelocity = Vector2.Lerp(targetVelocity, finalDir * _originalSpeed * 0.8f, transitionFactor);
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f);
                //修正半径
                float curRadius = (Projectile.Center - _rotCenter).Length();
                float radiusError = curRadius - ArcRadius;
                Projectile.velocity -= radiusDir * radiusError * 0.1f;
                if (progress > 1)
                {
                    AttackTimer = 0;
                    //自增
                    _drawArcTime++;
                    _isArcRotating = false;
                }
            }
        }
        #endregion

    }
    /// <summary>
    /// 基础回旋镖的相关数据
    /// </summary>
    /// <param name="returnTime">返程时间m/param>
    /// <param name="returnSpeed">返程基础速度</param>
    /// <param name="acceleration">返程加速度</param>
    /// <param name="killDistance">超出距离处死</param>
    public struct BoomerangDefault(int returnTime, float returnSpeed, float acceleration, int killDistance)
    {
        public int ReturnTime = returnTime;
        public float Acceleration = acceleration;
        public float ReturnSpeed = returnSpeed;
        public int KillDistance = killDistance;
    }
    public abstract class ThrownHammerProj : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public HJScarletPlayer ModPlayer => Owner.HJScarlet();
        public HJScarletGlobalProjs ModProj => Projectile.HJScarlet();
        public bool Stealth => ModProj.FocusStrike;
        public int AttackTimer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public int TargetIndex
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        /// <summary>
        /// 基础射弹数据
        /// </summary>
        /// <summary>
        /// 基础回旋镖类模组数据。
        /// returnTime：返程时间
        /// returnSpeed：返程基础速度
        /// acceleration：返程加速度
        /// killDistance：处死距离
        /// </summary>
        protected abstract BoomerangDefault BoomerangStat { get; }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            ExSD();
        }
    }
}