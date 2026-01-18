using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class TheDisasterProj : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => ProjPath  + nameof(TheDisaster);
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(16, 2);
        public enum Style
        {
            Shoot,
            StabOnTarget,
            StabOnGround
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public Vector2 StabPosition = Vector2.Zero;
        public ref int TargetIndex => ref Projectile.HJScarlet().GlobalTargetIndex;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = -1;
        }
        //AI总控的基本上是特效方面的绘制
        //和转角问题
        public override void AI()
        {
            switch (AttackType)
            {
                case Style.Shoot:
                    DoShoot();
                    break;
                case Style.StabOnTarget:
                    DoStabOnTarget();
                    break;
                case Style.StabOnGround:
                    DoStabOnGround();
                    break;
            }

        }

        private void DoStabOnTarget()
        {
            if (Projectile.GetTargetSafe(out NPC target, false))
            {
                Projectile.Center = target.Center + StabPosition;

            }
            else
                Projectile.Kill();
        }

        private void DoStabOnGround()
        {
        }

        public void DoShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            
        }

        public override bool PreKill(int timeLeft)
        {
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //刺入墙体也和刺入敌人差不多
            //考虑到可能存在的情况，这里需要过一个ai枚举的判定
            if(AttackType== Style.StabOnTarget || Projectile.velocity == Vector2.Zero)
            {
                //如果是target状态，直接返回false避免处死射弹
                return false;
            }
            //将矛体沿着转角方向刺入
            Projectile.position += Projectile.SafeDirByRot() * 60f;
            //做掉射弹速度
            Projectile.velocity *= 0f;
            //干掉eu状态，方便处理无敌帧
            Projectile.extraUpdates = 0;
            //刷新生命值为600
            Projectile.timeLeft = 600;
            //切换ai
            Projectile.netUpdate = true;
            AttackType = Style.StabOnGround;
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if(AttackType == Style.StabOnTarget)
            {
                //生成日曜爆炸
            }
        }
        public override bool ShouldUpdatePosition() => AttackType == Style.Shoot && Projectile.velocity.Length() > 0f;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //将taget传入
            if (TargetIndex == -1 && AttackType == Style.Shoot)
            {
                TargetIndex = target.whoAmI;
                //做掉射弹速度
                Projectile.velocity *= 0f;
                //干掉eu状态，方便处理无敌帧
                Projectile.extraUpdates = 0;
                //刷新生命值为600
                Projectile.timeLeft = 600;
                Projectile.netUpdate = true;
                StabPosition = target.Center - Projectile.Center;
                AttackType = Style.StabOnTarget;
            }

            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
