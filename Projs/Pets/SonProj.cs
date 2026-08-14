using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;

namespace HJScarletRework.Projs.Pets
{
    public class SonProj : ScarletPetProjClass
    {
        public override int TotalFrames => 6;
        public override void SimplePetFunction()
        {
            SimplePetAnimation(4);

            if (Owner.HJScarlet().sonPet)
                Projectile.timeLeft = 2;
            if (Owner.dead)
                Owner.HJScarlet().sonPet = false;
        }
        public override void PetAI()
        {
            Vector2 mountedPos = Owner.MountedCenter - Vector2.UnitY * 90;
            Projectile.Center = Vector2.Lerp(Projectile.Center, mountedPos, 0.50f);
            Lighting.AddLight(Owner.Center, new Vector3(255, 255, 255) * .1f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D tex = Projectile.GetTexture();
            Rectangle frame = tex.Frame(1, TotalFrames, 0, Projectile.frame);
            Vector2 ori = frame.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            SpriteEffects se = Owner.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            sb.EnterShaderArea();
            float lerp = (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly / 2f));
            float lerpScale = Lerp(0.90f, 1.1f, lerp);
            Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
            float scale = Projectile.scale * .42f * lerpScale;
            sb.Draw(glow, pos, null, Color.White * .95f, 0, glow.Size() / 2, scale, 0, 0);
            sb.Draw(glow, pos, null, Color.White * .95f, 0, glow.Size() / 2, scale, SpriteEffects.FlipHorizontally, 0);
            Texture2D orb = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            float orbScale = Projectile.scale * .42f * lerpScale;
            sb.Draw(orb, pos, null, Color.White * .75f, 0, orb.Size() / 2, orbScale, 0, 0);
            sb.EnterShaderArea(BlendState.NonPremultiplied);
            Texture2D ring = HJScarletTexture.Particle_RingShiny.Value;
            float ringScale = Projectile.scale * .17f * lerpScale;
            sb.Draw(ring, pos, null, Color.White, 0, ring.Size() / 2, ringScale, 0, 0);
            sb.EndShaderArea();

            sb.Draw(tex, pos, frame, Color.White, 0, ori, Projectile.scale, se, 0);
            return false;
        }
    }
}
