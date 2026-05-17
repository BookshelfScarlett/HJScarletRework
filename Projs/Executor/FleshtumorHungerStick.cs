using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class FleshtumorHungerStick : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => $"Terraria/Images/NPC_{NPCID.TheHungry}";
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
            Main.projFrames[Type] = 6;
        }
        public override Vector2 TileHitbox => new Vector2(12);
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 2;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.SetupImmnuity(-1);
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.AddFrames(4, 6);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
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
