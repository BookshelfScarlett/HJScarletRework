using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Magic;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Magic
{
    public class LivingBoomerangMagicHeldProj : HJScarletProj
    {
        public override string Texture => GetInstance<LivingBoomerangMagic>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(GetInstance<LivingBoomerangMagic>().Item, GetInstance<LivingBoomerangMagic>().Item.useTime * Projectile.MaxUpdates, 5 * Projectile.MaxUpdates);
        public AnimationStruct Helper = new AnimationStruct(2);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 100;
            Projectile.SetupImmnuity(Projectile.MaxUpdates * 30);
        }
        public override bool? CanDamage()
        {
            return true;
        }
        public bool IsUsing => (Owner.channel) && !Owner.dead && !Owner.CCed&&!Owner.noItems;
        public override void OnFirstFrame()
        {
            Helper.Progress[0] = (int)(AttackSpeed * .75f);
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            if (IsUsing)
            {
                HoldIdleState();
                Owner.ChangeDir(Projectile.direction);
                Owner.itemTime = Owner.itemAnimation = 2;
                Owner.ControlPlayerArm(Projectile.rotation, 2);
                Projectile.timeLeft = 2;
            }
            else
                return;
            Owner.AddBuff(BuffID.ManaRegeneration,60);
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                float progress = Helper.GetAniProgress(0);
                Projectile.scale = Lerp(0.15f, 1f, progress);
                Projectile.Opacity = Projectile.scale;
            }
            else
            {
            }
        }

        public void HoldIdleState()
        {
            Vector2 targetMountedPosition = Owner.GetToMouseVector2(Projectile.Center) * 220f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetMountedPosition.ToSafeNormalize(), .05f);
            float tarRot = Projectile.velocity.ToRotation();
            float beginRot = Projectile.rotation;
            float value = WrapAngle(tarRot - beginRot);
            Projectile.rotation = beginRot + value;
            bool reverse = !Helper.IsDone[0] && Owner.HeldItem.type != ItemType<PestilenceFlower>() || Owner.controlUseTile;
            Vector2 tarPos = Owner.MountedCenter + Owner.Center.GetNormalVector2(Main.MouseWorld).ToSafeNormalize() * 60;
            if (reverse)
                tarPos = Owner.MountedCenter + Owner.Center.GetNormalVector2(Main.MouseWorld).ToSafeNormalize() * 0;
            float lerpValue = reverse ? 0.10f : .05f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, tarPos, lerpValue);
            Projectile.position.Y += (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 1.1f) * 0.5f);
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X - Projectile.Center.X > 0).ToDirectionInt();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
