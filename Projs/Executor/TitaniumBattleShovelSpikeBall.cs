using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class TitaniumBattleShovelSpikeBall : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.SpikyBall);
        public override Vector2 TileHitbox => new Vector2(12);
        public override void SetStaticDefaults()
        {
        }
        public override void ExSD()
        {
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.width = Projectile.height = 24;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = GetSeconds(12) * Projectile.MaxUpdates;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void ProjAI()
        {
            Projectile.AffactedByGrav(0.98f, 1f, 0.14f, 30);
            Projectile.rotation = Projectile.SpeedAffectRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            SB.FastDraw(projTex, drawPos, Color.White, Projectile.rotation, ori, Projectile.scale, 0); ;
            return false;
        }
    }
}
