using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class EndlessWarLightening : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public NPC CurTarget = null;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 30;
            Projectile.SetupImmnuity(2);
            Projectile.penetrate = 1;
            Projectile.width = Projectile.height = 30;
            Projectile.timeLeft = 50 * 40;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;

        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            if (CurTarget.IsLegal())
            {
                Projectile.HomingTarget(CurTarget.Center, -1, 20, 20);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Projectile.numUpdates % 3 == 0)
            {
                ECSParticle.HighResolutionThunder(Projectile.Center.ToRandCirclePos(3), Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.DodgerBlue), Main.rand.Next(25, 45), 1, Projectile.rotation + PiOver2, Main.rand.NextFloat(.3f, .44f) * 1.7f, 0);
                ECSParticle.LightntingGlow(Projectile.Center, Projectile.SafeDir(), Color.RoyalBlue, 30, 1f, 1.35f);
            }
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
