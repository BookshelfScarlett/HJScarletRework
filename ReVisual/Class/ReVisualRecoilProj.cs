using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.ReVisual.Class
{
    /// <summary>
    /// 一个通用的后坐力射弹
    /// </summary>
    public class ReVisualRecoilProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public void SetUpHoldoutData(int gunType, float recoilPower, int lifeTime, Vector2 holdoutoffset)
        {
            ItemLifeTime = lifeTime;
            GunItemType = gunType;
            RecoilPower = recoilPower;
            HoldProjOffset = holdoutoffset;
        }
        /// <summary>
        /// 这把后坐力武器的”枪“的ID
        /// <br>用于给予后坐力的贴图</br>
        /// </summary>
        public int GunItemType = ItemID.BeeGun;
        public ref float RecoilPower => ref Projectile.ai[0];
        public int ItemLifeTime
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public Vector2 HoldProjOffset = Vector2.Zero;
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(0);
        }
        public override void ProjAI()
        {
            int duration = Owner.itemAnimationMax; // Define the duration the projectile will exist in frames

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }
            Projectile.velocity = Owner.ToMouseVector2();
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dir = Math.Sign(Owner.LocalMouseWorld().X - Owner.Center.X);
            Projectile.spriteDirection = dir;
            Owner.heldProj = Projectile.whoAmI; // Update the player's held projectile id
            Owner.ChangeDir(dir);
            Owner.ControlPlayerArm(Projectile.rotation);

            if (RecoilPower != 0)
            {
                float pullBack = RecoilPower;
                float animationProgress = 0.5f - Owner.itemTime / (float)Owner.itemTimeMax;
                if (animationProgress < .4f)
                    pullBack -= 2.75f * (float)Math.Pow((.6f - animationProgress) / .6f, 2f);
                Projectile.Center = Owner.MountedCenter + Projectile.rotation.ToRotationVector2() * pullBack;
            }
            else
                Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Texture2D tex = TextureAssets.Item[GunItemType].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 offset = HoldProjOffset * new Vector2(Owner.direction, 1);
            float drawRot = Projectile.rotation;
            if (Projectile.spriteDirection < 0)
            {
                drawRot += Pi;
            }
            drawPos += offset.RotatedBy(drawRot);
            Vector2 rotPoint = tex.Size() / 2f;
            SpriteEffects se = (Projectile.spriteDirection * Owner.gravDir) == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SB.Draw(tex, drawPos, null, Color.White, drawRot, rotPoint, Projectile.scale, se, 0);
            return false;
        }
    }
}
