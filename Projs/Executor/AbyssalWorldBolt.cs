using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    internal class AbyssalWorldBolt : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(7);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 8;
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 50;
            Projectile.penetrate = 1;
        }
        public override void ProjAI()
        {
            GreenWater.SpawnParticle(Projectile.Center.ToRandCirclePos(6), RandVelTwoPi(.3f, .9f),Vector2.One, 0, 40, HJScarletTexture.Texture_WhiteCircle.Value);
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
