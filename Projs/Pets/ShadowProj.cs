using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Pets
{
    public class ShadowProj:ScarletPetProjClass
    {
        public override int TotalFrames => 12;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.LightPet[Type] = true;
            Main.projPet[Type] = true;
        }
        
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void SimplePetFunction()
        {
            SimplePetAnimation(4);
            if (Owner.HJScarlet().ShadowPet)
                Projectile.timeLeft = 2;
            if (Owner.dead)
                Owner.HJScarlet().ShadowPet= false;

            base.SimplePetFunction();
        }
        public override void PetAI()
        {
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D tex = Projectile.GetTexture();
            Rectangle frame = tex.Frame(1, TotalFrames, 0, Projectile.frame);
            Vector2 ori = frame.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            SpriteEffects se = Owner.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sb.Draw(tex, pos, frame, Color.White, 0, ori, Projectile.scale, se, 0);
            return false;
        }
    }
 }
