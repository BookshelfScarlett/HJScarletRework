using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;

namespace HJScarletRework.Projs.Pets
{
    public class DracoProj : ScarletPetProjClass
    {
        public override int TotalFrames => 6;
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.ignoreWater = true;
        }
        public override void SimplePetFunction()
        {
            SimplePetAnimation(12f);
            if (Owner.dead)
                Owner.HJScarlet().dracoPet = false;
            if (Owner.HJScarlet().dracoPet)
                Projectile.timeLeft = 2;
        }
        public float Osci = 0;
        public override void PetAI()
        {
            Osci += ToRadians(1f);
            float mountedPosX = Owner.MountedCenter.X - 80f * Owner.direction;
            float mountedPosY = Owner.MountedCenter.Y + (float)(Math.Sin(Osci) * 5f) - 10f;
            Vector2 mountedPos = new(mountedPosX, mountedPosY);
            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, 0.10f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D tex = Projectile.GetTexture();
            Rectangle frame = tex.Frame(1, TotalFrames, 0, Projectile.frame);
            Vector2 ori = frame.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            SpriteEffects se = Owner.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sb.Draw(tex, pos, frame, Color.White, 0, ori, Projectile.scale*.82f, se, 0);
            return false;
        }
    }
}
