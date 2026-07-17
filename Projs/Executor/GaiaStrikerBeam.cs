using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Metaballs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class GaiaStrikerBeam : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * (Projectile.extraUpdates + 1);
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            int DustCount = 7;
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < DustCount; i++)
                BloodyMetaball.SpawnParticle(Projectile.Center.ToRandCirclePos(12) + Projectile.velocity / DustCount * i, Projectile.rotation.ToRotationVector2(), Main.rand.NextFloat(0.4f, 0.55f) * .86f, Projectile.rotation);
            Projectile.velocity *= 1.03f;
        }
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
