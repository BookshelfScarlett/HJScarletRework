using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Ranged;
using Terraria;

namespace HJScarletRework.Projs.Ranged
{
    public class BlazingSunHeldProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        public override string Texture => GetInstance<BlazingSun>().Texture;
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(GetInstance<BlazingSun>().Item, GetInstance<BlazingSun>().Item.useTime * Projectile.MaxUpdates, 5 * Projectile.MaxUpdates);
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void OnFirstFrame()
        {
            if (!Owner.HasProj<BlazingSunFireball>())
            {
                for (int i = -1; i < 2; i += 2)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<BlazingSunFireball>(), Projectile.originalDamage, 1f, Projectile.owner);
                    proj.ai[1] = i;
                }
            }

            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            UpdateBowStatment();
            UpdateDeadState();
            UpdateAttack();
            UpdatePlayerStatement();
        }

        public void UpdateAttack()
        {
            if (!(Owner.channel && !Owner.noItems && !Owner.CCed))
            {
                if (Timer < AttackSpeed / 3)
                    Timer++;
                return;
            }
            Owner.itemAnimation = Owner.itemTime = 2;
            Timer++;
            if (Timer < AttackSpeed / 3)
                return;
            int c = Main.rand.Next(1, 3);
            for (int i = 0; i < c; i++)
            {
                Vector2 firePos = Projectile.Center - new Vector2(-15f * Projectile.direction, Main.rand.Next(-25, 26)).BetterRotatedBy(Projectile.rotation);
                Vector2 dir = Projectile.SafeDirByRot();
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), firePos, dir * Main.rand.NextFloat(.9f, 1.1f) * 16f, ProjectileType<BlazingSunArrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            ScarletSound(HJScarletSounds.HymnFireball_Release, Projectile.Center, instances: 0, pitch: -.4f, pitchVariance: .1f);
            Timer = 0;
        }

        private void UpdateBowStatment()
        {
            //Projectile.velocity = Projectile.Center.GetNormalVector2(Main.MouseWorld);
        }

        public void UpdatePlayerStatement()
        {
            Projectile.rotation = Owner.ToMouseVector2().ToRotation();
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ControlPlayerArm(Projectile.rotation);
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
        }
        public void UpdateDeadState()
        {
            if (!Owner.IsHolding<BlazingSun>() || Owner.dead || Owner.CCed)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetRangedWeaponHeldProjData(out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float drawRot, out SpriteEffects se);
            drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            drawPos -= new Vector2(-10 * Owner.direction, 5).BetterRotatedBy(drawRot);
            SB.FastDraw(tex, drawPos, Color.White, drawRot, rotPoint, Projectile.scale, se);
            return false;
        }
    }
}
