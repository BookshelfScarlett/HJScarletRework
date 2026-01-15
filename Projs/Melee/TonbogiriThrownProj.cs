using ContinentOfJourney.Projectiles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class TonbogiriThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<Tonbogiri>().Texture;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting();
        public override void ExSD()
        {
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = 60;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White, rotFix: ToRadians(135));
            Projectile.DrawProj(Color.White, rotFix:ToRadians(135));
            return false;
        }
    }
}
