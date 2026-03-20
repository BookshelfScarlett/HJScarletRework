using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class LightBiteArrow : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(7, 4);
            Main.projFrames[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.hostile = false;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
        }
        public override bool? CanDamage() => true;
        public override void AI()
        {
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        public override bool PreKill(int timeLeft)
        {
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
