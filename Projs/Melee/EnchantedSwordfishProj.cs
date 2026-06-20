using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class EnchantedSwordfishProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            base.ExSD();
        }
        public override void ProjAI()
        {
            base.ProjAI();
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override Vector2 TileHitbox => new Vector2(12);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
