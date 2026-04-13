using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.General
{
    public class CowboyBullet : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        int hittime = 0;
        public override void ExSD()
        {
            //碰撞箱是故意放大的
            Projectile.width = Projectile.height = 16;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }
        public override void ProjAI()
        {
            //只需要做这个。
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            new ShinyCrossStar(Projectile.Center, Vector2.Zero, RandLerpColor(Color.LightGoldenrodYellow, Color.DarkOrange), 40, 0, 1, 1f,false).Spawn();
            SoundEngine.PlaySound(SoundID.Dig with {MaxInstances = 0, Pitch = -0.47f}, Projectile.Center); 
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = HJScarletTexture.Texture_WhiteCube.Value;
            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.LightGoldenrodYellow, Projectile.rotation + PiOver2, tex.ToOrigin(), new Vector2(0.18f, 2.1f) * Projectile.scale, 0, 0);
            return false;
        }
    }
}
