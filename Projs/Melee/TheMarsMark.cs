using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class TheMarsMark : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(10, 2);
        }
        public enum Style
        {
            Spawn,
            ToTarget
        }
        public ref float Timer => ref Projectile.ai[0];
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public int ParentProjectileIndex = -1;
        public bool DoStrike = false;
        public NPC AvaiTar = null;
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 6;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.extraUpdates = 0;
        }
        public override void AI()
        {
            Projectile.rotation += ToRadians(1f);
            //锁住射弹位置
            if (Projectile.GetTargetSafe(out NPC target, false))
            {
                //这里的Timer只会用于生成第一个射弹，然后消失
                Timer++;
                if (Timer < 20f)
                    return;

                Projectile.Center = Vector2.Lerp(Projectile.Center, target.Center, 0.1f);
                Vector2 spawnPos = Projectile.Center + Vector2.UnitY.RotatedByRandom(TwoPi) * 150f;
                Vector2 dir = (target.Center - spawnPos).SafeNormalize(Vector2.UnitX);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, dir * Main.rand.NextFloat(6f, 11f), ProjectileType<TheMarsPhantom>(), Projectile.originalDamage, 1.5f, Owner.whoAmI);
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
            }
            Projectile.Kill();

        }
        public override bool? CanDamage() => false;
    }
}
