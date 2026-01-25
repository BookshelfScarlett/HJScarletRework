using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Pets
{
    public class ShadowProj : ModProjectile, ILocalizedModType
    {
        public override string LocalizationCategory => "Projs.Friendly.Pets";
        public override string Texture => $"HJScarletRework/Assets/Texture/Pets/Pet_{GetType().Name}";
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Type] = 11;
            ProjectileID.Sets.LightPet[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileType<NoneProj>());
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            if (Owner.dead)
                Owner.HJScarlet().ShadowPet = false;

            if (Owner.HJScarlet().ShadowPet)
                Projectile.timeLeft = 2;
            SimplePetAnimation(15f);
            Projectile.Center = new Vector2(Owner.MountedCenter.X - 40f, Owner.MountedCenter.Y - 35f);
            Projectile.light = 1;
        }
        public void SimplePetAnimation(float Speed)
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > Speed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > Main.projFrames[Projectile.type] - 1)
                    Projectile.frame = 0;
            }
        }
        //public override bool PreDraw(ref Color lightColor)
        //{
        //    Projectile.DrawProj(Color.White, 1);
        //    return false;
        //}
    }
}
