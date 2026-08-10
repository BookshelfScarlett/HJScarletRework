using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    /// <summary>
    /// 薄荷台风的总管理
    /// </summary>
    public class EndlessWarMintTyphoon : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void ExSD()
        {
            Projectile.SetUpHeldProj();
            Projectile.timeLeft = GetSeconds(10);
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnFirstFrame()
        {
            ScreenDarknessSystem.AddScreenDarkness(.85f, GetSeconds(10));
        }
        public override void ProjAI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter < 3)
                return;
            Projectile.frameCounter = 0;
            float thunderPosX = Projectile.Center.X + Main.rand.NextFloat(-1400, 1400f);
            float thunderPosY = Projectile.Center.Y - Main.rand.NextFloat(600, 800);
            Vector2 thunderPos = new Vector2(thunderPosX, thunderPosY);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), thunderPos, -Vector2.UnitY, ProjectileType<EndlessWarMintTyphoonLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
