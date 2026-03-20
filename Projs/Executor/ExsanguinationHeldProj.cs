using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class ExsanguinationHeldProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<Exsanguination>().Texture;
        public int ExecutionTime = GetInstance<Exsanguination>().ExecutionTime;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Projectile.type] = true;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
            Projectile.extraUpdates = 0;
            Projectile.noEnchantmentVisuals = true;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void ProjAI()
        {
            if (CheckOwnerDead())
                return;
            UpdatePlayerState();
            UpdateHeldAnimation();
            UpdateAttack();
            Projectile.netUpdate = true;
        }

        private void UpdateAttack()
        {
            Projectile.timeLeft = 2;
            Timer++;
            if (Timer % 2f == 0)
                SoundEngine.PlaySound(HJScarletSounds.Light_Fire, Projectile.Center);
            if (Timer % 1f == 0)
            {
                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 safedir = Projectile.rotation.ToRotationVector2();
                    Vector2 shootPos = Projectile.Center + safedir * 60f - (safedir.RotatedBy(PiOver2) * 5f * Projectile.direction);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), shootPos + safedir.RotatedBy(PiOver2 * i) * 7f * Main.rand.NextFloat(), safedir * 10f, ProjectileType<ExsanguinationBulletProj>(), Projectile.damage, Projectile.knockBack);
                }
            }
        }

        private void UpdateHeldAnimation()
        {
            //震动这把枪。
            Projectile.position += Main.rand.NextVector2Circular(1.3f, 1.3f);
            Projectile.rotation = Owner.ToMouseVector2().ToRotation();
            
        }

        public bool CheckOwnerDead()
        {
            bool ifStillUse = (Owner.channel || Owner.controlUseTile) && !Owner.noItems && !Owner.CCed;
            if(!ifStillUse)
            {
                Projectile.Kill();
                return true;
            }
            return false;
        }
        private void UpdatePlayerState()
        {
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemAnimation = Owner.itemTime = 2;
            Owner.ControlPlayerArm(Projectile.rotation);
            Projectile.Center = Owner.MountedCenter;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offset = new(15 * Owner.direction, 5);
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 rotationPoint = tex.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(tex, drawPos + offset.RotatedBy(drawRot), null, Projectile.GetAlpha(lightColor), drawRot, rotationPoint, Projectile.scale * Main.player[Projectile.owner].gravDir * 0.6f, flipSprite, default);
            return false;
        }
    }
}
