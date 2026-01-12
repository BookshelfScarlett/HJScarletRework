using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using System.Threading;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class StormSpearThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<StormSpear>().Texture;
        public ref float Timer => ref Projectile.ai[0];
        public bool IsHit = false;
        public override void ExSD()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            IsHit = true;
        }
        public override bool? CanDamage() => Timer > 10f;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(Color.White,drawTime: 1);
            return false;
        }
    }
}
