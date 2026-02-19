using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Ranged;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Ranged
{
    public class TheMossMainProj : ThrownHammerProjSecond 
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public enum Style
        {
            Shoot,
            Return
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 60;
            Projectile.tileCollide = true;
            Projectile.light = 0.3f;
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.tileCollide = false;
            Projectile.netUpdate = true;
            //需注意的是，这里的撞墙不会让射弹直接消失。
            if(AttackType == Style.Shoot && Timer < 25f)
                Timer = 25f;
            Projectile.velocity = Projectile.oldVelocity;
            return false;
        }
        public override void CustomHammerAI()
        {
            //初始化的第一帧如果玩家刚好启用了FocusAI，立刻处死当前这个射弹
            if (!Projectile.HJScarlet().FirstFrame)
            {
                if (ActiveFocusMode && !Owner.HasProj<TheMossHeldLock>(out int projID))
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, projID, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    proj.rotation = Projectile.rotation;
                    Projectile.Kill();
                    return;
                }
            }
            //rotatio.
            Projectile.rotation += ToRadians(15f);
            Vector2 orbPos = Projectile.Center.ToRandCirclePos(36f);
            Vector2 dir = (orbPos - Projectile.Center).ToSafeNormalize();
            new ShinyOrbHard(orbPos, dir.ToRandVelocity(ToRadians(2f), 1.2f) * -1f, RandLerpColor(Color.DarkViolet, Color.Black), 20, Main.rand.NextFloat(0.3f, 0.6f)).Spawn();
            if (Main.rand.NextBool())
            {
                Vector2 spawnPosOffset = Projectile.velocity.ToSafeNormalize().RotatedBy(PiOver2) * Main.rand.NextFloat(-18f, 18f);
                Vector2 orbPos2 = Projectile.Center.ToRandCirclePosEdge(5f) + spawnPosOffset;
                Vector2 orbVel = Projectile.velocity.ToRandVelocity(ToRadians(0f), 1f, 1.4f);
                float orbScale = Main.rand.NextFloat(0.4f, 0.6f) * 0.44f;
                new ShinyOrbParticle(orbPos2, orbVel, RandLerpColor(new Color(33, 26, 33), Color.DarkSeaGreen), 20, orbScale).Spawn();
            }

            switch (AttackType)
            {
                case Style.Shoot:
                    DoShoot();
                    break;
                case Style.Return:
                    DoReturn();
                    break;
            }
        }

        private void DoShoot()
        {
            Timer++;
            if(Timer > ReturnTime)
            {
                AttackType = Style.Return;
                //这个是有必要的，经常写以防止忘了
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
                Timer *= 0;
            }
        }

        private void DoReturn()
        {
            Projectile.HomingTarget(Owner.Center, -1, ReturnSpeed, 12f);
            if (!IsIntersectOwner())
                return;
            if (ActiveFocusMode)
            {
                if(Owner.HasProj<TheMossHeldLock>())
                {
                }
            }
            else
            {
                if (IsHitonEnableFocusMechanicProj)
                    PlayerFocusStrikeTime += 1;
            }
            Main.NewText(PlayerFocusStrikeTime);
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.Violet, rotFix: SpriteRotation);
            Projectile.DrawProj(lightColor, rotFix: SpriteRotation, useOldPos: true);
            return false;
        }
    }
    public abstract class ThrownHammerLockProj : HJScarletFriendlyProj
    {
        public virtual int HitBoxSD => 30;
        /// <summary>
        /// 总挂载时间
        /// 这里直接输入秒数，会在初始化的时候自动转化为实际的帧数
        /// </summary>
        public virtual int MountedTime { get; }
        public override ClassCategory Category => ClassCategory.Ranged;
        public sealed override void SetDefaults()
        {
            Projectile.height = Projectile.width = HitBoxSD;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = (MountedTime * 60);
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            ExSD();
        }
    }
    /// <summary>
    /// 给了更多自由度，现在也不要求强行实现回旋镖的基础数据
    /// </summary>
    public abstract class ThrownHammerProjSecond : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public ref bool IsHitonEnableFocusMechanicProj => ref Projectile.HJScarlet().IsHitOnEnablFocusMechanicProj;
        public ref bool ActiveFocusMode => ref Projectile.HJScarlet().FocusStrike;
        public ref int PlayerFocusStrikeTime => ref Owner.HJScarlet().FocusStrikeTime;
        internal float ReturnTime = 30f;
        internal float ReturnSpeed = 12f;
        internal float GetDistance = 50f;
        /// <summary>
        /// 通用的进程计时器（Timer）
        /// 这一计时器覆盖了Projecitle.ai[0]
        /// 因此最好不要对Projectile.ai[0]进行别的操作
        /// </summary>
        public ref float Timer => ref Projectile.ai[0];
        /// <summary>
        /// 射弹的“自转”。需注意的是，这个自转不会作用于Projectile.rotation上
        /// 要么手动让Projectile.rotation = SpriteRotaion,要么写在draw函数内
        /// 推荐后者
        /// </summary>
        public ref float SpriteRotation => ref Projectile.HJScarlet().ExtraAI[0];
        /// <summary>
        /// hitbox.
        /// </summary>
        public virtual int HitBoxSD => 30;
        public sealed override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.height = Projectile.width = HitBoxSD;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            ExSD();
        }
        public sealed override void AI()
        {
            if (Projectile.TooAwayFromOwner())
                Projectile.Kill();
            CustomHammerAI();
        }
        /// <summary>
        /// AI钩子已经被封存硬编码了，如果需要后续更多的ai，只能写在这里
        /// </summary>
        public virtual void CustomHammerAI() { }
        /// <summary>
        /// 重构后的回旋镖数据，现在只需要复写这一项即可直接自动应用回旋镖的“返程”与前进相关的代码
        /// </summary>
        /// <param name="returnTime">发起返程的时间，使用帧数处理</param>
        /// <param name="returnSpeed">返程速度</param>
        /// <param name="getDistance">距离玩家多近时处死射弹，即“回收”锤子的距离</param>
        public virtual void ModifyBoomerangState(ref float returnTime, ref float returnSpeed, ref float getDistance)
        {
            ReturnTime = returnTime;
            ReturnSpeed = returnSpeed;
            GetDistance = getDistance;
        }
        /// <summary>
        /// 封装的一个快捷方法让你直接判定其是否接触玩家。引用getDistaance。
        /// </summary>
        /// <returns></returns>
        public bool IsIntersectOwner() => (Projectile.Center - Owner.Center).LengthSquared() < GetDistance * GetDistance;
        public sealed override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            IsHitonEnableFocusMechanicProj = true;
            ExModifyHitNPC(target, ref modifiers);

        }
        /// <summary>
        /// 封锁了原本的ModifyHitNPC，原本的钩子只用于增加专注攻击的次数了
        /// </summary>
        /// <param name="target"></param>
        /// <param name="modifiers"></param>
        public virtual void ExModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) { }
    }
}
