using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class WildPointerHoming : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Melee;
        public override string Texture => GetInstance<WildPointerThrown>().Texture;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(4, 2);
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.height = Projectile.width = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            //byd画这么大干什么
            Projectile.scale *= 0.7f;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            NPC target = Projectile.FindClosestTarget(500);
            if (target != null)
                Projectile.HomingTarget(target.Center, 1800f, 20f, 20f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.White, 4, rotFix: 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
