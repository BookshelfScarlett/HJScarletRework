using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Metaballs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class DreamingLightBeam : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6, 2);
        }
        public override void ExSD()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 16;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
            FlickerWater.SpawnParticle(Main.MouseWorld, RandVelTwoPi(1f, 8f), Vector2.One, 0, 120, HJScarletTexture.Texture_WhiteCircle.Value);
        }
        public override void ProjAI()
        {
            Projectile.Center = Main.MouseWorld;
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
