using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class EndlessWarProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(5);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 128;
            Projectile.extraUpdates = 4;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
