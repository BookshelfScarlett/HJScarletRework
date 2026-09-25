using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletFloatingBook : HJScarletProj
    {
        public virtual int ExtraUpdates => 5;
        public virtual int OriginalItemID => -1;
        public virtual int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * Projectile.MaxUpdates, 5 * Projectile.MaxUpdates);
        public AnimationStruct Helper = new(2);
        public ref float Timer => ref Projectile.ai[0];
        public virtual float RotFixer => 0;
        public override void SetStaticDefaults()
        {
            ScarletProjIDSets.IsHeldProj[Type] = true;
        }
        public virtual void ExSSD() { }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = SetDamageClass;
            Projectile.tileCollide = false;
            Projectile.SetUpHeldProj(ExtraUpdates);
            ExSD();
        }
        /// <summary>
        /// 只支持左键使用
        /// <br>右键只用于将该书本回归至玩家中心</br>
        /// <br>如果考虑多功能用途，请考虑新建一种方式而非占用右键，但现在暂时用不上</br>
        /// </summary>
        public bool IsLeftUsing => Owner.channel && !Owner.noItems && !Owner.CCed;
        public bool IsRightUsing => Owner.controlUseTile && !Owner.noItems && !Owner.CCed;

        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
                OnFirstFrame();
            Projectile.timeLeft = 2;
            if (!Owner.IsHolding(OriginalItemID)||Owner.dead)
            {
                Helper.IsDone[0] = false;
                Helper.IsDone[1] = false;
                Helper.Progress[0] -= 3;
                if (Helper.GetAniProgress(0) < .1f)
                    Projectile.Kill();
            }
            else
            {
                HandleBookAttack();
            }
            GlobalReset();
            //控制漂浮书本的位置。
            Vector2 targetMountedPosition = Owner.GetToMouseVector2(Projectile.Center) * 150f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetMountedPosition.ToSafeNormalize(), .05f);
            float tarRot = Projectile.velocity.ToRotation();
            float beginRot = Projectile.rotation;
            float value = WrapAngle(tarRot - beginRot);
            Projectile.rotation = beginRot + value;
            bool reverse = !Helper.IsDone[0] && !Owner.IsHolding(OriginalItemID) || Owner.controlUseTile;
            Vector2 tarPos = Owner.MountedCenter + Owner.Center.GetNormalVector2(Main.MouseWorld).ToSafeNormalize() * 60;
            if (reverse)
                tarPos = Owner.MountedCenter + Owner.Center.GetNormalVector2(Main.MouseWorld).ToSafeNormalize() * 0;
            Projectile.Center = Vector2.Lerp(Projectile.Center, tarPos, .05f);
            Projectile.position.Y += (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 1.1f) * 0.5f);
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X - Projectile.Center.X > 0).ToDirectionInt();

            if (IsLeftUsing)
            {
                Owner.ChangeDir(Projectile.direction);
                Owner.itemTime = Owner.itemAnimation = 2;
                Owner.ControlPlayerArm(Projectile.rotation, 2);
            }
        }
        protected virtual void GlobalReset()
        {

        }
        public float PrevProgress1 = 0;
        public override void OnFirstFrame()
        {
            PrevProgress1 = -1;
            Helper.MaxProgress[0] = (int)(AttackSpeed * .75f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .35f);
        }
        public void HandleBookAttack()
        {
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
            }
            else
            {
                if (IsLeftUsing)
                {
                    PrevProgress1 = Helper.Progress[1];
                    if (Helper.OnAnimationBegin(1))
                        ScarletSound(HJScarletSounds.Evolution_Thrown, Projectile.Center, pitch: .71f);
                    if (!Helper.IsDone[1])
                        Helper.UpdateAniState(1);
                    else
                        Timer++;
                    if (Helper.IsDone[1])
                    {
                        if (IsLeftUsing)
                            HandleLeftAttack();
                        else
                            HandleRightAttack();
                    }
                }
                else
                {
                    if (PrevProgress1 == Helper.Progress[1])
                        ScarletSound(HJScarletSounds.Evolution_Thrown, Projectile.Center, pitch: .91f);
                    Helper.Progress[1]--;
                    Helper.IsDone[1] = false;
                    if (Helper.Progress[1] <= 0)
                        Helper.Progress[1] = 0;
                }
            }
        }
        public virtual void HandleLeftAttack() { }
        public virtual void HandleRightAttack() { }
        public override bool PreDraw(ref Color lightColor)
        {
            float progress1 = EaseOutBack(Helper.GetAniProgress(0));
            float progress2 = EaseOutBack(Helper.GetAniProgress(1));
            float globalTimeProgress = Lerp(0.95f, 1.05f, (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f)));
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation - RotFixer + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 origin = tex.Size() / 2;
            Vector2 realDrawPos = drawPos;
            SpriteEffects se = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, realDrawPos + (TwoPi / 16f * i).ToRotationVector2() * 1.2f * progress2, null, Color.White.ToAddColor() * progress2, rotation, origin, Projectile.scale * progress2, se, 0);
            SB.Draw(tex, realDrawPos, null, Color.Lerp(Color.Transparent, Color.White, progress1), rotation, origin, Projectile.scale * progress1, se, 0);
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
    }
}
