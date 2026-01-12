using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class TheDisasterProj : HJScarletFriendlyProj
    {
        public override ClassCategory UseDamage => ClassCategory.Melee;
        public override string Texture => HJScarletItemProj.Proj_Disaster.Path;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(16, 2);
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void AI()
        {
            base.AI();
        }
        public override bool PreKill(int timeLeft)
        {
            return true;
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
            return false;
        }
    }
}
